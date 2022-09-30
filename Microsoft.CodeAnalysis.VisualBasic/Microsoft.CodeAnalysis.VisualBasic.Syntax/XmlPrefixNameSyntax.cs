using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlPrefixNameSyntax : XmlNodeSyntax
	{
		public SyntaxToken Name => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax)base.Green)._name, base.Position, 0);

		internal XmlPrefixNameSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlPrefixNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlNameTokenSyntax name)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax(kind, errors, annotations, name), null, 0)
		{
		}

		public XmlPrefixNameSyntax WithName(SyntaxToken name)
		{
			return Update(name);
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
			return visitor.VisitXmlPrefixName(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlPrefixName(this);
		}

		public XmlPrefixNameSyntax Update(SyntaxToken name)
		{
			if (name != Name)
			{
				XmlPrefixNameSyntax xmlPrefixNameSyntax = SyntaxFactory.XmlPrefixName(name);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlPrefixNameSyntax, annotations);
				}
				return xmlPrefixNameSyntax;
			}
			return this;
		}
	}
}
