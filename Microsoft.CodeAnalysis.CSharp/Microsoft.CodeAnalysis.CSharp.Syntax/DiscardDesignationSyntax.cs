#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class DiscardDesignationSyntax : VariableDesignationSyntax
    {
        public SyntaxToken UnderscoreToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DiscardDesignationSyntax)base.Green).underscoreToken, base.Position, 0);

        internal DiscardDesignationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitDiscardDesignation(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDiscardDesignation(this);

        public DiscardDesignationSyntax Update(SyntaxToken underscoreToken)
        {
            if (underscoreToken != UnderscoreToken)
            {
                DiscardDesignationSyntax discardDesignationSyntax = SyntaxFactory.DiscardDesignation(underscoreToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return discardDesignationSyntax;
                }
                return discardDesignationSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public DiscardDesignationSyntax WithUnderscoreToken(SyntaxToken underscoreToken)
        {
            return Update(underscoreToken);
        }
    }
}
