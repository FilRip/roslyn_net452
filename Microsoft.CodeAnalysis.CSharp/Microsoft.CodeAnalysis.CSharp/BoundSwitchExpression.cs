using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundSwitchExpression : BoundExpression
    {
        public BoundExpression Expression { get; }

        public ImmutableArray<BoundSwitchExpressionArm> SwitchArms { get; }

        public BoundDecisionDag DecisionDag { get; }

        public LabelSymbol? DefaultLabel { get; }

        public bool ReportedNotExhaustive { get; }

        protected BoundSwitchExpression(BoundKind kind, SyntaxNode syntax, BoundExpression expression, ImmutableArray<BoundSwitchExpressionArm> switchArms, BoundDecisionDag decisionDag, LabelSymbol? defaultLabel, bool reportedNotExhaustive, TypeSymbol? type, bool hasErrors = false)
            : base(kind, syntax, type, hasErrors)
        {
            Expression = expression;
            SwitchArms = switchArms;
            DecisionDag = decisionDag;
            DefaultLabel = defaultLabel;
            ReportedNotExhaustive = reportedNotExhaustive;
        }
    }
}
