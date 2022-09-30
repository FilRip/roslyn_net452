#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class XmlNameAttributeSyntax : XmlAttributeSyntax
    {
        private XmlNameSyntax? name;

        private IdentifierNameSyntax? identifier;

        public override XmlNameSyntax Name => GetRedAtZero(ref name);

        public override SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlNameAttributeSyntax)base.Green).equalsToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken StartQuoteToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlNameAttributeSyntax)base.Green).startQuoteToken, GetChildPosition(2), GetChildIndex(2));

        public IdentifierNameSyntax Identifier => GetRed(ref identifier, 3);

        public override SyntaxToken EndQuoteToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlNameAttributeSyntax)base.Green).endQuoteToken, GetChildPosition(4), GetChildIndex(4));

        internal XmlNameAttributeSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref name),
                3 => GetRed(ref identifier, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => name,
                3 => identifier,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlNameAttribute(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlNameAttribute(this);

        public XmlNameAttributeSyntax Update(XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, IdentifierNameSyntax identifier, SyntaxToken endQuoteToken)
        {
            if (name != Name || equalsToken != EqualsToken || startQuoteToken != StartQuoteToken || identifier != Identifier || endQuoteToken != EndQuoteToken)
            {
                XmlNameAttributeSyntax xmlNameAttributeSyntax = SyntaxFactory.XmlNameAttribute(name, equalsToken, startQuoteToken, identifier, endQuoteToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return xmlNameAttributeSyntax;
                }
                return xmlNameAttributeSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override XmlAttributeSyntax WithNameCore(XmlNameSyntax name)
        {
            return WithName(name);
        }

        public new XmlNameAttributeSyntax WithName(XmlNameSyntax name)
        {
            return Update(name, EqualsToken, StartQuoteToken, Identifier, EndQuoteToken);
        }

        internal override XmlAttributeSyntax WithEqualsTokenCore(SyntaxToken equalsToken)
        {
            return WithEqualsToken(equalsToken);
        }

        public new XmlNameAttributeSyntax WithEqualsToken(SyntaxToken equalsToken)
        {
            return Update(Name, equalsToken, StartQuoteToken, Identifier, EndQuoteToken);
        }

        internal override XmlAttributeSyntax WithStartQuoteTokenCore(SyntaxToken startQuoteToken)
        {
            return WithStartQuoteToken(startQuoteToken);
        }

        public new XmlNameAttributeSyntax WithStartQuoteToken(SyntaxToken startQuoteToken)
        {
            return Update(Name, EqualsToken, startQuoteToken, Identifier, EndQuoteToken);
        }

        public XmlNameAttributeSyntax WithIdentifier(IdentifierNameSyntax identifier)
        {
            return Update(Name, EqualsToken, StartQuoteToken, identifier, EndQuoteToken);
        }

        internal override XmlAttributeSyntax WithEndQuoteTokenCore(SyntaxToken endQuoteToken)
        {
            return WithEndQuoteToken(endQuoteToken);
        }

        public new XmlNameAttributeSyntax WithEndQuoteToken(SyntaxToken endQuoteToken)
        {
            return Update(Name, EqualsToken, StartQuoteToken, Identifier, endQuoteToken);
        }
    }
}
