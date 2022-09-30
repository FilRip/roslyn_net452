#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken Token => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LiteralExpressionSyntax)base.Green).token, base.Position, 0);

        internal LiteralExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return null;
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return null;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitLiteralExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLiteralExpression(this);

        public LiteralExpressionSyntax Update(SyntaxToken token)
        {
            if (token != Token)
            {
                LiteralExpressionSyntax literalExpressionSyntax = SyntaxFactory.LiteralExpression(Kind(), token);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return literalExpressionSyntax;
                }
                return literalExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public LiteralExpressionSyntax WithToken(SyntaxToken token)
        {
            return Update(token);
        }
    }
}
