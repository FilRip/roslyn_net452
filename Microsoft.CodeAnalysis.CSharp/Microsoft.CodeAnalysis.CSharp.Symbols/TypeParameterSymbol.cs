using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class TypeParameterSymbol : TypeSymbol, IGenericParameterReference, ITypeReference, IReference, INamedEntity, IParameterListEntry, IGenericMethodParameterReference, IGenericTypeParameterReference, IGenericParameter, IGenericMethodParameter, IGenericTypeParameter, ITypeParameterSymbolInternal, ITypeSymbolInternal, INamespaceOrTypeSymbolInternal, ISymbolInternal
    {
        bool ITypeReference.IsEnum => false;

        bool ITypeReference.IsValueType => false;

        Microsoft.Cci.PrimitiveTypeCode ITypeReference.TypeCode => Microsoft.Cci.PrimitiveTypeCode.NotPrimitive;

        TypeDefinitionHandle ITypeReference.TypeDef => default(TypeDefinitionHandle);

        IGenericMethodParameter IGenericParameter.AsGenericMethodParameter
        {
            get
            {
                if (AdaptedTypeParameterSymbol.ContainingSymbol.Kind == SymbolKind.Method)
                {
                    return this;
                }
                return null;
            }
        }

        IGenericMethodParameterReference ITypeReference.AsGenericMethodParameterReference
        {
            get
            {
                if (AdaptedTypeParameterSymbol.ContainingSymbol.Kind == SymbolKind.Method)
                {
                    return this;
                }
                return null;
            }
        }

        IGenericTypeInstanceReference ITypeReference.AsGenericTypeInstanceReference => null;

        IGenericTypeParameter IGenericParameter.AsGenericTypeParameter
        {
            get
            {
                if (AdaptedTypeParameterSymbol.ContainingSymbol.Kind == SymbolKind.NamedType)
                {
                    return this;
                }
                return null;
            }
        }

        IGenericTypeParameterReference ITypeReference.AsGenericTypeParameterReference
        {
            get
            {
                if (AdaptedTypeParameterSymbol.ContainingSymbol.Kind == SymbolKind.NamedType)
                {
                    return this;
                }
                return null;
            }
        }

        INamespaceTypeReference ITypeReference.AsNamespaceTypeReference => null;

        INestedTypeReference ITypeReference.AsNestedTypeReference => null;

        ISpecializedNestedTypeReference ITypeReference.AsSpecializedNestedTypeReference => null;

        string INamedEntity.Name => AdaptedTypeParameterSymbol.MetadataName;

        ushort IParameterListEntry.Index => (ushort)AdaptedTypeParameterSymbol.Ordinal;

        IMethodReference IGenericMethodParameterReference.DefiningMethod => ((MethodSymbol)AdaptedTypeParameterSymbol.ContainingSymbol).GetCciAdapter();

        ITypeReference IGenericTypeParameterReference.DefiningType => ((NamedTypeSymbol)AdaptedTypeParameterSymbol.ContainingSymbol).GetCciAdapter();

        bool IGenericParameter.MustBeReferenceType => AdaptedTypeParameterSymbol.HasReferenceTypeConstraint;

        bool IGenericParameter.MustBeValueType
        {
            get
            {
                if (!AdaptedTypeParameterSymbol.HasValueTypeConstraint)
                {
                    return AdaptedTypeParameterSymbol.HasUnmanagedTypeConstraint;
                }
                return true;
            }
        }

        bool IGenericParameter.MustHaveDefaultConstructor
        {
            get
            {
                if (!AdaptedTypeParameterSymbol.HasConstructorConstraint && !AdaptedTypeParameterSymbol.HasValueTypeConstraint)
                {
                    return AdaptedTypeParameterSymbol.HasUnmanagedTypeConstraint;
                }
                return true;
            }
        }

        TypeParameterVariance IGenericParameter.Variance => AdaptedTypeParameterSymbol.Variance switch
        {
            VarianceKind.None => TypeParameterVariance.NonVariant,
            VarianceKind.In => TypeParameterVariance.Contravariant,
            VarianceKind.Out => TypeParameterVariance.Covariant,
            _ => throw ExceptionUtilities.UnexpectedValue(AdaptedTypeParameterSymbol.Variance),
        };

        IMethodDefinition IGenericMethodParameter.DefiningMethod => ((MethodSymbol)AdaptedTypeParameterSymbol.ContainingSymbol).GetCciAdapter();

        ITypeDefinition IGenericTypeParameter.DefiningType => ((NamedTypeSymbol)AdaptedTypeParameterSymbol.ContainingSymbol).GetCciAdapter();

        internal TypeParameterSymbol AdaptedTypeParameterSymbol => this;

        public new virtual TypeParameterSymbol OriginalDefinition => this;

        protected sealed override TypeSymbol OriginalTypeSymbolDefinition => OriginalDefinition;

        public virtual TypeParameterSymbol ReducedFrom => null;

        public abstract int Ordinal { get; }

        internal ImmutableArray<TypeWithAnnotations> ConstraintTypesNoUseSiteDiagnostics
        {
            get
            {
                EnsureAllConstraintsAreResolved();
                return GetConstraintTypes(ConsList<TypeParameterSymbol>.Empty);
            }
        }

        public abstract bool HasConstructorConstraint { get; }

        public abstract TypeParameterKind TypeParameterKind { get; }

        public MethodSymbol DeclaringMethod => ContainingSymbol as MethodSymbol;

        public NamedTypeSymbol DeclaringType => ContainingSymbol as NamedTypeSymbol;

        public sealed override SymbolKind Kind => SymbolKind.TypeParameter;

        public sealed override TypeKind TypeKind => TypeKind.TypeParameter;

        public sealed override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public sealed override bool IsStatic => false;

        public sealed override bool IsAbstract => false;

        public sealed override bool IsSealed => false;

        internal sealed override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => null;

        internal NamedTypeSymbol EffectiveBaseClassNoUseSiteDiagnostics
        {
            get
            {
                EnsureAllConstraintsAreResolved();
                return GetEffectiveBaseClass(ConsList<TypeParameterSymbol>.Empty);
            }
        }

        internal ImmutableArray<NamedTypeSymbol> EffectiveInterfacesNoUseSiteDiagnostics
        {
            get
            {
                EnsureAllConstraintsAreResolved();
                return GetInterfaces(ConsList<TypeParameterSymbol>.Empty);
            }
        }

        internal TypeSymbol DeducedBaseTypeNoUseSiteDiagnostics
        {
            get
            {
                EnsureAllConstraintsAreResolved();
                return GetDeducedBaseType(ConsList<TypeParameterSymbol>.Empty);
            }
        }

        internal ImmutableArray<NamedTypeSymbol> AllEffectiveInterfacesNoUseSiteDiagnostics => base.GetAllInterfaces();

        public sealed override bool IsReferenceType
        {
            get
            {
                if (HasReferenceTypeConstraint)
                {
                    return true;
                }
                return IsReferenceTypeFromConstraintTypes;
            }
        }

        internal abstract bool? IsNotNullable { get; }

        public sealed override bool IsValueType
        {
            get
            {
                if (HasValueTypeConstraint)
                {
                    return true;
                }
                return IsValueTypeFromConstraintTypes;
            }
        }

        public sealed override bool IsRefLikeType => false;

        public sealed override bool IsReadOnly => false;

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

        public abstract bool HasReferenceTypeConstraint { get; }

        public abstract bool IsReferenceTypeFromConstraintTypes { get; }

        internal abstract bool? ReferenceTypeConstraintIsNullable { get; }

        public abstract bool HasNotNullConstraint { get; }

        public abstract bool HasValueTypeConstraint { get; }

        public abstract bool IsValueTypeFromConstraintTypes { get; }

        public abstract bool HasUnmanagedTypeConstraint { get; }

        public abstract VarianceKind Variance { get; }

        internal override bool IsRecord => false;

        internal override bool IsRecordStruct => false;

        ITypeDefinition ITypeReference.GetResolvedType(EmitContext context)
        {
            return null;
        }

        INamespaceTypeDefinition ITypeReference.AsNamespaceTypeDefinition(EmitContext context)
        {
            return null;
        }

        INestedTypeDefinition ITypeReference.AsNestedTypeDefinition(EmitContext context)
        {
            return null;
        }

        ITypeDefinition ITypeReference.AsTypeDefinition(EmitContext context)
        {
            return null;
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            throw ExceptionUtilities.Unreachable;
        }

        IDefinition IReference.AsDefinition(EmitContext context)
        {
            return null;
        }

        IEnumerable<TypeReferenceWithAttributes> IGenericParameter.GetConstraints(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            bool seenValueType = false;
            if (AdaptedTypeParameterSymbol.HasUnmanagedTypeConstraint)
            {
                INamedTypeReference specialType = moduleBeingBuilt.GetSpecialType(SpecialType.System_ValueType, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
                CustomModifier item = CSharpCustomModifier.CreateRequired(moduleBeingBuilt.Compilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_UnmanagedType));
                yield return new TypeReferenceWithAttributes(new ModifiedTypeReference(specialType, ImmutableArray.Create((ICustomModifier)item)));
                seenValueType = true;
            }
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = AdaptedTypeParameterSymbol.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeWithAnnotations current = enumerator.Current;
                SpecialType specialType2 = current.SpecialType;
                if (specialType2 != SpecialType.System_Object && specialType2 == SpecialType.System_ValueType)
                {
                    seenValueType = true;
                }
                ITypeReference typeRef = moduleBeingBuilt.Translate(current.Type, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
                yield return current.GetTypeRefWithAttributes(moduleBeingBuilt, AdaptedTypeParameterSymbol, typeRef);
            }
            if (AdaptedTypeParameterSymbol.HasValueTypeConstraint && !seenValueType)
            {
                INamedTypeReference specialType3 = moduleBeingBuilt.GetSpecialType(SpecialType.System_ValueType, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
                yield return new TypeReferenceWithAttributes(specialType3);
            }
        }

        internal new TypeParameterSymbol GetCciAdapter()
        {
            return this;
        }

        internal virtual UseSiteInfo<AssemblySymbol> GetConstraintsUseSiteErrorInfo()
        {
            return default(UseSiteInfo<AssemblySymbol>);
        }

        internal ImmutableArray<TypeWithAnnotations> ConstraintTypesWithDefinitionUseSiteDiagnostics(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ImmutableArray<TypeWithAnnotations> constraintTypesNoUseSiteDiagnostics = ConstraintTypesNoUseSiteDiagnostics;
            AppendConstraintsUseSiteErrorInfo(ref useSiteInfo);
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = constraintTypesNoUseSiteDiagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Type.OriginalDefinition.AddUseSiteInfo(ref useSiteInfo);
            }
            return constraintTypesNoUseSiteDiagnostics;
        }

        private void AppendConstraintsUseSiteErrorInfo(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            useSiteInfo.Add(GetConstraintsUseSiteErrorInfo());
        }

        public sealed override ImmutableArray<Symbol> GetMembers()
        {
            return ImmutableArray<Symbol>.Empty;
        }

        public sealed override ImmutableArray<Symbol> GetMembers(string name)
        {
            return ImmutableArray<Symbol>.Empty;
        }

        public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitTypeParameter(this, argument);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitTypeParameter(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitTypeParameter(this);
        }

        internal TypeParameterSymbol()
        {
        }

        internal sealed override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved = null)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        protected override ImmutableArray<NamedTypeSymbol> GetAllInterfaces()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal NamedTypeSymbol EffectiveBaseClass(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            AppendConstraintsUseSiteErrorInfo(ref useSiteInfo);
            NamedTypeSymbol effectiveBaseClassNoUseSiteDiagnostics = EffectiveBaseClassNoUseSiteDiagnostics;
            effectiveBaseClassNoUseSiteDiagnostics?.OriginalDefinition.AddUseSiteInfo(ref useSiteInfo);
            return effectiveBaseClassNoUseSiteDiagnostics;
        }

        internal TypeSymbol DeducedBaseType(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            AppendConstraintsUseSiteErrorInfo(ref useSiteInfo);
            TypeSymbol deducedBaseTypeNoUseSiteDiagnostics = DeducedBaseTypeNoUseSiteDiagnostics;
            deducedBaseTypeNoUseSiteDiagnostics?.OriginalDefinition.AddUseSiteInfo(ref useSiteInfo);
            return deducedBaseTypeNoUseSiteDiagnostics;
        }

        internal ImmutableArray<NamedTypeSymbol> AllEffectiveInterfacesWithDefinitionUseSiteDiagnostics(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ImmutableArray<NamedTypeSymbol> allEffectiveInterfacesNoUseSiteDiagnostics = AllEffectiveInterfacesNoUseSiteDiagnostics;
            TypeSymbol typeSymbol = DeducedBaseType(ref useSiteInfo);
            while ((object)typeSymbol != null)
            {
                typeSymbol = typeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = allEffectiveInterfacesNoUseSiteDiagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.OriginalDefinition.AddUseSiteInfo(ref useSiteInfo);
            }
            return allEffectiveInterfacesNoUseSiteDiagnostics;
        }

        internal abstract void EnsureAllConstraintsAreResolved();

        protected static void EnsureAllConstraintsAreResolved(ImmutableArray<TypeParameterSymbol> typeParameters)
        {
            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = typeParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.GetConstraintTypes(ConsList<TypeParameterSymbol>.Empty);
            }
        }

        internal abstract ImmutableArray<TypeWithAnnotations> GetConstraintTypes(ConsList<TypeParameterSymbol> inProgress);

        internal abstract ImmutableArray<NamedTypeSymbol> GetInterfaces(ConsList<TypeParameterSymbol> inProgress);

        internal abstract NamedTypeSymbol GetEffectiveBaseClass(ConsList<TypeParameterSymbol> inProgress);

        internal abstract TypeSymbol GetDeducedBaseType(ConsList<TypeParameterSymbol> inProgress);

        private static bool ConstraintImpliesReferenceType(TypeSymbol constraint)
        {
            if (constraint.TypeKind == TypeKind.TypeParameter)
            {
                return ((TypeParameterSymbol)constraint).IsReferenceTypeFromConstraintTypes;
            }
            return NonTypeParameterConstraintImpliesReferenceType(constraint);
        }

        internal static bool NonTypeParameterConstraintImpliesReferenceType(TypeSymbol constraint)
        {
            if (!constraint.IsReferenceType)
            {
                return false;
            }
            switch (constraint.TypeKind)
            {
                case TypeKind.Interface:
                    return false;
                case TypeKind.Error:
                    return false;
                default:
                    {
                        SpecialType specialType = constraint.SpecialType;
                        if ((uint)(specialType - 1) <= 1u || specialType == SpecialType.System_ValueType)
                        {
                            return false;
                        }
                        return true;
                    }
            }
        }

        internal static bool CalculateIsReferenceTypeFromConstraintTypes(ImmutableArray<TypeWithAnnotations> constraintTypes)
        {
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = constraintTypes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (ConstraintImpliesReferenceType(enumerator.Current.Type))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool? IsNotNullableFromConstraintTypes(ImmutableArray<TypeWithAnnotations> constraintTypes)
        {
            bool? result = false;
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = constraintTypes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                bool? flag = IsNotNullableFromConstraintType(enumerator.Current, out bool isNonNullableValueType);
                bool? flag2 = flag;
                isNonNullableValueType = true;
                if (flag2 == isNonNullableValueType)
                {
                    return true;
                }
                if (!flag.HasValue)
                {
                    result = null;
                }
            }
            return result;
        }

        internal static bool? IsNotNullableFromConstraintType(TypeWithAnnotations constraintType, out bool isNonNullableValueType)
        {
            if (constraintType.Type.IsNonNullableValueType())
            {
                isNonNullableValueType = true;
                return true;
            }
            isNonNullableValueType = false;
            if (constraintType.NullableAnnotation.IsAnnotated())
            {
                return false;
            }
            if (constraintType.TypeKind == TypeKind.TypeParameter)
            {
                bool? isNotNullable = ((TypeParameterSymbol)constraintType.Type).IsNotNullable;
                if (isNotNullable == false)
                {
                    return false;
                }
                if (!isNotNullable.HasValue)
                {
                    return null;
                }
            }
            if (constraintType.NullableAnnotation.IsOblivious())
            {
                return null;
            }
            return true;
        }

        internal static bool CalculateIsValueTypeFromConstraintTypes(ImmutableArray<TypeWithAnnotations> constraintTypes)
        {
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = constraintTypes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Type.IsValueType)
                {
                    return true;
                }
            }
            return false;
        }

        protected bool? CalculateIsNotNullableFromNonTypeConstraints()
        {
            if (HasNotNullConstraint || HasValueTypeConstraint)
            {
                return true;
            }
            if (HasReferenceTypeConstraint)
            {
                return !ReferenceTypeConstraintIsNullable;
            }
            return false;
        }

        protected bool? CalculateIsNotNullable()
        {
            bool? flag = CalculateIsNotNullableFromNonTypeConstraints();
            if (flag == true)
            {
                return flag;
            }
            ImmutableArray<TypeWithAnnotations> constraintTypesNoUseSiteDiagnostics = ConstraintTypesNoUseSiteDiagnostics;
            if (constraintTypesNoUseSiteDiagnostics.IsEmpty)
            {
                return flag;
            }
            bool? flag2 = IsNotNullableFromConstraintTypes(constraintTypesNoUseSiteDiagnostics);
            if (flag2 == true || flag == false)
            {
                return flag2;
            }
            return null;
        }

        internal sealed override ManagedKind GetManagedKind(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!HasUnmanagedTypeConstraint)
            {
                return ManagedKind.Managed;
            }
            return ManagedKind.Unmanaged;
        }

        internal sealed override bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            return false;
        }

        internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
        {
            return Equals(t2 as TypeParameterSymbol, comparison);
        }

        internal bool Equals(TypeParameterSymbol other)
        {
            return Equals(other, TypeCompareKind.ConsiderEverything);
        }

        private bool Equals(TypeParameterSymbol other, TypeCompareKind comparison)
        {
            if ((object)this == other)
            {
                return true;
            }
            if ((object)other == null || (object)other.OriginalDefinition != OriginalDefinition)
            {
                return false;
            }
            return other.ContainingSymbol.ContainingType.Equals(ContainingSymbol.ContainingType, comparison);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(ContainingSymbol, Ordinal);
        }

        internal override void AddNullableTransforms(ArrayBuilder<byte> transforms)
        {
        }

        internal override bool ApplyNullableTransforms(byte defaultTransformFlag, ImmutableArray<byte> transforms, ref int position, out TypeSymbol result)
        {
            result = this;
            return true;
        }

        internal override TypeSymbol SetNullabilityForReferenceTypes(Func<TypeWithAnnotations, TypeWithAnnotations> transform)
        {
            return this;
        }

        internal override TypeSymbol MergeEquivalentTypes(TypeSymbol other, VarianceKind variance)
        {
            return this;
        }

        protected sealed override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.TypeParameterSymbol(this, base.DefaultNullableAnnotation);
        }

        protected sealed override ITypeSymbol CreateITypeSymbol(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.TypeParameterSymbol(this, nullableAnnotation);
        }
    }
}
