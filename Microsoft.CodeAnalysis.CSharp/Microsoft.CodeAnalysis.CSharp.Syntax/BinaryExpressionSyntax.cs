#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? left;

        private ExpressionSyntax? right;

        public ExpressionSyntax Left => GetRedAtZero(ref left);

        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BinaryExpressionSyntax)base.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        public ExpressionSyntax Right => GetRed(ref right, 2);

        internal BinaryExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref left),
                2 => GetRed(ref right, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => left,
                2 => right,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitBinaryExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBinaryExpression(this);

        public BinaryExpressionSyntax Update(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            if (left != Left || operatorToken != OperatorToken || right != Right)
            {
                BinaryExpressionSyntax binaryExpressionSyntax = SyntaxFactory.BinaryExpression(Kind(), left, operatorToken, right);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return binaryExpressionSyntax;
                }
                return binaryExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public BinaryExpressionSyntax WithLeft(ExpressionSyntax left)
        {
            return Update(left, OperatorToken, Right);
        }

        public BinaryExpressionSyntax WithOperatorToken(SyntaxToken operatorToken)
        {
            return Update(Left, operatorToken, Right);
        }

        public BinaryExpressionSyntax WithRight(ExpressionSyntax right)
        {
            return Update(Left, OperatorToken, right);
        }
    }
}
