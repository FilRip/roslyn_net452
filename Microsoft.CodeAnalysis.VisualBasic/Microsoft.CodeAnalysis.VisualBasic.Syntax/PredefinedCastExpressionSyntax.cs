using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class PredefinedCastExpressionSyntax : ExpressionSyntax
	{
		internal ExpressionSyntax _expression;

		public SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax)base.Green)._keyword, base.Position, 0);

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax)base.Green)._openParenToken, GetChildPosition(1), GetChildIndex(1));

		public ExpressionSyntax Expression => GetRed(ref _expression, 2);

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax)base.Green)._closeParenToken, GetChildPosition(3), GetChildIndex(3));

		internal PredefinedCastExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal PredefinedCastExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax(kind, errors, annotations, keyword, openParenToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, closeParenToken), null, 0)
		{
		}

		public PredefinedCastExpressionSyntax WithKeyword(SyntaxToken keyword)
		{
			return Update(keyword, OpenParenToken, Expression, CloseParenToken);
		}

		public PredefinedCastExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(Keyword, openParenToken, Expression, CloseParenToken);
		}

		public PredefinedCastExpressionSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(Keyword, OpenParenToken, expression, CloseParenToken);
		}

		public PredefinedCastExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(Keyword, OpenParenToken, Expression, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _expression;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return Expression;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitPredefinedCastExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitPredefinedCastExpression(this);
		}

		public PredefinedCastExpressionSyntax Update(SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
		{
			if (keyword != Keyword || openParenToken != OpenParenToken || expression != Expression || closeParenToken != CloseParenToken)
			{
				PredefinedCastExpressionSyntax predefinedCastExpressionSyntax = SyntaxFactory.PredefinedCastExpression(keyword, openParenToken, expression, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(predefinedCastExpressionSyntax, annotations);
				}
				return predefinedCastExpressionSyntax;
			}
			return this;
		}
	}
}
