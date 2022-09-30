using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundNullCoalescingAssignmentOperator : BoundExpression
    {
        internal bool IsNullableValueTypeAssignment
        {
            get
            {
                TypeSymbol type = LeftOperand.Type;
                if ((object)type == null || !type.IsNullableType())
                {
                    return false;
                }
                return type.GetNullableUnderlyingType().Equals(RightOperand.Type);
            }
        }

        public BoundExpression LeftOperand { get; }

        public BoundExpression RightOperand { get; }

        public BoundNullCoalescingAssignmentOperator(SyntaxNode syntax, BoundExpression leftOperand, BoundExpression rightOperand, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.NullCoalescingAssignmentOperator, syntax, type, hasErrors || leftOperand.HasErrors() || rightOperand.HasErrors())
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitNullCoalescingAssignmentOperator(this);
        }

        public BoundNullCoalescingAssignmentOperator Update(BoundExpression leftOperand, BoundExpression rightOperand, TypeSymbol? type)
        {
            if (leftOperand != LeftOperand || rightOperand != RightOperand || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundNullCoalescingAssignmentOperator boundNullCoalescingAssignmentOperator = new BoundNullCoalescingAssignmentOperator(Syntax, leftOperand, rightOperand, type, base.HasErrors);
                boundNullCoalescingAssignmentOperator.CopyAttributes(this);
                return boundNullCoalescingAssignmentOperator;
            }
            return this;
        }
    }
}
