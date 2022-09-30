using System.Collections.Immutable;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public class RangeVariableSymbol : Symbol
    {
        private readonly string _name;

        private readonly ImmutableArray<Location> _locations;

        private readonly Symbol _containingSymbol;

        internal bool IsTransparent { get; }

        public override string Name => _name;

        public override SymbolKind Kind => SymbolKind.RangeVariable;

        public override ImmutableArray<Location> Locations => _locations;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray.Create(((CSharpSyntaxNode)_locations[0].SourceTree!.GetRoot().FindToken(_locations[0].SourceSpan.Start).Parent).GetReference());

        public override bool IsExtern => false;

        public override bool IsSealed => false;

        public override bool IsAbstract => false;

        public override bool IsOverride => false;

        public override bool IsVirtual => false;

        public override bool IsStatic => false;

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

        public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public override Symbol ContainingSymbol => _containingSymbol;

        internal RangeVariableSymbol(string Name, Symbol containingSymbol, Location location, bool isTransparent = false)
        {
            _name = Name;
            _containingSymbol = containingSymbol;
            _locations = ImmutableArray.Create(location);
            IsTransparent = isTransparent;
        }

        internal override TResult Accept<TArg, TResult>(CSharpSymbolVisitor<TArg, TResult> visitor, TArg a)
        {
            return visitor.VisitRangeVariable(this, a);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitRangeVariable(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitRangeVariable(this);
        }

        public override bool Equals(Symbol obj, TypeCompareKind compareKind)
        {
            if ((object)obj == this)
            {
                return true;
            }
            if (obj is RangeVariableSymbol rangeVariableSymbol && rangeVariableSymbol._locations[0].Equals(_locations[0]))
            {
                return _containingSymbol.Equals(rangeVariableSymbol.ContainingSymbol, compareKind);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(_locations[0].GetHashCode(), _containingSymbol.GetHashCode());
        }

        protected override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.RangeVariableSymbol(this);
        }
    }
}
