using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSwitchLabel : BoundNode
    {
        public LabelSymbol Label { get; }

        public BoundPattern Pattern { get; }

        public BoundExpression? WhenClause { get; }

        public BoundSwitchLabel(SyntaxNode syntax, LabelSymbol label, BoundPattern pattern, BoundExpression? whenClause, bool hasErrors = false)
            : base(BoundKind.SwitchLabel, syntax, hasErrors || pattern.HasErrors() || whenClause.HasErrors())
        {
            Label = label;
            Pattern = pattern;
            WhenClause = whenClause;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSwitchLabel(this);
        }

        public BoundSwitchLabel Update(LabelSymbol label, BoundPattern pattern, BoundExpression? whenClause)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, Label) || pattern != Pattern || whenClause != WhenClause)
            {
                BoundSwitchLabel boundSwitchLabel = new BoundSwitchLabel(Syntax, label, pattern, whenClause, base.HasErrors);
                boundSwitchLabel.CopyAttributes(this);
                return boundSwitchLabel;
            }
            return this;
        }
    }
}
