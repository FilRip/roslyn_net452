#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class PostfixUnaryExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? operand;

        public ExpressionSyntax Operand => GetRedAtZero(ref operand);

        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PostfixUnaryExpressionSyntax)base.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        internal PostfixUnaryExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return GetRedAtZero(ref operand);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return operand;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitPostfixUnaryExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPostfixUnaryExpression(this);

        public PostfixUnaryExpressionSyntax Update(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            if (operand != Operand || operatorToken != OperatorToken)
            {
                PostfixUnaryExpressionSyntax postfixUnaryExpressionSyntax = SyntaxFactory.PostfixUnaryExpression(Kind(), operand, operatorToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return postfixUnaryExpressionSyntax;
                }
                return postfixUnaryExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public PostfixUnaryExpressionSyntax WithOperand(ExpressionSyntax operand)
        {
            return Update(operand, OperatorToken);
        }

        public PostfixUnaryExpressionSyntax WithOperatorToken(SyntaxToken operatorToken)
        {
            return Update(Operand, operatorToken);
        }
    }
}
