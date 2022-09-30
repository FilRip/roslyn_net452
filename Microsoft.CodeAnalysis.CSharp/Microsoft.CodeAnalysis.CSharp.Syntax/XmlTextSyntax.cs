#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class XmlTextSyntax : XmlNodeSyntax
    {
        public SyntaxTokenList TextTokens
        {
            get
            {
                GreenNode slot = base.Green.GetSlot(0);
                if (slot == null)
                {
                    return default(SyntaxTokenList);
                }
                return new SyntaxTokenList(this, slot, base.Position, 0);
            }
        }

        internal XmlTextSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitXmlText(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlText(this);

        public XmlTextSyntax Update(SyntaxTokenList textTokens)
        {
            if (textTokens != TextTokens)
            {
                XmlTextSyntax xmlTextSyntax = SyntaxFactory.XmlText(textTokens);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return xmlTextSyntax;
                }
                return xmlTextSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public XmlTextSyntax WithTextTokens(SyntaxTokenList textTokens)
        {
            return Update(textTokens);
        }

        public XmlTextSyntax AddTextTokens(params SyntaxToken[] items)
        {
            return WithTextTokens(TextTokens.AddRange(items));
        }
    }
}
