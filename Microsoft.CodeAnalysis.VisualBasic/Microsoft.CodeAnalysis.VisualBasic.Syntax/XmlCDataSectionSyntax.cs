using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlCDataSectionSyntax : XmlNodeSyntax
	{
		public SyntaxToken BeginCDataToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax)base.Green)._beginCDataToken, base.Position, 0);

		public SyntaxTokenList TextTokens
		{
			get
			{
				GreenNode textTokens = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax)base.Green)._textTokens;
				return (textTokens == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, textTokens, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken EndCDataToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax)base.Green)._endCDataToken, GetChildPosition(2), GetChildIndex(2));

		internal XmlCDataSectionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlCDataSectionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax beginCDataToken, GreenNode textTokens, PunctuationSyntax endCDataToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax(kind, errors, annotations, beginCDataToken, textTokens, endCDataToken), null, 0)
		{
		}

		public XmlCDataSectionSyntax WithBeginCDataToken(SyntaxToken beginCDataToken)
		{
			return Update(beginCDataToken, TextTokens, EndCDataToken);
		}

		public XmlCDataSectionSyntax WithTextTokens(SyntaxTokenList textTokens)
		{
			return Update(BeginCDataToken, textTokens, EndCDataToken);
		}

		public XmlCDataSectionSyntax AddTextTokens(params SyntaxToken[] items)
		{
			return WithTextTokens(TextTokens.AddRange(items));
		}

		public XmlCDataSectionSyntax WithEndCDataToken(SyntaxToken endCDataToken)
		{
			return Update(BeginCDataToken, TextTokens, endCDataToken);
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
			return visitor.VisitXmlCDataSection(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlCDataSection(this);
		}

		public XmlCDataSectionSyntax Update(SyntaxToken beginCDataToken, SyntaxTokenList textTokens, SyntaxToken endCDataToken)
		{
			if (beginCDataToken != BeginCDataToken || textTokens != TextTokens || endCDataToken != EndCDataToken)
			{
				XmlCDataSectionSyntax xmlCDataSectionSyntax = SyntaxFactory.XmlCDataSection(beginCDataToken, textTokens, endCDataToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlCDataSectionSyntax, annotations);
				}
				return xmlCDataSectionSyntax;
			}
			return this;
		}
	}
}
