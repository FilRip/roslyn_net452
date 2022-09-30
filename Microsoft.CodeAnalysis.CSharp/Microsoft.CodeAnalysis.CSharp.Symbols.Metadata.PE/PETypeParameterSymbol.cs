using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE
{
    internal sealed class PETypeParameterSymbol : TypeParameterSymbol
    {
        private readonly Symbol _containingSymbol;

        private readonly GenericParameterHandle _handle;

        private readonly string _name;

        private readonly ushort _ordinal;

        private CachedUseSiteInfo<AssemblySymbol> _lazyCachedConstraintsUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;

        private readonly GenericParameterAttributes _flags;

        private ThreeState _lazyHasIsUnmanagedConstraint;

        private TypeParameterBounds _lazyBounds = TypeParameterBounds.Unset;

        private ImmutableArray<TypeWithAnnotations> _lazyDeclaredConstraintTypes;

        private ImmutableArray<CSharpAttributeData> _lazyCustomAttributes;

        public override TypeParameterKind TypeParameterKind
        {
            get
            {
                if (ContainingSymbol.Kind != SymbolKind.Method)
                {
                    return TypeParameterKind.Type;
                }
                return TypeParameterKind.Method;
            }
        }

        public override int Ordinal => _ordinal;

        public override string Name => _name;

        internal GenericParameterHandle Handle => _handle;

        public override Symbol ContainingSymbol => _containingSymbol;

        public override AssemblySymbol ContainingAssembly => _containingSymbol.ContainingAssembly;

        public override ImmutableArray<Location> Locations => _containingSymbol.Locations;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override bool HasConstructorConstraint => (_flags & GenericParameterAttributes.DefaultConstructorConstraint) != 0;

        public override bool HasReferenceTypeConstraint => (_flags & GenericParameterAttributes.ReferenceTypeConstraint) != 0;

        public override bool IsReferenceTypeFromConstraintTypes => TypeParameterSymbol.CalculateIsReferenceTypeFromConstraintTypes(base.ConstraintTypesNoUseSiteDiagnostics);

        internal override bool? ReferenceTypeConstraintIsNullable
        {
            get
            {
                if (!HasReferenceTypeConstraint)
                {
                    return false;
                }
                return GetNullableAttributeValue() switch
                {
                    2 => true,
                    1 => false,
                    _ => null,
                };
            }
        }

        public override bool HasNotNullConstraint
        {
            get
            {
                if ((_flags & (GenericParameterAttributes.ReferenceTypeConstraint | GenericParameterAttributes.NotNullableValueTypeConstraint)) == 0)
                {
                    return GetNullableAttributeValue() == 1;
                }
                return false;
            }
        }

        internal override bool? IsNotNullable
        {
            get
            {
                if ((_flags & (GenericParameterAttributes.ReferenceTypeConstraint | GenericParameterAttributes.NotNullableValueTypeConstraint)) == 0 && !HasNotNullConstraint)
                {
                    PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)ContainingModule;
                    PEModule module = pEModuleSymbol.Module;
                    GenericParameterConstraintHandleCollection constraintHandleCollection = GetConstraintHandleCollection(module);
                    if (constraintHandleCollection.Count == 0)
                    {
                        if (GetNullableAttributeValue() == 2)
                        {
                            return false;
                        }
                        return null;
                    }
                    if (GetDeclaredConstraintTypes(ConsList<PETypeParameterSymbol>.Empty).IsEmpty)
                    {
                        ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
                        MetadataDecoder decoderForConstraintTypes = GetDecoderForConstraintTypes(pEModuleSymbol);
                        bool hasUnmanagedModreqPattern = false;
                        MetadataReader metadataReader = module.MetadataReader;
                        foreach (GenericParameterConstraintHandle item in constraintHandleCollection)
                        {
                            TypeWithAnnotations constraintTypeOrDefault = GetConstraintTypeOrDefault(pEModuleSymbol, metadataReader, decoderForConstraintTypes, item, ref hasUnmanagedModreqPattern);
                            if (constraintTypeOrDefault.HasType)
                            {
                                instance.Add(constraintTypeOrDefault);
                            }
                        }
                        return TypeParameterSymbol.IsNotNullableFromConstraintTypes(instance.ToImmutableAndFree());
                    }
                }
                return CalculateIsNotNullable();
            }
        }

        public override bool HasValueTypeConstraint => (_flags & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0;

        public override bool IsValueTypeFromConstraintTypes => TypeParameterSymbol.CalculateIsValueTypeFromConstraintTypes(base.ConstraintTypesNoUseSiteDiagnostics);

        public override bool HasUnmanagedTypeConstraint
        {
            get
            {
                GetDeclaredConstraintTypes(ConsList<PETypeParameterSymbol>.Empty);
                return _lazyHasIsUnmanagedConstraint.Value();
            }
        }

        public override VarianceKind Variance => (VarianceKind)(_flags & GenericParameterAttributes.VarianceMask);

        internal sealed override CSharpCompilation DeclaringCompilation => null;

        internal PETypeParameterSymbol(PEModuleSymbol moduleSymbol, PENamedTypeSymbol definingNamedType, ushort ordinal, GenericParameterHandle handle)
            : this(moduleSymbol, (Symbol)definingNamedType, ordinal, handle)
        {
        }

        internal PETypeParameterSymbol(PEModuleSymbol moduleSymbol, PEMethodSymbol definingMethod, ushort ordinal, GenericParameterHandle handle)
            : this(moduleSymbol, (Symbol)definingMethod, ordinal, handle)
        {
        }

        private PETypeParameterSymbol(PEModuleSymbol moduleSymbol, Symbol definingSymbol, ushort ordinal, GenericParameterHandle handle)
        {
            _containingSymbol = definingSymbol;
            GenericParameterAttributes flags = GenericParameterAttributes.None;
            try
            {
                moduleSymbol.Module.GetGenericParamPropsOrThrow(handle, out _name, out flags);
            }
            catch (BadImageFormatException)
            {
                if (_name == null)
                {
                    _name = string.Empty;
                }
                _lazyCachedConstraintsUseSiteInfo.Initialize(new CSDiagnosticInfo(ErrorCode.ERR_BindToBogus, this));
            }
            _flags = (((flags & GenericParameterAttributes.NotNullableValueTypeConstraint) == 0) ? flags : (flags & ~GenericParameterAttributes.DefaultConstructorConstraint));
            _ordinal = ordinal;
            _handle = handle;
        }

        private ImmutableArray<TypeWithAnnotations> GetDeclaredConstraintTypes(ConsList<PETypeParameterSymbol> inProgress)
        {
            if (_lazyDeclaredConstraintTypes.IsDefault)
            {
                PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)ContainingModule;
                PEModule module = pEModuleSymbol.Module;
                GenericParameterConstraintHandleCollection constraintHandleCollection = GetConstraintHandleCollection(module);
                bool hasUnmanagedModreqPattern = false;
                ImmutableArray<TypeWithAnnotations> value;
                if (constraintHandleCollection.Count > 0)
                {
                    ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
                    MetadataDecoder decoderForConstraintTypes = GetDecoderForConstraintTypes(pEModuleSymbol);
                    TypeWithAnnotations bestObjectConstraint = default(TypeWithAnnotations);
                    MetadataReader metadataReader = module.MetadataReader;
                    foreach (GenericParameterConstraintHandle item in constraintHandleCollection)
                    {
                        TypeWithAnnotations constraintTypeOrDefault = GetConstraintTypeOrDefault(pEModuleSymbol, metadataReader, decoderForConstraintTypes, item, ref hasUnmanagedModreqPattern);
                        if (constraintTypeOrDefault.HasType && !ConstraintsHelper.IsObjectConstraint(constraintTypeOrDefault, ref bestObjectConstraint))
                        {
                            instance.Add(constraintTypeOrDefault);
                        }
                    }
                    if (bestObjectConstraint.HasType && ConstraintsHelper.IsObjectConstraintSignificant(CalculateIsNotNullableFromNonTypeConstraints(), bestObjectConstraint))
                    {
                        if (instance.Count == 0)
                        {
                            if (bestObjectConstraint.NullableAnnotation.IsOblivious() && !HasReferenceTypeConstraint)
                            {
                                bestObjectConstraint = default(TypeWithAnnotations);
                            }
                        }
                        else
                        {
                            inProgress = inProgress.Prepend(this);
                            ArrayBuilder<TypeWithAnnotations>.Enumerator enumerator2 = instance.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                if (!ConstraintsHelper.IsObjectConstraintSignificant(IsNotNullableFromConstraintType(enumerator2.Current, inProgress, out var _), bestObjectConstraint))
                                {
                                    bestObjectConstraint = default(TypeWithAnnotations);
                                    break;
                                }
                            }
                        }
                        if (bestObjectConstraint.HasType)
                        {
                            instance.Insert(0, bestObjectConstraint);
                        }
                    }
                    value = instance.ToImmutableAndFree();
                }
                else
                {
                    value = ImmutableArray<TypeWithAnnotations>.Empty;
                }
                if ((hasUnmanagedModreqPattern && (_flags & GenericParameterAttributes.NotNullableValueTypeConstraint) == 0) || hasUnmanagedModreqPattern != module.HasIsUnmanagedAttribute(_handle))
                {
                    hasUnmanagedModreqPattern = false;
                    _lazyCachedConstraintsUseSiteInfo.InterlockedCompareExchange(null, new UseSiteInfo<AssemblySymbol>(new CSDiagnosticInfo(ErrorCode.ERR_BindToBogus, this)));
                }
                _lazyHasIsUnmanagedConstraint = hasUnmanagedModreqPattern.ToThreeState();
                ImmutableInterlocked.InterlockedInitialize(ref _lazyDeclaredConstraintTypes, value);
            }
            return _lazyDeclaredConstraintTypes;
        }

        private MetadataDecoder GetDecoderForConstraintTypes(PEModuleSymbol moduleSymbol)
        {
            if (_containingSymbol.Kind == SymbolKind.Method)
            {
                return new MetadataDecoder(moduleSymbol, (PEMethodSymbol)_containingSymbol);
            }
            return new MetadataDecoder(moduleSymbol, (PENamedTypeSymbol)_containingSymbol);
        }

        private TypeWithAnnotations GetConstraintTypeOrDefault(PEModuleSymbol moduleSymbol, MetadataReader metadataReader, MetadataDecoder tokenDecoder, GenericParameterConstraintHandle constraintHandle, ref bool hasUnmanagedModreqPattern)
        {
            TypeSymbol typeSymbol = tokenDecoder.DecodeGenericParameterConstraint(metadataReader.GetGenericParameterConstraint(constraintHandle).Type, out ImmutableArray<ModifierInfo<TypeSymbol>> modifiers);
            if (!modifiers.IsDefaultOrEmpty && modifiers.Length > 1)
            {
                typeSymbol = new UnsupportedMetadataTypeSymbol();
            }
            else if (typeSymbol.SpecialType == SpecialType.System_ValueType)
            {
                if (!modifiers.IsDefaultOrEmpty)
                {
                    ModifierInfo<TypeSymbol> modifierInfo = modifiers.Single();
                    if (!modifierInfo.IsOptional && modifierInfo.Modifier.IsWellKnownTypeUnmanagedType())
                    {
                        hasUnmanagedModreqPattern = true;
                    }
                    else
                    {
                        typeSymbol = new UnsupportedMetadataTypeSymbol();
                    }
                }
                if (typeSymbol.SpecialType == SpecialType.System_ValueType && (_flags & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
                {
                    return default(TypeWithAnnotations);
                }
            }
            else if (!modifiers.IsDefaultOrEmpty)
            {
                typeSymbol = new UnsupportedMetadataTypeSymbol();
            }
            return TupleTypeDecoder.DecodeTupleTypesIfApplicable(NullableTypeDecoder.TransformType(TypeWithAnnotations.Create(typeSymbol), constraintHandle, moduleSymbol, _containingSymbol, _containingSymbol), constraintHandle, moduleSymbol);
        }

        private static bool? IsNotNullableFromConstraintType(TypeWithAnnotations constraintType, ConsList<PETypeParameterSymbol> inProgress, out bool isNonNullableValueType)
        {
            if (!(constraintType.Type is PETypeParameterSymbol pETypeParameterSymbol) || (object)pETypeParameterSymbol.ContainingSymbol != inProgress.Head.ContainingSymbol || pETypeParameterSymbol.GetConstraintHandleCollection().Count == 0)
            {
                return TypeParameterSymbol.IsNotNullableFromConstraintType(constraintType, out isNonNullableValueType);
            }
            bool? flag = pETypeParameterSymbol.CalculateIsNotNullable(inProgress, out isNonNullableValueType);
            if (isNonNullableValueType)
            {
                return true;
            }
            if (constraintType.NullableAnnotation.IsAnnotated() || flag == false)
            {
                return false;
            }
            if (constraintType.NullableAnnotation.IsOblivious() || !flag.HasValue)
            {
                return null;
            }
            return true;
        }

        private bool? CalculateIsNotNullable(ConsList<PETypeParameterSymbol> inProgress, out bool isNonNullableValueType)
        {
            if (inProgress.ContainsReference(this))
            {
                isNonNullableValueType = false;
                return false;
            }
            if (HasValueTypeConstraint)
            {
                isNonNullableValueType = true;
                return true;
            }
            bool? flag = CalculateIsNotNullableFromNonTypeConstraints();
            ImmutableArray<TypeWithAnnotations> declaredConstraintTypes = GetDeclaredConstraintTypes(inProgress);
            if (declaredConstraintTypes.IsEmpty)
            {
                isNonNullableValueType = false;
                return flag;
            }
            bool? flag2 = IsNotNullableFromConstraintTypes(declaredConstraintTypes, inProgress, out isNonNullableValueType);
            if (isNonNullableValueType)
            {
                return true;
            }
            if (flag2 == true || flag == false)
            {
                return flag2;
            }
            return flag;
        }

        private static bool? IsNotNullableFromConstraintTypes(ImmutableArray<TypeWithAnnotations> constraintTypes, ConsList<PETypeParameterSymbol> inProgress, out bool isNonNullableValueType)
        {
            isNonNullableValueType = false;
            bool? flag = false;
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = constraintTypes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                bool? flag2 = IsNotNullableFromConstraintType(enumerator.Current, inProgress, out isNonNullableValueType);
                if (isNonNullableValueType)
                {
                    return true;
                }
                if (flag2 == true)
                {
                    flag = true;
                }
                else if (!flag2.HasValue && flag == false)
                {
                    flag = null;
                }
            }
            return flag;
        }

        private GenericParameterConstraintHandleCollection GetConstraintHandleCollection(PEModule module)
        {
            try
            {
                return module.MetadataReader.GetGenericParameter(_handle).GetConstraints();
            }
            catch (BadImageFormatException)
            {
                GenericParameterConstraintHandleCollection result = default(GenericParameterConstraintHandleCollection);
                _lazyCachedConstraintsUseSiteInfo.InterlockedCompareExchange(null, new UseSiteInfo<AssemblySymbol>(new CSDiagnosticInfo(ErrorCode.ERR_BindToBogus, this)));
                return result;
            }
        }

        private GenericParameterConstraintHandleCollection GetConstraintHandleCollection()
        {
            return GetConstraintHandleCollection(((PEModuleSymbol)ContainingModule).Module);
        }

        private byte GetNullableAttributeValue()
        {
            if (((PEModuleSymbol)ContainingModule).Module.HasNullableAttribute(_handle, out var defaultTransform, out var _))
            {
                return defaultTransform;
            }
            return _containingSymbol.GetNullableContextValue().GetValueOrDefault();
        }

        internal override void EnsureAllConstraintsAreResolved()
        {
            if (!_lazyBounds.IsSet())
            {
                TypeParameterSymbol.EnsureAllConstraintsAreResolved((_containingSymbol.Kind == SymbolKind.Method) ? ((PEMethodSymbol)_containingSymbol).TypeParameters : ((PENamedTypeSymbol)_containingSymbol).TypeParameters);
            }
        }

        internal override ImmutableArray<TypeWithAnnotations> GetConstraintTypes(ConsList<TypeParameterSymbol> inProgress)
        {
            return GetBounds(inProgress)?.ConstraintTypes ?? ImmutableArray<TypeWithAnnotations>.Empty;
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfaces(ConsList<TypeParameterSymbol> inProgress)
        {
            return GetBounds(inProgress)?.Interfaces ?? ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override NamedTypeSymbol GetEffectiveBaseClass(ConsList<TypeParameterSymbol> inProgress)
        {
            TypeParameterBounds bounds = GetBounds(inProgress);
            if (bounds == null)
            {
                return GetDefaultBaseType();
            }
            return bounds.EffectiveBaseClass;
        }

        internal override TypeSymbol GetDeducedBaseType(ConsList<TypeParameterSymbol> inProgress)
        {
            TypeParameterBounds bounds = GetBounds(inProgress);
            if (bounds == null)
            {
                return GetDefaultBaseType();
            }
            return bounds.DeducedBaseType;
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            if (_lazyCustomAttributes.IsDefault)
            {
                ImmutableArray<CSharpAttributeData> customAttributesForToken = ((PEModuleSymbol)ContainingModule).GetCustomAttributesForToken(Handle, out CustomAttributeHandle filteredOutAttribute, HasUnmanagedTypeConstraint ? AttributeDescription.IsUnmanagedAttribute : default(AttributeDescription));
                ImmutableInterlocked.InterlockedInitialize(ref _lazyCustomAttributes, customAttributesForToken);
            }
            return _lazyCustomAttributes;
        }

        private TypeParameterBounds GetBounds(ConsList<TypeParameterSymbol> inProgress)
        {
            if (_lazyBounds == TypeParameterBounds.Unset)
            {
                ImmutableArray<TypeWithAnnotations> declaredConstraintTypes = GetDeclaredConstraintTypes(ConsList<PETypeParameterSymbol>.Empty);
                ArrayBuilder<TypeParameterDiagnosticInfo> instance = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
                ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder = null;
                bool inherited = _containingSymbol.Kind == SymbolKind.Method && ((MethodSymbol)_containingSymbol).IsOverride;
                TypeParameterBounds value = this.ResolveBounds(ContainingAssembly.CorLibrary, inProgress.Prepend(this), declaredConstraintTypes, inherited, null, instance, ref useSiteDiagnosticsBuilder, default(CompoundUseSiteInfo<AssemblySymbol>));
                if (useSiteDiagnosticsBuilder != null)
                {
                    instance.AddRange(useSiteDiagnosticsBuilder);
                }
                AssemblySymbol primaryDependency = base.PrimaryDependency;
                UseSiteInfo<AssemblySymbol> result = new UseSiteInfo<AssemblySymbol>(primaryDependency);
                ArrayBuilder<TypeParameterDiagnosticInfo>.Enumerator enumerator = instance.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    MergeUseSiteInfo(ref result, enumerator.Current.UseSiteInfo);
                    DiagnosticInfo? diagnosticInfo = result.DiagnosticInfo;
                    if (diagnosticInfo != null && diagnosticInfo!.Severity == DiagnosticSeverity.Error)
                    {
                        break;
                    }
                }
                instance.Free();
                _lazyCachedConstraintsUseSiteInfo.InterlockedCompareExchange(primaryDependency, result);
                Interlocked.CompareExchange(ref _lazyBounds, value, TypeParameterBounds.Unset);
            }
            return _lazyBounds;
        }

        internal override UseSiteInfo<AssemblySymbol> GetConstraintsUseSiteErrorInfo()
        {
            EnsureAllConstraintsAreResolved();
            return _lazyCachedConstraintsUseSiteInfo.ToUseSiteInfo(base.PrimaryDependency);
        }

        private NamedTypeSymbol GetDefaultBaseType()
        {
            return ContainingAssembly.GetSpecialType(SpecialType.System_Object);
        }
    }
}
