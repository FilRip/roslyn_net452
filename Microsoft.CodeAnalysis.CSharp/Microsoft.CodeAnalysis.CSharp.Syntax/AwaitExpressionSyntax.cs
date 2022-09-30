#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class AwaitExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        public SyntaxToken AwaitKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AwaitExpressionSyntax)base.Green).awaitKeyword, base.Position, 0);

        public ExpressionSyntax Expression => GetRed(ref expression, 1);

        internal AwaitExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitAwaitExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAwaitExpression(this);

        public AwaitExpressionSyntax Update(SyntaxToken awaitKeyword, ExpressionSyntax expression)
        {
            if (awaitKeyword != AwaitKeyword || expression != Expression)
            {
                AwaitExpressionSyntax awaitExpressionSyntax = SyntaxFactory.AwaitExpression(awaitKeyword, expression);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return awaitExpressionSyntax;
                }
                return awaitExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public AwaitExpressionSyntax WithAwaitKeyword(SyntaxToken awaitKeyword)
        {
            return Update(awaitKeyword, Expression);
        }

        public AwaitExpressionSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(AwaitKeyword, expression);
        }
    }
}
