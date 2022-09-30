using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlTextSyntax : XmlNodeSyntax
	{
		public SyntaxTokenList TextTokens
		{
			get
			{
				GreenNode textTokens = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax)base.Green)._textTokens;
				return (textTokens == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, textTokens, base.Position, 0);
			}
		}

		internal XmlTextSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlTextSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode textTokens)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax(kind, errors, annotations, textTokens), null, 0)
		{
		}

		public XmlTextSyntax WithTextTokens(SyntaxTokenList textTokens)
		{
			return Update(textTokens);
		}

		public XmlTextSyntax AddTextTokens(params SyntaxToken[] items)
		{
			return WithTextTokens(TextTokens.AddRange(items));
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
			return visitor.VisitXmlText(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlText(this);
		}

		public XmlTextSyntax Update(SyntaxTokenList textTokens)
		{
			if (textTokens != TextTokens)
			{
				XmlTextSyntax xmlTextSyntax = SyntaxFactory.XmlText(textTokens);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlTextSyntax, annotations);
				}
				return xmlTextSyntax;
			}
			return this;
		}
	}
}
