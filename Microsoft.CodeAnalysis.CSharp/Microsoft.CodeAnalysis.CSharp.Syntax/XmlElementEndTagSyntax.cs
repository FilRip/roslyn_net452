#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class XmlElementEndTagSyntax : CSharpSyntaxNode
    {
        private XmlNameSyntax? name;

        public SyntaxToken LessThanSlashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlElementEndTagSyntax)base.Green).lessThanSlashToken, base.Position, 0);

        public XmlNameSyntax Name => GetRed(ref name, 1);

        public SyntaxToken GreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlElementEndTagSyntax)base.Green).greaterThanToken, GetChildPosition(2), GetChildIndex(2));

        internal XmlElementEndTagSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref name, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return name;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlElementEndTag(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlElementEndTag(this);

        public XmlElementEndTagSyntax Update(SyntaxToken lessThanSlashToken, XmlNameSyntax name, SyntaxToken greaterThanToken)
        {
            if (lessThanSlashToken != LessThanSlashToken || name != Name || greaterThanToken != GreaterThanToken)
            {
                XmlElementEndTagSyntax xmlElementEndTagSyntax = SyntaxFactory.XmlElementEndTag(lessThanSlashToken, name, greaterThanToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return xmlElementEndTagSyntax;
                }
                return xmlElementEndTagSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public XmlElementEndTagSyntax WithLessThanSlashToken(SyntaxToken lessThanSlashToken)
        {
            return Update(lessThanSlashToken, Name, GreaterThanToken);
        }

        public XmlElementEndTagSyntax WithName(XmlNameSyntax name)
        {
            return Update(LessThanSlashToken, name, GreaterThanToken);
        }

        public XmlElementEndTagSyntax WithGreaterThanToken(SyntaxToken greaterThanToken)
        {
            return Update(LessThanSlashToken, Name, greaterThanToken);
        }
    }
}
