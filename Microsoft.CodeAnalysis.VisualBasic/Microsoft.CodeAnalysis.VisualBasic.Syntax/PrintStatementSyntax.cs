using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class PrintStatementSyntax : ExecutableStatementSyntax
	{
		internal ExpressionSyntax _expression;

		public SyntaxToken QuestionToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax)base.Green)._questionToken, base.Position, 0);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		internal PrintStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal PrintStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax questionToken, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax(kind, errors, annotations, questionToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green), null, 0)
		{
		}

		public PrintStatementSyntax WithQuestionToken(SyntaxToken questionToken)
		{
			return Update(questionToken, Expression);
		}

		public PrintStatementSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(QuestionToken, expression);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _expression;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Expression;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitPrintStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitPrintStatement(this);
		}

		public PrintStatementSyntax Update(SyntaxToken questionToken, ExpressionSyntax expression)
		{
			if (questionToken != QuestionToken || expression != Expression)
			{
				PrintStatementSyntax printStatementSyntax = SyntaxFactory.PrintStatement(questionToken, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(printStatementSyntax, annotations);
				}
				return printStatementSyntax;
			}
			return this;
		}
	}
}
