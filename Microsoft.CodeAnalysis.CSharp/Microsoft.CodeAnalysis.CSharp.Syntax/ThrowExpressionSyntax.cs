#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ThrowExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        public SyntaxToken ThrowKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ThrowExpressionSyntax)base.Green).throwKeyword, base.Position, 0);

        public ExpressionSyntax Expression => GetRed(ref expression, 1);

        internal ThrowExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitThrowExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitThrowExpression(this);

        public ThrowExpressionSyntax Update(SyntaxToken throwKeyword, ExpressionSyntax expression)
        {
            if (throwKeyword != ThrowKeyword || expression != Expression)
            {
                ThrowExpressionSyntax throwExpressionSyntax = SyntaxFactory.ThrowExpression(throwKeyword, expression);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return throwExpressionSyntax;
                }
                return throwExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ThrowExpressionSyntax WithThrowKeyword(SyntaxToken throwKeyword)
        {
            return Update(throwKeyword, Expression);
        }

        public ThrowExpressionSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(ThrowKeyword, expression);
        }
    }
}
