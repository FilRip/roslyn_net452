using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
	{
		internal ExpressionSyntax _expression;

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax)base.Green)._openParenToken, base.Position, 0);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax)base.Green)._closeParenToken, GetChildPosition(2), GetChildIndex(2));

		internal ParenthesizedExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ParenthesizedExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax(kind, errors, annotations, openParenToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, closeParenToken), null, 0)
		{
		}

		public ParenthesizedExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(openParenToken, Expression, CloseParenToken);
		}

		public ParenthesizedExpressionSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(OpenParenToken, expression, CloseParenToken);
		}

		public ParenthesizedExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(OpenParenToken, Expression, closeParenToken);
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
			return visitor.VisitParenthesizedExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitParenthesizedExpression(this);
		}

		public ParenthesizedExpressionSyntax Update(SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
		{
			if (openParenToken != OpenParenToken || expression != Expression || closeParenToken != CloseParenToken)
			{
				ParenthesizedExpressionSyntax parenthesizedExpressionSyntax = SyntaxFactory.ParenthesizedExpression(openParenToken, expression, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(parenthesizedExpressionSyntax, annotations);
				}
				return parenthesizedExpressionSyntax;
			}
			return this;
		}
	}
}
