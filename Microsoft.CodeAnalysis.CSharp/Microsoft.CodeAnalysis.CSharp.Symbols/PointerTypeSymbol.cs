using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class PointerTypeSymbol : TypeSymbol, IPointerTypeReference, ITypeReference, IReference
    {
        private readonly TypeWithAnnotations _pointedAtType;

        bool ITypeReference.IsEnum => false;

        bool ITypeReference.IsValueType => false;

        Microsoft.Cci.PrimitiveTypeCode ITypeReference.TypeCode => Microsoft.Cci.PrimitiveTypeCode.Pointer;

        TypeDefinitionHandle ITypeReference.TypeDef => default(TypeDefinitionHandle);

        IGenericMethodParameterReference ITypeReference.AsGenericMethodParameterReference => null;

        IGenericTypeInstanceReference ITypeReference.AsGenericTypeInstanceReference => null;

        IGenericTypeParameterReference ITypeReference.AsGenericTypeParameterReference => null;

        INamespaceTypeReference ITypeReference.AsNamespaceTypeReference => null;

        INestedTypeReference ITypeReference.AsNestedTypeReference => null;

        ISpecializedNestedTypeReference ITypeReference.AsSpecializedNestedTypeReference => null;

        internal PointerTypeSymbol AdaptedPointerTypeSymbol => this;

        public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public override bool IsStatic => false;

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        public TypeWithAnnotations PointedAtTypeWithAnnotations => _pointedAtType;

        public TypeSymbol PointedAtType => PointedAtTypeWithAnnotations.Type;

        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => null;

        public override bool IsReferenceType => false;

        public override bool IsValueType => true;

        public sealed override bool IsRefLikeType => false;

        public sealed override bool IsReadOnly => false;

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

        public override SymbolKind Kind => SymbolKind.PointerType;

        public override TypeKind TypeKind => TypeKind.Pointer;

        public override Symbol ContainingSymbol => null;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        internal override bool IsRecord => false;

        internal override bool IsRecordStruct => false;

        ITypeReference IPointerTypeReference.GetTargetType(EmitContext context)
        {
            ITypeReference typeReference = ((PEModuleBuilder)context.Module).Translate(AdaptedPointerTypeSymbol.PointedAtType, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
            if (AdaptedPointerTypeSymbol.PointedAtTypeWithAnnotations.CustomModifiers.Length == 0)
            {
                return typeReference;
            }
            return new ModifiedTypeReference(typeReference, ImmutableArray<ICustomModifier>.CastUp(AdaptedPointerTypeSymbol.PointedAtTypeWithAnnotations.CustomModifiers));
        }

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
            visitor.Visit(this);
        }

        IDefinition IReference.AsDefinition(EmitContext context)
        {
            return null;
        }

        internal new PointerTypeSymbol GetCciAdapter()
        {
            return this;
        }

        internal PointerTypeSymbol(TypeWithAnnotations pointedAtType)
        {
            _pointedAtType = pointedAtType;
        }

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal sealed override ManagedKind GetManagedKind(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return ManagedKind.Unmanaged;
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            return ImmutableArray<Symbol>.Empty;
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            return ImmutableArray<Symbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitPointerType(this, argument);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitPointerType(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitPointerType(this);
        }

        public override int GetHashCode()
        {
            int num = 0;
            TypeSymbol typeSymbol = this;
            while (typeSymbol.TypeKind == TypeKind.Pointer)
            {
                num++;
                typeSymbol = ((PointerTypeSymbol)typeSymbol).PointedAtType;
            }
            return Hash.Combine(typeSymbol, num);
        }

        internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
        {
            return Equals(t2 as PointerTypeSymbol, comparison);
        }

        private bool Equals(PointerTypeSymbol other, TypeCompareKind comparison)
        {
            if ((object)this == other)
            {
                return true;
            }
            if ((object)other == null || !other._pointedAtType.Equals(_pointedAtType, comparison))
            {
                return false;
            }
            return true;
        }

        internal override void AddNullableTransforms(ArrayBuilder<byte> transforms)
        {
            PointedAtTypeWithAnnotations.AddNullableTransforms(transforms);
        }

        internal override bool ApplyNullableTransforms(byte defaultTransformFlag, ImmutableArray<byte> transforms, ref int position, out TypeSymbol result)
        {
            if (!PointedAtTypeWithAnnotations.ApplyNullableTransforms(defaultTransformFlag, transforms, ref position, out var result2))
            {
                result = this;
                return false;
            }
            result = WithPointedAtType(result2);
            return true;
        }

        internal override TypeSymbol SetNullabilityForReferenceTypes(Func<TypeWithAnnotations, TypeWithAnnotations> transform)
        {
            return WithPointedAtType(transform(PointedAtTypeWithAnnotations));
        }

        internal override TypeSymbol MergeEquivalentTypes(TypeSymbol other, VarianceKind variance)
        {
            TypeWithAnnotations newPointedAtType = PointedAtTypeWithAnnotations.MergeEquivalentTypes(((PointerTypeSymbol)other).PointedAtTypeWithAnnotations, VarianceKind.None);
            return WithPointedAtType(newPointedAtType);
        }

        internal PointerTypeSymbol WithPointedAtType(TypeWithAnnotations newPointedAtType)
        {
            if (!PointedAtTypeWithAnnotations.IsSameAs(newPointedAtType))
            {
                return new PointerTypeSymbol(newPointedAtType);
            }
            return this;
        }

        public override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            UseSiteInfo<AssemblySymbol> result = default(UseSiteInfo<AssemblySymbol>);
            DeriveUseSiteInfoFromType(ref result, PointedAtTypeWithAnnotations, AllowedRequiredModifierType.None);
            return result;
        }

        internal override bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            return PointedAtTypeWithAnnotations.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes);
        }

        protected override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PointerTypeSymbol(this, base.DefaultNullableAnnotation);
        }

        protected override ITypeSymbol CreateITypeSymbol(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PointerTypeSymbol(this, nullableAnnotation);
        }
    }
}
