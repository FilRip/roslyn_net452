using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlProcessingInstructionSyntax : XmlNodeSyntax
	{
		public SyntaxToken LessThanQuestionToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax)base.Green)._lessThanQuestionToken, base.Position, 0);

		public SyntaxToken Name => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax)base.Green)._name, GetChildPosition(1), GetChildIndex(1));

		public SyntaxTokenList TextTokens
		{
			get
			{
				GreenNode textTokens = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax)base.Green)._textTokens;
				return (textTokens == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, textTokens, GetChildPosition(2), GetChildIndex(2));
			}
		}

		public SyntaxToken QuestionGreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax)base.Green)._questionGreaterThanToken, GetChildPosition(3), GetChildIndex(3));

		internal XmlProcessingInstructionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlProcessingInstructionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanQuestionToken, XmlNameTokenSyntax name, GreenNode textTokens, PunctuationSyntax questionGreaterThanToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax(kind, errors, annotations, lessThanQuestionToken, name, textTokens, questionGreaterThanToken), null, 0)
		{
		}

		public XmlProcessingInstructionSyntax WithLessThanQuestionToken(SyntaxToken lessThanQuestionToken)
		{
			return Update(lessThanQuestionToken, Name, TextTokens, QuestionGreaterThanToken);
		}

		public XmlProcessingInstructionSyntax WithName(SyntaxToken name)
		{
			return Update(LessThanQuestionToken, name, TextTokens, QuestionGreaterThanToken);
		}

		public XmlProcessingInstructionSyntax WithTextTokens(SyntaxTokenList textTokens)
		{
			return Update(LessThanQuestionToken, Name, textTokens, QuestionGreaterThanToken);
		}

		public XmlProcessingInstructionSyntax AddTextTokens(params SyntaxToken[] items)
		{
			return WithTextTokens(TextTokens.AddRange(items));
		}

		public XmlProcessingInstructionSyntax WithQuestionGreaterThanToken(SyntaxToken questionGreaterThanToken)
		{
			return Update(LessThanQuestionToken, Name, TextTokens, questionGreaterThanToken);
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
			return visitor.VisitXmlProcessingInstruction(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlProcessingInstruction(this);
		}

		public XmlProcessingInstructionSyntax Update(SyntaxToken lessThanQuestionToken, SyntaxToken name, SyntaxTokenList textTokens, SyntaxToken questionGreaterThanToken)
		{
			if (lessThanQuestionToken != LessThanQuestionToken || name != Name || textTokens != TextTokens || questionGreaterThanToken != QuestionGreaterThanToken)
			{
				XmlProcessingInstructionSyntax xmlProcessingInstructionSyntax = SyntaxFactory.XmlProcessingInstruction(lessThanQuestionToken, name, textTokens, questionGreaterThanToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlProcessingInstructionSyntax, annotations);
				}
				return xmlProcessingInstructionSyntax;
			}
			return this;
		}
	}
}
