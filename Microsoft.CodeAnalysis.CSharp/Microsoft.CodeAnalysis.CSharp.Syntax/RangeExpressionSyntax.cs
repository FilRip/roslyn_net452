#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class RangeExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? leftOperand;

        private ExpressionSyntax? rightOperand;

        public ExpressionSyntax? LeftOperand => GetRedAtZero(ref leftOperand);

        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.RangeExpressionSyntax)base.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        public ExpressionSyntax? RightOperand => GetRed(ref rightOperand, 2);

        internal RangeExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref leftOperand),
                2 => GetRed(ref rightOperand, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => leftOperand,
                2 => rightOperand,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitRangeExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRangeExpression(this);

        public RangeExpressionSyntax Update(ExpressionSyntax? leftOperand, SyntaxToken operatorToken, ExpressionSyntax? rightOperand)
        {
            if (leftOperand != LeftOperand || operatorToken != OperatorToken || rightOperand != RightOperand)
            {
                RangeExpressionSyntax rangeExpressionSyntax = SyntaxFactory.RangeExpression(leftOperand, operatorToken, rightOperand);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return rangeExpressionSyntax;
                }
                return rangeExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public RangeExpressionSyntax WithLeftOperand(ExpressionSyntax? leftOperand)
        {
            return Update(leftOperand, OperatorToken, RightOperand);
        }

        public RangeExpressionSyntax WithOperatorToken(SyntaxToken operatorToken)
        {
            return Update(LeftOperand, operatorToken, RightOperand);
        }

        public RangeExpressionSyntax WithRightOperand(ExpressionSyntax? rightOperand)
        {
            return Update(LeftOperand, OperatorToken, rightOperand);
        }
    }
}
