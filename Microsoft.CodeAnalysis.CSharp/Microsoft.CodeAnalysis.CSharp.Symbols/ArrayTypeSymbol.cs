using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class ArrayTypeSymbol : TypeSymbol, IArrayTypeReference, ITypeReference, IReference
    {
        private sealed class SZArray : ArrayTypeSymbol
        {
            private readonly ImmutableArray<NamedTypeSymbol> _interfaces;

            public override int Rank => 1;

            public override bool IsSZArray => true;

            internal override bool HasDefaultSizesAndLowerBounds => true;

            internal SZArray(TypeWithAnnotations elementTypeWithAnnotations, NamedTypeSymbol array, ImmutableArray<NamedTypeSymbol> constructedInterfaces)
                : base(elementTypeWithAnnotations, array)
            {
                _interfaces = constructedInterfaces;
            }

            protected override ArrayTypeSymbol WithElementTypeCore(TypeWithAnnotations newElementType)
            {
                ImmutableArray<NamedTypeSymbol> constructedInterfaces = _interfaces.SelectAsArray((NamedTypeSymbol i, TypeSymbol t) => i.OriginalDefinition.Construct(t), newElementType.Type);
                return new SZArray(newElementType, BaseTypeNoUseSiteDiagnostics, constructedInterfaces);
            }

            internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol>? basesBeingResolved = null)
            {
                return _interfaces;
            }
        }

        private abstract class MDArray : ArrayTypeSymbol
        {
            private readonly int _rank;

            public sealed override int Rank => _rank;

            public sealed override bool IsSZArray => false;

            internal MDArray(TypeWithAnnotations elementTypeWithAnnotations, int rank, NamedTypeSymbol array)
                : base(elementTypeWithAnnotations, array)
            {
                _rank = rank;
            }

            internal sealed override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol>? basesBeingResolved = null)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }
        }

        private sealed class MDArrayNoSizesOrBounds : MDArray
        {
            internal override bool HasDefaultSizesAndLowerBounds => true;

            internal MDArrayNoSizesOrBounds(TypeWithAnnotations elementTypeWithAnnotations, int rank, NamedTypeSymbol array)
                : base(elementTypeWithAnnotations, rank, array)
            {
            }

            protected override ArrayTypeSymbol WithElementTypeCore(TypeWithAnnotations elementTypeWithAnnotations)
            {
                return new MDArrayNoSizesOrBounds(elementTypeWithAnnotations, Rank, BaseTypeNoUseSiteDiagnostics);
            }
        }

        private sealed class MDArrayWithSizesAndBounds : MDArray
        {
            private readonly ImmutableArray<int> _sizes;

            private readonly ImmutableArray<int> _lowerBounds;

            public override ImmutableArray<int> Sizes => _sizes;

            public override ImmutableArray<int> LowerBounds => _lowerBounds;

            internal override bool HasDefaultSizesAndLowerBounds => false;

            internal MDArrayWithSizesAndBounds(TypeWithAnnotations elementTypeWithAnnotations, int rank, ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds, NamedTypeSymbol array)
                : base(elementTypeWithAnnotations, rank, array)
            {
                _sizes = sizes.NullToEmpty();
                _lowerBounds = lowerBounds;
            }

            protected override ArrayTypeSymbol WithElementTypeCore(TypeWithAnnotations elementTypeWithAnnotations)
            {
                return new MDArrayWithSizesAndBounds(elementTypeWithAnnotations, Rank, _sizes, _lowerBounds, BaseTypeNoUseSiteDiagnostics);
            }
        }

        private readonly TypeWithAnnotations _elementTypeWithAnnotations;

        private readonly NamedTypeSymbol _baseType;

        bool IArrayTypeReference.IsSZArray => AdaptedArrayTypeSymbol.IsSZArray;

        ImmutableArray<int> IArrayTypeReference.LowerBounds => AdaptedArrayTypeSymbol.LowerBounds;

        int IArrayTypeReference.Rank => AdaptedArrayTypeSymbol.Rank;

        ImmutableArray<int> IArrayTypeReference.Sizes => AdaptedArrayTypeSymbol.Sizes;

        bool ITypeReference.IsEnum => false;

        bool ITypeReference.IsValueType => false;

        TypeDefinitionHandle ITypeReference.TypeDef => default(TypeDefinitionHandle);

        Microsoft.Cci.PrimitiveTypeCode ITypeReference.TypeCode => Microsoft.Cci.PrimitiveTypeCode.NotPrimitive;

        IGenericMethodParameterReference? ITypeReference.AsGenericMethodParameterReference => null;

        IGenericTypeInstanceReference? ITypeReference.AsGenericTypeInstanceReference => null;

        IGenericTypeParameterReference? ITypeReference.AsGenericTypeParameterReference => null;

        INamespaceTypeReference? ITypeReference.AsNamespaceTypeReference => null;

        INestedTypeReference? ITypeReference.AsNestedTypeReference => null;

        ISpecializedNestedTypeReference? ITypeReference.AsSpecializedNestedTypeReference => null;

        internal ArrayTypeSymbol AdaptedArrayTypeSymbol => this;

        public abstract int Rank { get; }

        public abstract bool IsSZArray { get; }

        public virtual ImmutableArray<int> Sizes => ImmutableArray<int>.Empty;

        public virtual ImmutableArray<int> LowerBounds => default(ImmutableArray<int>);

        internal abstract bool HasDefaultSizesAndLowerBounds { get; }

        public TypeWithAnnotations ElementTypeWithAnnotations => _elementTypeWithAnnotations;

        public TypeSymbol ElementType => _elementTypeWithAnnotations.Type;

        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => _baseType;

        public override bool IsReferenceType => true;

        public override bool IsValueType => false;

        public sealed override bool IsRefLikeType => false;

        public sealed override bool IsReadOnly => false;

        internal sealed override ObsoleteAttributeData? ObsoleteAttributeData => null;

        public override SymbolKind Kind => SymbolKind.ArrayType;

        public override TypeKind TypeKind => TypeKind.Array;

        public override Symbol? ContainingSymbol => null;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public override bool IsStatic => false;

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        internal override bool IsRecord => false;

        internal override bool IsRecordStruct => false;

        ITypeReference IArrayTypeReference.GetElementType(EmitContext context)
        {
            PEModuleBuilder obj = (PEModuleBuilder)context.Module;
            TypeWithAnnotations elementTypeWithAnnotations = AdaptedArrayTypeSymbol.ElementTypeWithAnnotations;
            ITypeReference typeReference = obj.Translate(elementTypeWithAnnotations.Type, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
            if (elementTypeWithAnnotations.CustomModifiers.Length == 0)
            {
                return typeReference;
            }
            return new ModifiedTypeReference(typeReference, ImmutableArray<ICustomModifier>.CastUp(elementTypeWithAnnotations.CustomModifiers));
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        ITypeDefinition? ITypeReference.GetResolvedType(EmitContext context)
        {
            return null;
        }

        INamespaceTypeDefinition? ITypeReference.AsNamespaceTypeDefinition(EmitContext context)
        {
            return null;
        }

        INestedTypeDefinition? ITypeReference.AsNestedTypeDefinition(EmitContext context)
        {
            return null;
        }

        ITypeDefinition? ITypeReference.AsTypeDefinition(EmitContext context)
        {
            return null;
        }

        IDefinition? IReference.AsDefinition(EmitContext context)
        {
            return null;
        }

        internal new ArrayTypeSymbol GetCciAdapter()
        {
            return this;
        }

        private ArrayTypeSymbol(TypeWithAnnotations elementTypeWithAnnotations, NamedTypeSymbol array)
        {
            _elementTypeWithAnnotations = elementTypeWithAnnotations;
            _baseType = array;
        }

        internal static ArrayTypeSymbol CreateCSharpArray(AssemblySymbol declaringAssembly, TypeWithAnnotations elementTypeWithAnnotations, int rank = 1)
        {
            if (rank == 1)
            {
                return CreateSZArray(declaringAssembly, elementTypeWithAnnotations);
            }
            return CreateMDArray(declaringAssembly, elementTypeWithAnnotations, rank, default(ImmutableArray<int>), default(ImmutableArray<int>));
        }

        internal static ArrayTypeSymbol CreateMDArray(TypeWithAnnotations elementTypeWithAnnotations, int rank, ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds, NamedTypeSymbol array)
        {
            if (sizes.IsDefaultOrEmpty && lowerBounds.IsDefault)
            {
                return new MDArrayNoSizesOrBounds(elementTypeWithAnnotations, rank, array);
            }
            return new MDArrayWithSizesAndBounds(elementTypeWithAnnotations, rank, sizes, lowerBounds, array);
        }

        internal static ArrayTypeSymbol CreateMDArray(AssemblySymbol declaringAssembly, TypeWithAnnotations elementType, int rank, ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds)
        {
            return CreateMDArray(elementType, rank, sizes, lowerBounds, declaringAssembly.GetSpecialType(SpecialType.System_Array));
        }

        internal static ArrayTypeSymbol CreateSZArray(TypeWithAnnotations elementTypeWithAnnotations, NamedTypeSymbol array)
        {
            return new SZArray(elementTypeWithAnnotations, array, GetSZArrayInterfaces(elementTypeWithAnnotations, array.ContainingAssembly));
        }

        internal static ArrayTypeSymbol CreateSZArray(TypeWithAnnotations elementTypeWithAnnotations, NamedTypeSymbol array, ImmutableArray<NamedTypeSymbol> constructedInterfaces)
        {
            return new SZArray(elementTypeWithAnnotations, array, constructedInterfaces);
        }

        internal static ArrayTypeSymbol CreateSZArray(AssemblySymbol declaringAssembly, TypeWithAnnotations elementType)
        {
            return CreateSZArray(elementType, declaringAssembly.GetSpecialType(SpecialType.System_Array), GetSZArrayInterfaces(elementType, declaringAssembly));
        }

        internal ArrayTypeSymbol WithElementType(TypeWithAnnotations elementTypeWithAnnotations)
        {
            if (!ElementTypeWithAnnotations.IsSameAs(elementTypeWithAnnotations))
            {
                return WithElementTypeCore(elementTypeWithAnnotations);
            }
            return this;
        }

        protected abstract ArrayTypeSymbol WithElementTypeCore(TypeWithAnnotations elementTypeWithAnnotations);

        private static ImmutableArray<NamedTypeSymbol> GetSZArrayInterfaces(TypeWithAnnotations elementTypeWithAnnotations, AssemblySymbol declaringAssembly)
        {
            ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            NamedTypeSymbol specialType = declaringAssembly.GetSpecialType(SpecialType.System_Collections_Generic_IList_T);
            if (!specialType.IsErrorType())
            {
                instance.Add(new ConstructedNamedTypeSymbol(specialType, ImmutableArray.Create(elementTypeWithAnnotations)));
            }
            NamedTypeSymbol specialType2 = declaringAssembly.GetSpecialType(SpecialType.System_Collections_Generic_IReadOnlyList_T);
            if (!specialType2.IsErrorType())
            {
                instance.Add(new ConstructedNamedTypeSymbol(specialType2, ImmutableArray.Create(elementTypeWithAnnotations)));
            }
            return instance.ToImmutableAndFree();
        }

        internal bool HasSameShapeAs(ArrayTypeSymbol other)
        {
            if (Rank == other.Rank)
            {
                return IsSZArray == other.IsSZArray;
            }
            return false;
        }

        internal bool HasSameSizesAndLowerBoundsAs(ArrayTypeSymbol other)
        {
            if (Sizes.SequenceEqual(other.Sizes))
            {
                ImmutableArray<int> lowerBounds = LowerBounds;
                if (lowerBounds.IsDefault)
                {
                    return other.LowerBounds.IsDefault;
                }
                ImmutableArray<int> lowerBounds2 = other.LowerBounds;
                if (!lowerBounds2.IsDefault)
                {
                    return lowerBounds.SequenceEqual(lowerBounds2);
                }
                return false;
            }
            return false;
        }

        internal sealed override ManagedKind GetManagedKind(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return ManagedKind.Managed;
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
            return visitor.VisitArrayType(this, argument);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitArrayType(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitArrayType(this);
        }

        internal override bool Equals(TypeSymbol? t2, TypeCompareKind comparison)
        {
            return Equals(t2 as ArrayTypeSymbol, comparison);
        }

        private bool Equals(ArrayTypeSymbol? other, TypeCompareKind comparison)
        {
            if ((object)this == other)
            {
                return true;
            }
            if ((object)other == null || !other!.HasSameShapeAs(this) || !other!.ElementTypeWithAnnotations.Equals(ElementTypeWithAnnotations, comparison))
            {
                return false;
            }
            if ((comparison & TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) == 0 && !HasSameSizesAndLowerBoundsAs(other))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int currentKey = 0;
            TypeSymbol typeSymbol = this;
            while (typeSymbol.TypeKind == TypeKind.Array)
            {
                ArrayTypeSymbol obj = (ArrayTypeSymbol)typeSymbol;
                currentKey = Hash.Combine(obj.Rank, currentKey);
                typeSymbol = obj.ElementType;
            }
            return Hash.Combine(typeSymbol, currentKey);
        }

        internal override void AddNullableTransforms(ArrayBuilder<byte> transforms)
        {
            ElementTypeWithAnnotations.AddNullableTransforms(transforms);
        }

        internal override bool ApplyNullableTransforms(byte defaultTransformFlag, ImmutableArray<byte> transforms, ref int position, out TypeSymbol result)
        {
            if (!ElementTypeWithAnnotations.ApplyNullableTransforms(defaultTransformFlag, transforms, ref position, out var result2))
            {
                result = this;
                return false;
            }
            result = WithElementType(result2);
            return true;
        }

        internal override TypeSymbol SetNullabilityForReferenceTypes(Func<TypeWithAnnotations, TypeWithAnnotations> transform)
        {
            return WithElementType(transform(ElementTypeWithAnnotations));
        }

        internal override TypeSymbol MergeEquivalentTypes(TypeSymbol other, VarianceKind variance)
        {
            TypeWithAnnotations elementTypeWithAnnotations = ElementTypeWithAnnotations.MergeEquivalentTypes(((ArrayTypeSymbol)other).ElementTypeWithAnnotations, variance);
            return WithElementType(elementTypeWithAnnotations);
        }

        public override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            UseSiteInfo<AssemblySymbol> result = default(UseSiteInfo<AssemblySymbol>);
            DeriveUseSiteInfoFromType(ref result, ElementTypeWithAnnotations, AllowedRequiredModifierType.None);
            return result;
        }

        internal override bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            if (!_elementTypeWithAnnotations.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes) && ((object)_baseType == null || !_baseType.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes)))
            {
                return Symbol.GetUnificationUseSiteDiagnosticRecursive(ref result, InterfacesNoUseSiteDiagnostics(), owner, ref checkedTypes);
            }
            return true;
        }

        protected sealed override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.ArrayTypeSymbol(this, base.DefaultNullableAnnotation);
        }

        protected sealed override ITypeSymbol CreateITypeSymbol(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.ArrayTypeSymbol(this, nullableAnnotation);
        }
    }
}
