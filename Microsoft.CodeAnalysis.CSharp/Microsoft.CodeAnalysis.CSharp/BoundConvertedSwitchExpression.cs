using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundConvertedSwitchExpression : BoundSwitchExpression
    {
        public new TypeSymbol Type => base.Type;

        public TypeSymbol? NaturalTypeOpt { get; }

        public bool WasTargetTyped { get; }

        public Conversion Conversion { get; }

        public BoundConvertedSwitchExpression(SyntaxNode syntax, TypeSymbol? naturalTypeOpt, bool wasTargetTyped, Conversion conversion, BoundExpression expression, ImmutableArray<BoundSwitchExpressionArm> switchArms, BoundDecisionDag decisionDag, LabelSymbol? defaultLabel, bool reportedNotExhaustive, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ConvertedSwitchExpression, syntax, expression, switchArms, decisionDag, defaultLabel, reportedNotExhaustive, type, hasErrors || expression.HasErrors() || switchArms.HasErrors() || decisionDag.HasErrors())
        {
            NaturalTypeOpt = naturalTypeOpt;
            WasTargetTyped = wasTargetTyped;
            Conversion = conversion;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitConvertedSwitchExpression(this);
        }

        public BoundConvertedSwitchExpression Update(TypeSymbol? naturalTypeOpt, bool wasTargetTyped, Conversion conversion, BoundExpression expression, ImmutableArray<BoundSwitchExpressionArm> switchArms, BoundDecisionDag decisionDag, LabelSymbol? defaultLabel, bool reportedNotExhaustive, TypeSymbol type)
        {
            if (!TypeSymbol.Equals(naturalTypeOpt, NaturalTypeOpt, TypeCompareKind.ConsiderEverything) || wasTargetTyped != WasTargetTyped || conversion != Conversion || expression != base.Expression || switchArms != base.SwitchArms || decisionDag != base.DecisionDag || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(defaultLabel, base.DefaultLabel) || reportedNotExhaustive != base.ReportedNotExhaustive || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundConvertedSwitchExpression boundConvertedSwitchExpression = new BoundConvertedSwitchExpression(Syntax, naturalTypeOpt, wasTargetTyped, conversion, expression, switchArms, decisionDag, defaultLabel, reportedNotExhaustive, type, base.HasErrors);
                boundConvertedSwitchExpression.CopyAttributes(this);
                return boundConvertedSwitchExpression;
            }
            return this;
        }
    }
}
