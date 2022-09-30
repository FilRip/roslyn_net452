#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class DefaultSwitchLabelSyntax : SwitchLabelSyntax
    {
        public override SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DefaultSwitchLabelSyntax)base.Green).keyword, base.Position, 0);

        public override SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DefaultSwitchLabelSyntax)base.Green).colonToken, GetChildPosition(1), GetChildIndex(1));

        internal DefaultSwitchLabelSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitDefaultSwitchLabel(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDefaultSwitchLabel(this);

        public DefaultSwitchLabelSyntax Update(SyntaxToken keyword, SyntaxToken colonToken)
        {
            if (keyword != Keyword || colonToken != ColonToken)
            {
                DefaultSwitchLabelSyntax defaultSwitchLabelSyntax = SyntaxFactory.DefaultSwitchLabel(keyword, colonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return defaultSwitchLabelSyntax;
                }
                return defaultSwitchLabelSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override SwitchLabelSyntax WithKeywordCore(SyntaxToken keyword)
        {
            return WithKeyword(keyword);
        }

        public new DefaultSwitchLabelSyntax WithKeyword(SyntaxToken keyword)
        {
            return Update(keyword, ColonToken);
        }

        internal override SwitchLabelSyntax WithColonTokenCore(SyntaxToken colonToken)
        {
            return WithColonToken(colonToken);
        }

        public new DefaultSwitchLabelSyntax WithColonToken(SyntaxToken colonToken)
        {
            return Update(Keyword, colonToken);
        }
    }
}
