using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlCommentSyntax : XmlNodeSyntax
	{
		public SyntaxToken LessThanExclamationMinusMinusToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax)base.Green)._lessThanExclamationMinusMinusToken, base.Position, 0);

		public SyntaxTokenList TextTokens
		{
			get
			{
				GreenNode textTokens = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax)base.Green)._textTokens;
				return (textTokens == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, textTokens, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken MinusMinusGreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax)base.Green)._minusMinusGreaterThanToken, GetChildPosition(2), GetChildIndex(2));

		internal XmlCommentSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlCommentSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanExclamationMinusMinusToken, GreenNode textTokens, PunctuationSyntax minusMinusGreaterThanToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax(kind, errors, annotations, lessThanExclamationMinusMinusToken, textTokens, minusMinusGreaterThanToken), null, 0)
		{
		}

		public XmlCommentSyntax WithLessThanExclamationMinusMinusToken(SyntaxToken lessThanExclamationMinusMinusToken)
		{
			return Update(lessThanExclamationMinusMinusToken, TextTokens, MinusMinusGreaterThanToken);
		}

		public XmlCommentSyntax WithTextTokens(SyntaxTokenList textTokens)
		{
			return Update(LessThanExclamationMinusMinusToken, textTokens, MinusMinusGreaterThanToken);
		}

		public XmlCommentSyntax AddTextTokens(params SyntaxToken[] items)
		{
			return WithTextTokens(TextTokens.AddRange(items));
		}

		public XmlCommentSyntax WithMinusMinusGreaterThanToken(SyntaxToken minusMinusGreaterThanToken)
		{
			return Update(LessThanExclamationMinusMinusToken, TextTokens, minusMinusGreaterThanToken);
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
			return visitor.VisitXmlComment(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlComment(this);
		}

		public XmlCommentSyntax Update(SyntaxToken lessThanExclamationMinusMinusToken, SyntaxTokenList textTokens, SyntaxToken minusMinusGreaterThanToken)
		{
			if (lessThanExclamationMinusMinusToken != LessThanExclamationMinusMinusToken || textTokens != TextTokens || minusMinusGreaterThanToken != MinusMinusGreaterThanToken)
			{
				XmlCommentSyntax xmlCommentSyntax = SyntaxFactory.XmlComment(lessThanExclamationMinusMinusToken, textTokens, minusMinusGreaterThanToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlCommentSyntax, annotations);
				}
				return xmlCommentSyntax;
			}
			return this;
		}
	}
}
