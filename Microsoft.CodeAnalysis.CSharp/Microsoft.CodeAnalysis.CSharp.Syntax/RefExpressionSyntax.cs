#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class RefExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        public SyntaxToken RefKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.RefExpressionSyntax)base.Green).refKeyword, base.Position, 0);

        public ExpressionSyntax Expression => GetRed(ref expression, 1);

        internal RefExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref expression, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return expression;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitRefExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRefExpression(this);

        public RefExpressionSyntax Update(SyntaxToken refKeyword, ExpressionSyntax expression)
        {
            if (refKeyword != RefKeyword || expression != Expression)
            {
                RefExpressionSyntax refExpressionSyntax = SyntaxFactory.RefExpression(refKeyword, expression);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return refExpressionSyntax;
                }
                return refExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public RefExpressionSyntax WithRefKeyword(SyntaxToken refKeyword)
        {
            return Update(refKeyword, Expression);
        }

        public RefExpressionSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(RefKeyword, expression);
        }
    }
}
