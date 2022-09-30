#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class PrefixUnaryExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? operand;

        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PrefixUnaryExpressionSyntax)base.Green).operatorToken, base.Position, 0);

        public ExpressionSyntax Operand => GetRed(ref operand, 1);

        internal PrefixUnaryExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref operand, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return operand;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitPrefixUnaryExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPrefixUnaryExpression(this);

        public PrefixUnaryExpressionSyntax Update(SyntaxToken operatorToken, ExpressionSyntax operand)
        {
            if (operatorToken != OperatorToken || operand != Operand)
            {
                PrefixUnaryExpressionSyntax prefixUnaryExpressionSyntax = SyntaxFactory.PrefixUnaryExpression(Kind(), operatorToken, operand);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return prefixUnaryExpressionSyntax;
                }
                return prefixUnaryExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public PrefixUnaryExpressionSyntax WithOperatorToken(SyntaxToken operatorToken)
        {
            return Update(operatorToken, Operand);
        }

        public PrefixUnaryExpressionSyntax WithOperand(ExpressionSyntax operand)
        {
            return Update(OperatorToken, operand);
        }
    }
}
