#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class XmlNameSyntax : CSharpSyntaxNode
    {
        private XmlPrefixSyntax? prefix;

        public XmlPrefixSyntax? Prefix => GetRedAtZero(ref prefix);

        public SyntaxToken LocalName => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlNameSyntax)base.Green).localName, GetChildPosition(1), GetChildIndex(1));

        internal XmlNameSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return GetRedAtZero(ref prefix);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return prefix;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlName(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlName(this);

        public XmlNameSyntax Update(XmlPrefixSyntax? prefix, SyntaxToken localName)
        {
            if (prefix != Prefix || localName != LocalName)
            {
                XmlNameSyntax xmlNameSyntax = SyntaxFactory.XmlName(prefix, localName);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return xmlNameSyntax;
                }
                return xmlNameSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public XmlNameSyntax WithPrefix(XmlPrefixSyntax? prefix)
        {
            return Update(prefix, LocalName);
        }

        public XmlNameSyntax WithLocalName(SyntaxToken localName)
        {
            return Update(Prefix, localName);
        }
    }
}
