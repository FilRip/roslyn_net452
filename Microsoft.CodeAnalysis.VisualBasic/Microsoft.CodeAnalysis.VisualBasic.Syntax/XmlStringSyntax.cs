using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlStringSyntax : XmlNodeSyntax
	{
		public SyntaxToken StartQuoteToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)base.Green)._startQuoteToken, base.Position, 0);

		public SyntaxTokenList TextTokens
		{
			get
			{
				GreenNode textTokens = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)base.Green)._textTokens;
				return (textTokens == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, textTokens, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken EndQuoteToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)base.Green)._endQuoteToken, GetChildPosition(2), GetChildIndex(2));

		internal XmlStringSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlStringSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax startQuoteToken, GreenNode textTokens, PunctuationSyntax endQuoteToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax(kind, errors, annotations, startQuoteToken, textTokens, endQuoteToken), null, 0)
		{
		}

		public XmlStringSyntax WithStartQuoteToken(SyntaxToken startQuoteToken)
		{
			return Update(startQuoteToken, TextTokens, EndQuoteToken);
		}

		public XmlStringSyntax WithTextTokens(SyntaxTokenList textTokens)
		{
			return Update(StartQuoteToken, textTokens, EndQuoteToken);
		}

		public XmlStringSyntax AddTextTokens(params SyntaxToken[] items)
		{
			return WithTextTokens(TextTokens.AddRange(items));
		}

		public XmlStringSyntax WithEndQuoteToken(SyntaxToken endQuoteToken)
		{
			return Update(StartQuoteToken, TextTokens, endQuoteToken);
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
			return visitor.VisitXmlString(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlString(this);
		}

		public XmlStringSyntax Update(SyntaxToken startQuoteToken, SyntaxTokenList textTokens, SyntaxToken endQuoteToken)
		{
			if (startQuoteToken != StartQuoteToken || textTokens != TextTokens || endQuoteToken != EndQuoteToken)
			{
				XmlStringSyntax xmlStringSyntax = SyntaxFactory.XmlString(startQuoteToken, textTokens, endQuoteToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlStringSyntax, annotations);
				}
				return xmlStringSyntax;
			}
			return this;
		}
	}
}
