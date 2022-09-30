using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlNameSyntax : XmlNodeSyntax
	{
		internal XmlPrefixSyntax _prefix;

		public XmlPrefixSyntax Prefix => GetRedAtZero(ref _prefix);

		public SyntaxToken LocalName => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)base.Green)._localName, GetChildPosition(1), GetChildIndex(1));

		internal XmlNameSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlPrefixSyntax prefix, XmlNameTokenSyntax localName)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax(kind, errors, annotations, (prefix != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax)prefix.Green) : null, localName), null, 0)
		{
		}

		public XmlNameSyntax WithPrefix(XmlPrefixSyntax prefix)
		{
			return Update(prefix, LocalName);
		}

		public XmlNameSyntax WithLocalName(SyntaxToken localName)
		{
			return Update(Prefix, localName);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 0)
			{
				return _prefix;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 0)
			{
				return Prefix;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitXmlName(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlName(this);
		}

		public XmlNameSyntax Update(XmlPrefixSyntax prefix, SyntaxToken localName)
		{
			if (prefix != Prefix || localName != LocalName)
			{
				XmlNameSyntax xmlNameSyntax = SyntaxFactory.XmlName(prefix, localName);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlNameSyntax, annotations);
				}
				return xmlNameSyntax;
			}
			return this;
		}
	}
}
