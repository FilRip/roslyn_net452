using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ConditionalAccessExpressionSyntax : ExpressionSyntax
	{
		internal ExpressionSyntax _expression;

		internal ExpressionSyntax _whenNotNull;

		public ExpressionSyntax Expression => GetRedAtZero(ref _expression);

		public SyntaxToken QuestionMarkToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax)base.Green)._questionMarkToken, GetChildPosition(1), GetChildIndex(1));

		public ExpressionSyntax WhenNotNull => GetRed(ref _whenNotNull, 2);

		internal ConditionalAccessExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ConditionalAccessExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax expression, PunctuationSyntax questionMarkToken, ExpressionSyntax whenNotNull)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax(kind, errors, annotations, (expression != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green) : null, questionMarkToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)whenNotNull.Green), null, 0)
		{
		}

		public ConditionalAccessExpressionSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(expression, QuestionMarkToken, WhenNotNull);
		}

		public ConditionalAccessExpressionSyntax WithQuestionMarkToken(SyntaxToken questionMarkToken)
		{
			return Update(Expression, questionMarkToken, WhenNotNull);
		}

		public ConditionalAccessExpressionSyntax WithWhenNotNull(ExpressionSyntax whenNotNull)
		{
			return Update(Expression, QuestionMarkToken, whenNotNull);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _expression, 
				2 => _whenNotNull, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Expression, 
				2 => WhenNotNull, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitConditionalAccessExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitConditionalAccessExpression(this);
		}

		public ConditionalAccessExpressionSyntax Update(ExpressionSyntax expression, SyntaxToken questionMarkToken, ExpressionSyntax whenNotNull)
		{
			if (expression != Expression || questionMarkToken != QuestionMarkToken || whenNotNull != WhenNotNull)
			{
				ConditionalAccessExpressionSyntax conditionalAccessExpressionSyntax = SyntaxFactory.ConditionalAccessExpression(expression, questionMarkToken, whenNotNull);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(conditionalAccessExpressionSyntax, annotations);
				}
				return conditionalAccessExpressionSyntax;
			}
			return this;
		}
	}
}
