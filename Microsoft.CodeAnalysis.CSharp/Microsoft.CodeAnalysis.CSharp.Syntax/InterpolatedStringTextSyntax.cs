#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class InterpolatedStringTextSyntax : InterpolatedStringContentSyntax
    {
        public SyntaxToken TextToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InterpolatedStringTextSyntax)base.Green).textToken, base.Position, 0);

        internal InterpolatedStringTextSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitInterpolatedStringText(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitInterpolatedStringText(this);

        public InterpolatedStringTextSyntax Update(SyntaxToken textToken)
        {
            if (textToken != TextToken)
            {
                InterpolatedStringTextSyntax interpolatedStringTextSyntax = SyntaxFactory.InterpolatedStringText(textToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return interpolatedStringTextSyntax;
                }
                return interpolatedStringTextSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public InterpolatedStringTextSyntax WithTextToken(SyntaxToken textToken)
        {
            return Update(textToken);
        }
    }
}
