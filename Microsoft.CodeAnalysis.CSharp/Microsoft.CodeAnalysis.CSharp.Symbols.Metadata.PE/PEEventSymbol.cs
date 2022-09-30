using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.DocumentationComments;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE
{
    internal sealed class PEEventSymbol : EventSymbol
    {
        [Flags()]
        private enum Flags : byte
        {
            IsSpecialName = 1,
            IsRuntimeSpecialName = 2,
            CallMethodsDirectly = 4
        }

        private readonly string _name;

        private readonly PENamedTypeSymbol _containingType;

        private readonly EventDefinitionHandle _handle;

        private readonly TypeWithAnnotations _eventTypeWithAnnotations;

        private readonly PEMethodSymbol _addMethod;

        private readonly PEMethodSymbol _removeMethod;

        private readonly PEFieldSymbol? _associatedFieldOpt;

        private ImmutableArray<CSharpAttributeData> _lazyCustomAttributes;

        private Tuple<CultureInfo, string>? _lazyDocComment;

        private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;

        private ObsoleteAttributeData _lazyObsoleteAttributeData = Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized;

        private const int UnsetAccessibility = -1;

        private int _lazyDeclaredAccessibility = -1;

        private readonly Flags _flags;

        public override bool IsWindowsRuntimeEvent
        {
            get
            {
                NamedTypeSymbol eventRegistrationToken = ((PEModuleSymbol)ContainingModule).EventRegistrationToken;
                if (TypeSymbol.Equals(_addMethod.ReturnType, eventRegistrationToken, TypeCompareKind.ConsiderEverything) && _addMethod.ParameterCount == 1 && _removeMethod.ParameterCount == 1)
                {
                    return TypeSymbol.Equals(_removeMethod.Parameters[0].Type, eventRegistrationToken, TypeCompareKind.ConsiderEverything);
                }
                return false;
            }
        }

        internal override FieldSymbol? AssociatedField => _associatedFieldOpt;

        public override Symbol ContainingSymbol => _containingType;

        public override NamedTypeSymbol ContainingType => _containingType;

        public override string Name => _name;

        internal override bool HasSpecialName => (_flags & Flags.IsSpecialName) != 0;

        internal override bool HasRuntimeSpecialName => (_flags & Flags.IsRuntimeSpecialName) != 0;

        internal EventDefinitionHandle Handle => _handle;

        public override Accessibility DeclaredAccessibility
        {
            get
            {
                if (_lazyDeclaredAccessibility == -1)
                {
                    Accessibility declaredAccessibilityFromAccessors = PEPropertyOrEventHelpers.GetDeclaredAccessibilityFromAccessors(_addMethod, _removeMethod);
                    Interlocked.CompareExchange(ref _lazyDeclaredAccessibility, (int)declaredAccessibilityFromAccessors, -1);
                }
                return (Accessibility)_lazyDeclaredAccessibility;
            }
        }

        public override bool IsExtern
        {
            get
            {
                if (!_addMethod.IsExtern)
                {
                    return _removeMethod.IsExtern;
                }
                return true;
            }
        }

        public override bool IsAbstract
        {
            get
            {
                if (!_addMethod.IsAbstract)
                {
                    return _removeMethod.IsAbstract;
                }
                return true;
            }
        }

        public override bool IsSealed
        {
            get
            {
                if (!_addMethod.IsSealed)
                {
                    return _removeMethod.IsSealed;
                }
                return true;
            }
        }

        public override bool IsVirtual
        {
            get
            {
                if (!IsOverride && !IsAbstract)
                {
                    if (!_addMethod.IsVirtual)
                    {
                        return _removeMethod.IsVirtual;
                    }
                    return true;
                }
                return false;
            }
        }

        public override bool IsOverride
        {
            get
            {
                if (!_addMethod.IsOverride)
                {
                    return _removeMethod.IsOverride;
                }
                return true;
            }
        }

        public override bool IsStatic
        {
            get
            {
                if (_addMethod.IsStatic)
                {
                    return _removeMethod.IsStatic;
                }
                return false;
            }
        }

        public override TypeWithAnnotations TypeWithAnnotations => _eventTypeWithAnnotations;

        public override MethodSymbol AddMethod => _addMethod;

        public override MethodSymbol RemoveMethod => _removeMethod;

        public override ImmutableArray<Location> Locations => _containingType.ContainingPEModule.MetadataLocation.Cast<MetadataLocation, Location>();

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override ImmutableArray<EventSymbol> ExplicitInterfaceImplementations
        {
            get
            {
                if (_addMethod.ExplicitInterfaceImplementations.Length == 0 && _removeMethod.ExplicitInterfaceImplementations.Length == 0)
                {
                    return ImmutableArray<EventSymbol>.Empty;
                }
                ISet<EventSymbol> eventsForExplicitlyImplementedAccessor = PEPropertyOrEventHelpers.GetEventsForExplicitlyImplementedAccessor(_addMethod);
                eventsForExplicitlyImplementedAccessor.IntersectWith(PEPropertyOrEventHelpers.GetEventsForExplicitlyImplementedAccessor(_removeMethod));
                ArrayBuilder<EventSymbol> instance = ArrayBuilder<EventSymbol>.GetInstance();
                foreach (EventSymbol item in eventsForExplicitlyImplementedAccessor)
                {
                    instance.Add(item);
                }
                return instance.ToImmutableAndFree();
            }
        }

        internal override bool MustCallMethodsDirectly => (_flags & Flags.CallMethodsDirectly) != 0;

        internal override ObsoleteAttributeData ObsoleteAttributeData
        {
            get
            {
                ObsoleteAttributeHelpers.InitializeObsoleteDataFromMetadata(ref _lazyObsoleteAttributeData, _handle, (PEModuleSymbol)ContainingModule, ignoreByRefLikeMarker: false);
                return _lazyObsoleteAttributeData;
            }
        }

        internal sealed override CSharpCompilation? DeclaringCompilation => null;

        internal PEEventSymbol(PEModuleSymbol moduleSymbol, PENamedTypeSymbol containingType, EventDefinitionHandle handle, PEMethodSymbol addMethod, PEMethodSymbol removeMethod, MultiDictionary<string, PEFieldSymbol> privateFieldNameToSymbols)
        {
            _addMethod = addMethod;
            _removeMethod = removeMethod;
            _handle = handle;
            _containingType = containingType;
            EventAttributes flags = EventAttributes.None;
            EntityHandle type = default(EntityHandle);
            try
            {
                moduleSymbol.Module.GetEventDefPropsOrThrow(handle, out _name, out flags, out type);
            }
            catch (BadImageFormatException mrEx)
            {
                _name = _name ?? string.Empty;
                _lazyCachedUseSiteInfo.Initialize(new CSDiagnosticInfo(ErrorCode.ERR_BindToBogus, this));
                if (type.IsNil)
                {
                    _eventTypeWithAnnotations = TypeWithAnnotations.Create(new UnsupportedMetadataTypeSymbol(mrEx));
                }
            }
            TypeSymbol typeSymbol = _eventTypeWithAnnotations.Type;
            if (!_eventTypeWithAnnotations.HasType)
            {
                typeSymbol = new MetadataDecoder(moduleSymbol, containingType).GetTypeOfToken(type);
                TypeWithAnnotations metadataType = TypeWithAnnotations.Create(NativeIntegerTypeDecoder.TransformType(DynamicTypeDecoder.TransformType(typeSymbol, 0, handle, moduleSymbol), handle, moduleSymbol));
                metadataType = NullableTypeDecoder.TransformType(metadataType, handle, moduleSymbol, _containingType, _containingType);
                metadataType = (_eventTypeWithAnnotations = TupleTypeDecoder.DecodeTupleTypesIfApplicable(metadataType, handle, moduleSymbol));
            }
            bool isWindowsRuntimeEvent = IsWindowsRuntimeEvent;
            if (isWindowsRuntimeEvent ? (!DoModifiersMatch(_addMethod, _removeMethod)) : (!DoSignaturesMatch(moduleSymbol, typeSymbol, _addMethod, _removeMethod)))
            {
                _flags |= Flags.CallMethodsDirectly;
            }
            else
            {
                _addMethod.SetAssociatedEvent(this, MethodKind.EventAdd);
                _removeMethod.SetAssociatedEvent(this, MethodKind.EventRemove);
                PEFieldSymbol associatedField = GetAssociatedField(privateFieldNameToSymbols, isWindowsRuntimeEvent);
                if ((object)associatedField != null)
                {
                    _associatedFieldOpt = associatedField;
                    associatedField.SetAssociatedEvent(this);
                }
            }
            if ((flags & EventAttributes.SpecialName) != 0)
            {
                _flags |= Flags.IsSpecialName;
            }
            if ((flags & EventAttributes.RTSpecialName) != 0)
            {
                _flags |= Flags.IsRuntimeSpecialName;
            }
        }

        private PEFieldSymbol? GetAssociatedField(MultiDictionary<string, PEFieldSymbol> privateFieldNameToSymbols, bool isWindowsRuntimeEvent)
        {
            foreach (PEFieldSymbol item in privateFieldNameToSymbols[_name])
            {
                TypeSymbol type = item.Type;
                if (isWindowsRuntimeEvent)
                {
                    if (TypeSymbol.Equals(((PEModuleSymbol)ContainingModule).EventRegistrationTokenTable_T, type.OriginalDefinition, TypeCompareKind.ConsiderEverything) && TypeSymbol.Equals(_eventTypeWithAnnotations.Type, ((NamedTypeSymbol)type).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0].Type, TypeCompareKind.ConsiderEverything))
                    {
                        return item;
                    }
                }
                else if (TypeSymbol.Equals(type, _eventTypeWithAnnotations.Type, TypeCompareKind.ConsiderEverything))
                {
                    return item;
                }
            }
            return null;
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            if (_lazyCustomAttributes.IsDefault)
            {
                ((PEModuleSymbol)ContainingModule).LoadCustomAttributes(_handle, ref _lazyCustomAttributes);
            }
            return _lazyCustomAttributes;
        }

        internal override IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
        {
            return GetAttributes();
        }

        private static bool DoSignaturesMatch(PEModuleSymbol moduleSymbol, TypeSymbol eventType, PEMethodSymbol addMethod, PEMethodSymbol removeMethod)
        {
            if ((eventType.IsDelegateType() || eventType.IsErrorType()) && DoesSignatureMatch(moduleSymbol, eventType, addMethod) && DoesSignatureMatch(moduleSymbol, eventType, removeMethod))
            {
                return DoModifiersMatch(addMethod, removeMethod);
            }
            return false;
        }

        private static bool DoModifiersMatch(PEMethodSymbol addMethod, PEMethodSymbol removeMethod)
        {
            if (addMethod.IsExtern == removeMethod.IsExtern)
            {
                return addMethod.IsStatic == removeMethod.IsStatic;
            }
            return false;
        }

        private static bool DoesSignatureMatch(PEModuleSymbol moduleSymbol, TypeSymbol eventType, PEMethodSymbol method)
        {
            MetadataDecoder metadataDecoder = new MetadataDecoder(moduleSymbol, method);
            ParamInfo<TypeSymbol>[] signatureForMethod = metadataDecoder.GetSignatureForMethod(method.Handle, out SignatureHeader signatureHeader, out BadImageFormatException metadataException, setParamHandles: false);
            if (metadataException != null)
            {
                return false;
            }
            return metadataDecoder.DoesSignatureMatchEvent(eventType, signatureForMethod);
        }

        public override string GetDocumentationCommentXml(CultureInfo? preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PEDocumentationCommentUtils.GetDocumentationComment(this, _containingType.ContainingPEModule, preferredCulture, cancellationToken, ref _lazyDocComment);
        }

        public override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            AssemblySymbol primaryDependency = base.PrimaryDependency;
            if (!_lazyCachedUseSiteInfo.IsInitialized)
            {
                UseSiteInfo<AssemblySymbol> result = new UseSiteInfo<AssemblySymbol>(primaryDependency);
                CalculateUseSiteDiagnostic(ref result);
                _lazyCachedUseSiteInfo.Initialize(primaryDependency, result);
            }
            return _lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency);
        }
    }
}
