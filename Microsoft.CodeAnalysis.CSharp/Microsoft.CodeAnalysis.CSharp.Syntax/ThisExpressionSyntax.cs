#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ThisExpressionSyntax : InstanceExpressionSyntax
    {
        public SyntaxToken Token => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ThisExpressionSyntax)base.Green).token, base.Position, 0);

        internal ThisExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitThisExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitThisExpression(this);

        public ThisExpressionSyntax Update(SyntaxToken token)
        {
            if (token != Token)
            {
                ThisExpressionSyntax thisExpressionSyntax = SyntaxFactory.ThisExpression(token);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return thisExpressionSyntax;
                }
                return thisExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ThisExpressionSyntax WithToken(SyntaxToken token)
        {
            return Update(token);
        }
    }
}
