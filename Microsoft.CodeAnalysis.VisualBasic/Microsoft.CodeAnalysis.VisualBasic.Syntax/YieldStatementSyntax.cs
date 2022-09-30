using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class YieldStatementSyntax : ExecutableStatementSyntax
	{
		internal ExpressionSyntax _expression;

		public SyntaxToken YieldKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax)base.Green)._yieldKeyword, base.Position, 0);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		internal YieldStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal YieldStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax yieldKeyword, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax(kind, errors, annotations, yieldKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green), null, 0)
		{
		}

		public YieldStatementSyntax WithYieldKeyword(SyntaxToken yieldKeyword)
		{
			return Update(yieldKeyword, Expression);
		}

		public YieldStatementSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(YieldKeyword, expression);
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
			return visitor.VisitYieldStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitYieldStatement(this);
		}

		public YieldStatementSyntax Update(SyntaxToken yieldKeyword, ExpressionSyntax expression)
		{
			if (yieldKeyword != YieldKeyword || expression != Expression)
			{
				YieldStatementSyntax yieldStatementSyntax = SyntaxFactory.YieldStatement(yieldKeyword, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(yieldStatementSyntax, annotations);
				}
				return yieldStatementSyntax;
			}
			return this;
		}
	}
}
