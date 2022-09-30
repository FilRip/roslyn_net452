using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundNullCoalescingOperator : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression LeftOperand { get; }

        public BoundExpression RightOperand { get; }

        public Conversion LeftConversion { get; }

        public BoundNullCoalescingOperatorResultKind OperatorResultKind { get; }

        public BoundNullCoalescingOperator(SyntaxNode syntax, BoundExpression leftOperand, BoundExpression rightOperand, Conversion leftConversion, BoundNullCoalescingOperatorResultKind operatorResultKind, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.NullCoalescingOperator, syntax, type, hasErrors || leftOperand.HasErrors() || rightOperand.HasErrors())
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            LeftConversion = leftConversion;
            OperatorResultKind = operatorResultKind;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitNullCoalescingOperator(this);
        }

        public BoundNullCoalescingOperator Update(BoundExpression leftOperand, BoundExpression rightOperand, Conversion leftConversion, BoundNullCoalescingOperatorResultKind operatorResultKind, TypeSymbol type)
        {
            if (leftOperand != LeftOperand || rightOperand != RightOperand || leftConversion != LeftConversion || operatorResultKind != OperatorResultKind || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundNullCoalescingOperator boundNullCoalescingOperator = new BoundNullCoalescingOperator(Syntax, leftOperand, rightOperand, leftConversion, operatorResultKind, type, base.HasErrors);
                boundNullCoalescingOperator.CopyAttributes(this);
                return boundNullCoalescingOperator;
            }
            return this;
        }
    }
}
