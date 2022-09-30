using System.Collections.Immutable;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SourceTypeParameterSymbol : SourceTypeParameterSymbolBase
    {
        private readonly SourceNamedTypeSymbol _owner;

        private readonly VarianceKind _varianceKind;

        public override TypeParameterKind TypeParameterKind => TypeParameterKind.Type;

        public override Symbol ContainingSymbol => _owner;

        public override VarianceKind Variance => _varianceKind;

        public override bool HasConstructorConstraint => (GetConstraintKinds() & TypeParameterConstraintKind.Constructor) != 0;

        public override bool HasValueTypeConstraint => (GetConstraintKinds() & TypeParameterConstraintKind.AllValueTypeKinds) != 0;

        public override bool IsValueTypeFromConstraintTypes => (GetConstraintKinds() & TypeParameterConstraintKind.ValueTypeFromConstraintTypes) != 0;

        public override bool HasReferenceTypeConstraint => (GetConstraintKinds() & TypeParameterConstraintKind.ReferenceType) != 0;

        public override bool IsReferenceTypeFromConstraintTypes => (GetConstraintKinds() & TypeParameterConstraintKind.ReferenceTypeFromConstraintTypes) != 0;

        internal override bool? ReferenceTypeConstraintIsNullable => CalculateReferenceTypeConstraintIsNullable(GetConstraintKinds());

        public override bool HasNotNullConstraint => (GetConstraintKinds() & TypeParameterConstraintKind.NotNull) != 0;

        internal override bool? IsNotNullable
        {
            get
            {
                if ((GetConstraintKinds() & TypeParameterConstraintKind.ObliviousNullabilityIfReferenceType) != 0)
                {
                    return null;
                }
                return CalculateIsNotNullable();
            }
        }

        public override bool HasUnmanagedTypeConstraint => (GetConstraintKinds() & TypeParameterConstraintKind.Unmanaged) != 0;

        protected override ImmutableArray<TypeParameterSymbol> ContainerTypeParameters => _owner.TypeParameters;

        public SourceTypeParameterSymbol(SourceNamedTypeSymbol owner, string name, int ordinal, VarianceKind varianceKind, ImmutableArray<Location> locations, ImmutableArray<SyntaxReference> syntaxRefs)
            : base(name, ordinal, locations, syntaxRefs)
        {
            _owner = owner;
            _varianceKind = varianceKind;
        }

        protected override TypeParameterBounds ResolveBounds(ConsList<TypeParameterSymbol> inProgress, BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<TypeWithAnnotations> typeParameterConstraintTypes = _owner.GetTypeParameterConstraintTypes(Ordinal);
            if (typeParameterConstraintTypes.IsEmpty && GetConstraintKinds() == TypeParameterConstraintKind.None)
            {
                return null;
            }
            return this.ResolveBounds(ContainingAssembly.CorLibrary, inProgress.Prepend(this), typeParameterConstraintTypes, inherited: false, DeclaringCompilation, diagnostics);
        }

        private TypeParameterConstraintKind GetConstraintKinds()
        {
            return _owner.GetTypeParameterConstraintKind(Ordinal);
        }
    }
}
