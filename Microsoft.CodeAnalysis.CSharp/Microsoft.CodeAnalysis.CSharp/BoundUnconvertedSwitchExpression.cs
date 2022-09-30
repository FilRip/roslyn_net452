using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundUnconvertedSwitchExpression : BoundSwitchExpression
    {
        public override object Display
        {
            get
            {
                if ((object)base.Type != null)
                {
                    return base.Display;
                }
                return MessageID.IDS_FeatureSwitchExpression.Localize();
            }
        }

        public BoundUnconvertedSwitchExpression(SyntaxNode syntax, BoundExpression expression, ImmutableArray<BoundSwitchExpressionArm> switchArms, BoundDecisionDag decisionDag, LabelSymbol? defaultLabel, bool reportedNotExhaustive, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.UnconvertedSwitchExpression, syntax, expression, switchArms, decisionDag, defaultLabel, reportedNotExhaustive, type, hasErrors || expression.HasErrors() || switchArms.HasErrors() || decisionDag.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitUnconvertedSwitchExpression(this);
        }

        public BoundUnconvertedSwitchExpression Update(BoundExpression expression, ImmutableArray<BoundSwitchExpressionArm> switchArms, BoundDecisionDag decisionDag, LabelSymbol? defaultLabel, bool reportedNotExhaustive, TypeSymbol? type)
        {
            if (expression != base.Expression || switchArms != base.SwitchArms || decisionDag != base.DecisionDag || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(defaultLabel, base.DefaultLabel) || reportedNotExhaustive != base.ReportedNotExhaustive || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundUnconvertedSwitchExpression boundUnconvertedSwitchExpression = new BoundUnconvertedSwitchExpression(Syntax, expression, switchArms, decisionDag, defaultLabel, reportedNotExhaustive, type, base.HasErrors);
                boundUnconvertedSwitchExpression.CopyAttributes(this);
                return boundUnconvertedSwitchExpression;
            }
            return this;
        }
    }
}
