using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSwitchExpressionArm : BoundNode
    {
        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundPattern Pattern { get; }

        public BoundExpression? WhenClause { get; }

        public BoundExpression Value { get; }

        public LabelSymbol Label { get; }

        public BoundSwitchExpressionArm(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundPattern pattern, BoundExpression? whenClause, BoundExpression value, LabelSymbol label, bool hasErrors = false)
            : base(BoundKind.SwitchExpressionArm, syntax, hasErrors || pattern.HasErrors() || whenClause.HasErrors() || value.HasErrors())
        {
            Locals = locals;
            Pattern = pattern;
            WhenClause = whenClause;
            Value = value;
            Label = label;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSwitchExpressionArm(this);
        }

        public BoundSwitchExpressionArm Update(ImmutableArray<LocalSymbol> locals, BoundPattern pattern, BoundExpression? whenClause, BoundExpression value, LabelSymbol label)
        {
            if (locals != Locals || pattern != Pattern || whenClause != WhenClause || value != Value || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, Label))
            {
                BoundSwitchExpressionArm boundSwitchExpressionArm = new BoundSwitchExpressionArm(Syntax, locals, pattern, whenClause, value, label, base.HasErrors);
                boundSwitchExpressionArm.CopyAttributes(this);
                return boundSwitchExpressionArm;
            }
            return this;
        }
    }
}
