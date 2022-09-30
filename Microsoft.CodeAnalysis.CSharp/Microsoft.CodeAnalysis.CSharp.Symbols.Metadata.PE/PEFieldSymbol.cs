using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.DocumentationComments;
using Microsoft.CodeAnalysis.CSharp.Emit;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE
{
    internal sealed class PEFieldSymbol : FieldSymbol
    {
        private struct PackedFlags
        {
            private const int HasDisallowNullAttribute = 1;

            private const int HasAllowNullAttribute = 2;

            private const int HasMaybeNullAttribute = 4;

            private const int HasNotNullAttribute = 8;

            private const int FlowAnalysisAnnotationsCompletionBit = 16;

            private int _bits;

            public bool SetFlowAnalysisAnnotations(FlowAnalysisAnnotations value)
            {
                int num = 16;
                if ((value & FlowAnalysisAnnotations.DisallowNull) != 0)
                {
                    num |= 1;
                }
                if ((value & FlowAnalysisAnnotations.AllowNull) != 0)
                {
                    num |= 2;
                }
                if ((value & FlowAnalysisAnnotations.MaybeNull) != 0)
                {
                    num |= 4;
                }
                if ((value & FlowAnalysisAnnotations.NotNull) != 0)
                {
                    num |= 8;
                }
                return ThreadSafeFlagOperations.Set(ref _bits, num);
            }

            public bool TryGetFlowAnalysisAnnotations(out FlowAnalysisAnnotations value)
            {
                int bits = _bits;
                value = FlowAnalysisAnnotations.None;
                if (((uint)bits & (true ? 1u : 0u)) != 0)
                {
                    value |= FlowAnalysisAnnotations.DisallowNull;
                }
                if (((uint)bits & 2u) != 0)
                {
                    value |= FlowAnalysisAnnotations.AllowNull;
                }
                if (((uint)bits & 4u) != 0)
                {
                    value |= FlowAnalysisAnnotations.MaybeNull;
                }
                if (((uint)bits & 8u) != 0)
                {
                    value |= FlowAnalysisAnnotations.NotNull;
                }
                return (bits & 0x10) != 0;
            }
        }

        private readonly FieldDefinitionHandle _handle;

        private readonly string _name;

        private readonly FieldAttributes _flags;

        private readonly PENamedTypeSymbol _containingType;

        private bool _lazyIsVolatile;

        private ImmutableArray<CSharpAttributeData> _lazyCustomAttributes;

        private ConstantValue _lazyConstantValue = Microsoft.CodeAnalysis.ConstantValue.Unset;

        private Tuple<CultureInfo, string> _lazyDocComment;

        private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;

        private ObsoleteAttributeData _lazyObsoleteAttributeData = ObsoleteAttributeData.Uninitialized;

        private TypeWithAnnotations.Boxed _lazyType;

        private int _lazyFixedSize;

        private NamedTypeSymbol _lazyFixedImplementationType;

        private PEEventSymbol _associatedEventOpt;

        private PackedFlags _packedFlags;

        public override Symbol ContainingSymbol => _containingType;

        public override NamedTypeSymbol ContainingType => _containingType;

        public override string Name => _name;

        internal FieldAttributes Flags => _flags;

        internal override bool HasSpecialName => (_flags & FieldAttributes.SpecialName) != 0;

        internal override bool HasRuntimeSpecialName => (_flags & FieldAttributes.RTSpecialName) != 0;

        internal override bool IsNotSerialized => (_flags & FieldAttributes.NotSerialized) != 0;

        internal override MarshalPseudoCustomAttributeData MarshallingInformation => null;

        internal override bool IsMarshalledExplicitly => (_flags & FieldAttributes.HasFieldMarshal) != 0;

        internal override UnmanagedType MarshallingType
        {
            get
            {
                if ((_flags & FieldAttributes.HasFieldMarshal) == 0)
                {
                    return 0;
                }
                return _containingType.ContainingPEModule.Module.GetMarshallingType(_handle);
            }
        }

        internal override ImmutableArray<byte> MarshallingDescriptor
        {
            get
            {
                if ((_flags & FieldAttributes.HasFieldMarshal) == 0)
                {
                    return default(ImmutableArray<byte>);
                }
                return _containingType.ContainingPEModule.Module.GetMarshallingDescriptor(_handle);
            }
        }

        internal override int? TypeLayoutOffset => _containingType.ContainingPEModule.Module.GetFieldOffset(_handle);

        internal FieldDefinitionHandle Handle => _handle;

        private PEModuleSymbol ContainingPEModule => ((PENamespaceSymbol)ContainingNamespace).ContainingPEModule;

        public override FlowAnalysisAnnotations FlowAnalysisAnnotations
        {
            get
            {
                if (!_packedFlags.TryGetFlowAnalysisAnnotations(out var value))
                {
                    value = DecodeFlowAnalysisAttributes(_containingType.ContainingPEModule.Module, _handle);
                    _packedFlags.SetFlowAnalysisAnnotations(value);
                }
                return value;
            }
        }

        public override bool IsFixedSizeBuffer
        {
            get
            {
                EnsureSignatureIsLoaded();
                return (object)_lazyFixedImplementationType != null;
            }
        }

        public override int FixedSize
        {
            get
            {
                EnsureSignatureIsLoaded();
                return _lazyFixedSize;
            }
        }

        public override Symbol AssociatedSymbol => _associatedEventOpt;

        public override bool IsReadOnly => (_flags & FieldAttributes.InitOnly) != 0;

        public override bool IsVolatile
        {
            get
            {
                EnsureSignatureIsLoaded();
                return _lazyIsVolatile;
            }
        }

        public override bool IsConst
        {
            get
            {
                if ((_flags & FieldAttributes.Literal) == 0)
                {
                    return GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false) != null;
                }
                return true;
            }
        }

        public override ImmutableArray<Location> Locations => _containingType.ContainingPEModule.MetadataLocation.Cast<MetadataLocation, Location>();

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override Accessibility DeclaredAccessibility
        {
            get
            {
                Accessibility accessibility = Accessibility.Private;
                switch (_flags & FieldAttributes.FieldAccessMask)
                {
                    case FieldAttributes.Assembly:
                        return Accessibility.Internal;
                    case FieldAttributes.FamORAssem:
                        return Accessibility.ProtectedOrInternal;
                    case FieldAttributes.FamANDAssem:
                        return Accessibility.ProtectedAndInternal;
                    case FieldAttributes.PrivateScope:
                    case FieldAttributes.Private:
                        return Accessibility.Private;
                    case FieldAttributes.Public:
                        return Accessibility.Public;
                    case FieldAttributes.Family:
                        return Accessibility.Protected;
                    default:
                        return Accessibility.Private;
                }
            }
        }

        public override bool IsStatic => (_flags & FieldAttributes.Static) != 0;

        internal override ObsoleteAttributeData ObsoleteAttributeData
        {
            get
            {
                ObsoleteAttributeHelpers.InitializeObsoleteDataFromMetadata(ref _lazyObsoleteAttributeData, _handle, (PEModuleSymbol)ContainingModule, ignoreByRefLikeMarker: false);
                return _lazyObsoleteAttributeData;
            }
        }

        internal sealed override CSharpCompilation DeclaringCompilation => null;

        internal PEFieldSymbol(PEModuleSymbol moduleSymbol, PENamedTypeSymbol containingType, FieldDefinitionHandle fieldDef)
        {
            _handle = fieldDef;
            _containingType = containingType;
            _packedFlags = default(PackedFlags);
            try
            {
                moduleSymbol.Module.GetFieldDefPropsOrThrow(fieldDef, out _name, out _flags);
            }
            catch (BadImageFormatException)
            {
                if (_name == null)
                {
                    _name = string.Empty;
                }
                _lazyCachedUseSiteInfo.Initialize(new CSDiagnosticInfo(ErrorCode.ERR_BindToBogus, this));
            }
        }

        internal void SetAssociatedEvent(PEEventSymbol eventSymbol)
        {
            if ((object)_associatedEventOpt == null)
            {
                _associatedEventOpt = eventSymbol;
            }
        }

        private void EnsureSignatureIsLoaded()
        {
            if (_lazyType == null)
            {
                PEModuleSymbol containingPEModule = _containingType.ContainingPEModule;
                TypeSymbol metadataType = new MetadataDecoder(containingPEModule, _containingType).DecodeFieldSignature(_handle, out ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers);
                ImmutableArray<CustomModifier> immutableArray = CSharpCustomModifier.Convert(customModifiers);
                TypeWithAnnotations metadataType2 = TypeWithAnnotations.Create(NativeIntegerTypeDecoder.TransformType(DynamicTypeDecoder.TransformType(metadataType, immutableArray.Length, _handle, containingPEModule), _handle, containingPEModule), NullableAnnotation.Oblivious, immutableArray);
                metadataType2 = NullableTypeDecoder.TransformType(metadataType2, _handle, containingPEModule, this, _containingType);
                metadataType2 = TupleTypeDecoder.DecodeTupleTypesIfApplicable(metadataType2, _handle, containingPEModule);
                _lazyIsVolatile = immutableArray.Any((CustomModifier m) => !m.IsOptional && m.Modifier.SpecialType == SpecialType.System_Runtime_CompilerServices_IsVolatile);
                if (immutableArray.IsEmpty && IsFixedBuffer(out var fixedSize, out var fixedElementType))
                {
                    _lazyFixedSize = fixedSize;
                    _lazyFixedImplementationType = metadataType2.Type as NamedTypeSymbol;
                    metadataType2 = TypeWithAnnotations.Create(new PointerTypeSymbol(TypeWithAnnotations.Create(fixedElementType)));
                }
                Interlocked.CompareExchange(ref _lazyType, new TypeWithAnnotations.Boxed(metadataType2), null);
            }
        }

        private bool IsFixedBuffer(out int fixedSize, out TypeSymbol fixedElementType)
        {
            fixedSize = 0;
            fixedElementType = null;
            PEModuleSymbol containingPEModule = ContainingPEModule;
            if (containingPEModule.Module.HasFixedBufferAttribute(_handle, out var elementTypeName, out var bufferSize))
            {
                TypeSymbol typeSymbolForSerializedType = new MetadataDecoder(containingPEModule).GetTypeSymbolForSerializedType(elementTypeName);
                if (typeSymbolForSerializedType.FixedBufferElementSizeInBytes() != 0)
                {
                    fixedSize = bufferSize;
                    fixedElementType = typeSymbolForSerializedType;
                    return true;
                }
            }
            return false;
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            EnsureSignatureIsLoaded();
            return _lazyType.Value;
        }

        private static FlowAnalysisAnnotations DecodeFlowAnalysisAttributes(PEModule module, FieldDefinitionHandle handle)
        {
            FlowAnalysisAnnotations flowAnalysisAnnotations = FlowAnalysisAnnotations.None;
            if (module.HasAttribute(handle, AttributeDescription.AllowNullAttribute))
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.AllowNull;
            }
            if (module.HasAttribute(handle, AttributeDescription.DisallowNullAttribute))
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.DisallowNull;
            }
            if (module.HasAttribute(handle, AttributeDescription.MaybeNullAttribute))
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.MaybeNull;
            }
            if (module.HasAttribute(handle, AttributeDescription.NotNullAttribute))
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.NotNull;
            }
            return flowAnalysisAnnotations;
        }

        internal override NamedTypeSymbol FixedImplementationType(PEModuleBuilder emitModule)
        {
            EnsureSignatureIsLoaded();
            return _lazyFixedImplementationType;
        }

        internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress, bool earlyDecodingWellKnownAttributes)
        {
            if (_lazyConstantValue == Microsoft.CodeAnalysis.ConstantValue.Unset)
            {
                ConstantValue value = null;
                if ((_flags & FieldAttributes.Literal) != 0)
                {
                    value = _containingType.ContainingPEModule.Module.GetConstantFieldValue(_handle);
                }
                if (base.Type.SpecialType == SpecialType.System_Decimal && _containingType.ContainingPEModule.Module.HasDecimalConstantAttribute(Handle, out var defaultValue))
                {
                    value = defaultValue;
                }
                Interlocked.CompareExchange(ref _lazyConstantValue, value, Microsoft.CodeAnalysis.ConstantValue.Unset);
            }
            return _lazyConstantValue;
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            if (_lazyCustomAttributes.IsDefault)
            {
                PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)ContainingModule;
                if (FilterOutDecimalConstantAttribute())
                {
                    ImmutableArray<CSharpAttributeData> customAttributesForToken = pEModuleSymbol.GetCustomAttributesForToken(_handle, out CustomAttributeHandle filteredOutAttribute, AttributeDescription.DecimalConstantAttribute);
                    ImmutableInterlocked.InterlockedInitialize(ref _lazyCustomAttributes, customAttributesForToken);
                }
                else
                {
                    pEModuleSymbol.LoadCustomAttributes(_handle, ref _lazyCustomAttributes);
                }
            }
            return _lazyCustomAttributes;
        }

        private bool FilterOutDecimalConstantAttribute()
        {
            ConstantValue constantValue;
            if (base.Type.SpecialType == SpecialType.System_Decimal && (object)(constantValue = GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false)) != null)
            {
                return constantValue.Discriminator == ConstantValueTypeDiscriminator.Decimal;
            }
            return false;
        }

        internal override IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
        {
            ImmutableArray<CSharpAttributeData>.Enumerator enumerator = GetAttributes().GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
            if (FilterOutDecimalConstantAttribute())
            {
                PEModuleSymbol containingPEModule = _containingType.ContainingPEModule;
                yield return new PEAttributeData(containingPEModule, containingPEModule.Module.FindLastTargetAttribute(_handle, AttributeDescription.DecimalConstantAttribute).Handle);
            }
        }

        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
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
