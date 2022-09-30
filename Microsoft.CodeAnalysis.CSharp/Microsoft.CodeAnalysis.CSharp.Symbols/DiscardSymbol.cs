using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class DiscardSymbol : Symbol
    {
        public TypeWithAnnotations TypeWithAnnotations { get; }

        public override Symbol? ContainingSymbol => null;

        public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override bool IsAbstract => false;

        public override bool IsExtern => false;

        public override bool IsImplicitlyDeclared => true;

        public override bool IsOverride => false;

        public override bool IsSealed => false;

        public override bool IsStatic => false;

        public override bool IsVirtual => false;

        public override SymbolKind Kind => SymbolKind.Discard;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        internal override ObsoleteAttributeData? ObsoleteAttributeData => null;

        public DiscardSymbol(TypeWithAnnotations typeWithAnnotations)
        {
            TypeWithAnnotations = typeWithAnnotations;
        }

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument a)
        {
            return visitor.VisitDiscard(this, a);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitDiscard(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitDiscard(this);
        }

        public override bool Equals(Symbol? obj, TypeCompareKind compareKind)
        {
            if (obj is DiscardSymbol discardSymbol)
            {
                return TypeWithAnnotations.Equals(discardSymbol.TypeWithAnnotations, compareKind);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return TypeWithAnnotations.GetHashCode();
        }

        protected override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.DiscardSymbol(this);
        }
    }
}
