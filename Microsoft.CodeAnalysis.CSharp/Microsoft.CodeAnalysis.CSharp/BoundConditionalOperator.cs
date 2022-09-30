using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundConditionalOperator : BoundExpression
    {
        public override ConstantValue? ConstantValue => ConstantValueOpt;

        public bool IsDynamic
        {
            get
            {
                if (Condition.Kind == BoundKind.UnaryOperator)
                {
                    return ((BoundUnaryOperator)Condition).OperatorKind.IsDynamic();
                }
                return false;
            }
        }

        public new TypeSymbol Type => base.Type;

        public bool IsRef { get; }

        public BoundExpression Condition { get; }

        public BoundExpression Consequence { get; }

        public BoundExpression Alternative { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public TypeSymbol? NaturalTypeOpt { get; }

        public bool WasTargetTyped { get; }

        public BoundConditionalOperator(SyntaxNode syntax, bool isRef, BoundExpression condition, BoundExpression consequence, BoundExpression alternative, ConstantValue? constantValueOpt, TypeSymbol? naturalTypeOpt, bool wasTargetTyped, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ConditionalOperator, syntax, type, hasErrors || condition.HasErrors() || consequence.HasErrors() || alternative.HasErrors())
        {
            IsRef = isRef;
            Condition = condition;
            Consequence = consequence;
            Alternative = alternative;
            ConstantValueOpt = constantValueOpt;
            NaturalTypeOpt = naturalTypeOpt;
            WasTargetTyped = wasTargetTyped;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitConditionalOperator(this);
        }

        public BoundConditionalOperator Update(bool isRef, BoundExpression condition, BoundExpression consequence, BoundExpression alternative, ConstantValue? constantValueOpt, TypeSymbol? naturalTypeOpt, bool wasTargetTyped, TypeSymbol type)
        {
            if (isRef != IsRef || condition != Condition || consequence != Consequence || alternative != Alternative || constantValueOpt != ConstantValueOpt || !TypeSymbol.Equals(naturalTypeOpt, NaturalTypeOpt, TypeCompareKind.ConsiderEverything) || wasTargetTyped != WasTargetTyped || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundConditionalOperator boundConditionalOperator = new BoundConditionalOperator(Syntax, isRef, condition, consequence, alternative, constantValueOpt, naturalTypeOpt, wasTargetTyped, type, base.HasErrors);
                boundConditionalOperator.CopyAttributes(this);
                return boundConditionalOperator;
            }
            return this;
        }
    }
}
