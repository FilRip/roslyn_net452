#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class XmlTextAttributeSyntax : XmlAttributeSyntax
    {
        private XmlNameSyntax? name;

        public override XmlNameSyntax Name => GetRedAtZero(ref name);

        public override SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlTextAttributeSyntax)base.Green).equalsToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken StartQuoteToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlTextAttributeSyntax)base.Green).startQuoteToken, GetChildPosition(2), GetChildIndex(2));

        public SyntaxTokenList TextTokens
        {
            get
            {
                GreenNode slot = base.Green.GetSlot(3);
                if (slot == null)
                {
                    return default(SyntaxTokenList);
                }
                return new SyntaxTokenList(this, slot, GetChildPosition(3), GetChildIndex(3));
            }
        }

        public override SyntaxToken EndQuoteToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlTextAttributeSyntax)base.Green).endQuoteToken, GetChildPosition(4), GetChildIndex(4));

        internal XmlTextAttributeSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return GetRedAtZero(ref name);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return name;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlTextAttribute(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlTextAttribute(this);

        public XmlTextAttributeSyntax Update(XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, SyntaxTokenList textTokens, SyntaxToken endQuoteToken)
        {
            if (name != Name || equalsToken != EqualsToken || startQuoteToken != StartQuoteToken || textTokens != TextTokens || endQuoteToken != EndQuoteToken)
            {
                XmlTextAttributeSyntax xmlTextAttributeSyntax = SyntaxFactory.XmlTextAttribute(name, equalsToken, startQuoteToken, textTokens, endQuoteToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return xmlTextAttributeSyntax;
                }
                return xmlTextAttributeSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override XmlAttributeSyntax WithNameCore(XmlNameSyntax name)
        {
            return WithName(name);
        }

        public new XmlTextAttributeSyntax WithName(XmlNameSyntax name)
        {
            return Update(name, EqualsToken, StartQuoteToken, TextTokens, EndQuoteToken);
        }

        internal override XmlAttributeSyntax WithEqualsTokenCore(SyntaxToken equalsToken)
        {
            return WithEqualsToken(equalsToken);
        }

        public new XmlTextAttributeSyntax WithEqualsToken(SyntaxToken equalsToken)
        {
            return Update(Name, equalsToken, StartQuoteToken, TextTokens, EndQuoteToken);
        }

        internal override XmlAttributeSyntax WithStartQuoteTokenCore(SyntaxToken startQuoteToken)
        {
            return WithStartQuoteToken(startQuoteToken);
        }

        public new XmlTextAttributeSyntax WithStartQuoteToken(SyntaxToken startQuoteToken)
        {
            return Update(Name, EqualsToken, startQuoteToken, TextTokens, EndQuoteToken);
        }

        public XmlTextAttributeSyntax WithTextTokens(SyntaxTokenList textTokens)
        {
            return Update(Name, EqualsToken, StartQuoteToken, textTokens, EndQuoteToken);
        }

        internal override XmlAttributeSyntax WithEndQuoteTokenCore(SyntaxToken endQuoteToken)
        {
            return WithEndQuoteToken(endQuoteToken);
        }

        public new XmlTextAttributeSyntax WithEndQuoteToken(SyntaxToken endQuoteToken)
        {
            return Update(Name, EqualsToken, StartQuoteToken, TextTokens, endQuoteToken);
        }

        public XmlTextAttributeSyntax AddTextTokens(params SyntaxToken[] items)
        {
            return WithTextTokens(TextTokens.AddRange(items));
        }
    }
}
