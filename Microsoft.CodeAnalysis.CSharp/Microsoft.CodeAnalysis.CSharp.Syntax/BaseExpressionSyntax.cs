#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class BaseExpressionSyntax : InstanceExpressionSyntax
    {
        public SyntaxToken Token => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BaseExpressionSyntax)base.Green).token, base.Position, 0);

        internal BaseExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitBaseExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBaseExpression(this);

        public BaseExpressionSyntax Update(SyntaxToken token)
        {
            if (token != Token)
            {
                BaseExpressionSyntax baseExpressionSyntax = SyntaxFactory.BaseExpression(token);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return baseExpressionSyntax;
                }
                return baseExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public BaseExpressionSyntax WithToken(SyntaxToken token)
        {
            return Update(token);
        }
    }
}
