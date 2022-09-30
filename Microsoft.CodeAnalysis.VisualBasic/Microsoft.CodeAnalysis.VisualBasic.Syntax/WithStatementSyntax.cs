using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class WithStatementSyntax : StatementSyntax
	{
		internal ExpressionSyntax _expression;

		public SyntaxToken WithKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax)base.Green)._withKeyword, base.Position, 0);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		internal WithStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal WithStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax withKeyword, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax(kind, errors, annotations, withKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green), null, 0)
		{
		}

		public WithStatementSyntax WithWithKeyword(SyntaxToken withKeyword)
		{
			return Update(withKeyword, Expression);
		}

		public WithStatementSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(WithKeyword, expression);
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
			return visitor.VisitWithStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitWithStatement(this);
		}

		public WithStatementSyntax Update(SyntaxToken withKeyword, ExpressionSyntax expression)
		{
			if (withKeyword != WithKeyword || expression != Expression)
			{
				WithStatementSyntax withStatementSyntax = SyntaxFactory.WithStatement(withKeyword, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(withStatementSyntax, annotations);
				}
				return withStatementSyntax;
			}
			return this;
		}
	}
}
