using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundIsPatternExpression : BoundExpression
    {
        public BoundExpression Expression { get; }

        public BoundPattern Pattern { get; }

        public bool IsNegated { get; }

        public BoundDecisionDag DecisionDag { get; }

        public LabelSymbol WhenTrueLabel { get; }

        public LabelSymbol WhenFalseLabel { get; }

        public BoundIsPatternExpression(SyntaxNode syntax, BoundExpression expression, BoundPattern pattern, bool isNegated, BoundDecisionDag decisionDag, LabelSymbol whenTrueLabel, LabelSymbol whenFalseLabel, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.IsPatternExpression, syntax, type, hasErrors || expression.HasErrors() || pattern.HasErrors() || decisionDag.HasErrors())
        {
            Expression = expression;
            Pattern = pattern;
            IsNegated = isNegated;
            DecisionDag = decisionDag;
            WhenTrueLabel = whenTrueLabel;
            WhenFalseLabel = whenFalseLabel;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitIsPatternExpression(this);
        }

        public BoundIsPatternExpression Update(BoundExpression expression, BoundPattern pattern, bool isNegated, BoundDecisionDag decisionDag, LabelSymbol whenTrueLabel, LabelSymbol whenFalseLabel, TypeSymbol? type)
        {
            if (expression != Expression || pattern != Pattern || isNegated != IsNegated || decisionDag != DecisionDag || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(whenTrueLabel, WhenTrueLabel) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(whenFalseLabel, WhenFalseLabel) || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundIsPatternExpression boundIsPatternExpression = new BoundIsPatternExpression(Syntax, expression, pattern, isNegated, decisionDag, whenTrueLabel, whenFalseLabel, type, base.HasErrors);
                boundIsPatternExpression.CopyAttributes(this);
                return boundIsPatternExpression;
            }
            return this;
        }
    }
}
