#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class OmittedTypeArgumentSyntax : TypeSyntax
    {
        public SyntaxToken OmittedTypeArgumentToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.OmittedTypeArgumentSyntax)base.Green).omittedTypeArgumentToken, base.Position, 0);

        internal OmittedTypeArgumentSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitOmittedTypeArgument(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitOmittedTypeArgument(this);

        public OmittedTypeArgumentSyntax Update(SyntaxToken omittedTypeArgumentToken)
        {
            if (omittedTypeArgumentToken != OmittedTypeArgumentToken)
            {
                OmittedTypeArgumentSyntax omittedTypeArgumentSyntax = SyntaxFactory.OmittedTypeArgument(omittedTypeArgumentToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return omittedTypeArgumentSyntax;
                }
                return omittedTypeArgumentSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public OmittedTypeArgumentSyntax WithOmittedTypeArgumentToken(SyntaxToken omittedTypeArgumentToken)
        {
            return Update(omittedTypeArgumentToken);
        }
    }
}
