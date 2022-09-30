using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class AwaitExpressionSyntax : ExpressionSyntax
	{
		internal ExpressionSyntax _expression;

		public SyntaxToken AwaitKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax)base.Green)._awaitKeyword, base.Position, 0);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		internal AwaitExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal AwaitExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax awaitKeyword, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax(kind, errors, annotations, awaitKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green), null, 0)
		{
		}

		public AwaitExpressionSyntax WithAwaitKeyword(SyntaxToken awaitKeyword)
		{
			return Update(awaitKeyword, Expression);
		}

		public AwaitExpressionSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(AwaitKeyword, expression);
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
			return visitor.VisitAwaitExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitAwaitExpression(this);
		}

		public AwaitExpressionSyntax Update(SyntaxToken awaitKeyword, ExpressionSyntax expression)
		{
			if (awaitKeyword != AwaitKeyword || expression != Expression)
			{
				AwaitExpressionSyntax awaitExpressionSyntax = SyntaxFactory.AwaitExpression(awaitKeyword, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(awaitExpressionSyntax, annotations);
				}
				return awaitExpressionSyntax;
			}
			return this;
		}
	}
}
