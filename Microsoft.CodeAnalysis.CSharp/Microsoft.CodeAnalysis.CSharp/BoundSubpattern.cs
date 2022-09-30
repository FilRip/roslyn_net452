using System.Diagnostics;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSubpattern : BoundNode
    {
        public Symbol? Symbol { get; }

        public BoundPattern Pattern { get; }

        public BoundSubpattern(SyntaxNode syntax, Symbol? symbol, BoundPattern pattern, bool hasErrors = false)
            : base(BoundKind.Subpattern, syntax, hasErrors || pattern.HasErrors())
        {
            Symbol = symbol;
            Pattern = pattern;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSubpattern(this);
        }

        public BoundSubpattern Update(Symbol? symbol, BoundPattern pattern)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(symbol, Symbol) || pattern != Pattern)
            {
                BoundSubpattern boundSubpattern = new BoundSubpattern(Syntax, symbol, pattern, base.HasErrors);
                boundSubpattern.CopyAttributes(this);
                return boundSubpattern;
            }
            return this;
        }
    }
}
