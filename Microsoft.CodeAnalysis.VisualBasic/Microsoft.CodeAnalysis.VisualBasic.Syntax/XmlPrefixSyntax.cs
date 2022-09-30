using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlPrefixSyntax : VisualBasicSyntaxNode
	{
		public SyntaxToken Name => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax)base.Green)._name, base.Position, 0);

		public SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax)base.Green)._colonToken, GetChildPosition(1), GetChildIndex(1));

		internal XmlPrefixSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlPrefixSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlNameTokenSyntax name, PunctuationSyntax colonToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax(kind, errors, annotations, name, colonToken), null, 0)
		{
		}

		public XmlPrefixSyntax WithName(SyntaxToken name)
		{
			return Update(name, ColonToken);
		}

		public XmlPrefixSyntax WithColonToken(SyntaxToken colonToken)
		{
			return Update(Name, colonToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitXmlPrefix(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlPrefix(this);
		}

		public XmlPrefixSyntax Update(SyntaxToken name, SyntaxToken colonToken)
		{
			if (name != Name || colonToken != ColonToken)
			{
				XmlPrefixSyntax xmlPrefixSyntax = SyntaxFactory.XmlPrefix(name, colonToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlPrefixSyntax, annotations);
				}
				return xmlPrefixSyntax;
			}
			return this;
		}
	}
}
