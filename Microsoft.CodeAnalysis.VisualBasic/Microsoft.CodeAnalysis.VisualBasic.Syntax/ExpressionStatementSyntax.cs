using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ExpressionStatementSyntax : ExecutableStatementSyntax
	{
		internal ExpressionSyntax _expression;

		public ExpressionSyntax Expression => GetRedAtZero(ref _expression);

		internal ExpressionStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ExpressionStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green), null, 0)
		{
		}

		public ExpressionStatementSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(expression);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 0)
			{
				return _expression;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 0)
			{
				return Expression;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitExpressionStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitExpressionStatement(this);
		}

		public ExpressionStatementSyntax Update(ExpressionSyntax expression)
		{
			if (expression != Expression)
			{
				ExpressionStatementSyntax expressionStatementSyntax = SyntaxFactory.ExpressionStatement(expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(expressionStatementSyntax, annotations);
				}
				return expressionStatementSyntax;
			}
			return this;
		}
	}
}
