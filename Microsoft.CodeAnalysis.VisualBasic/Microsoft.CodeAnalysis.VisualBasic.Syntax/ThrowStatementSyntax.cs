using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ThrowStatementSyntax : ExecutableStatementSyntax
	{
		internal ExpressionSyntax _expression;

		public SyntaxToken ThrowKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax)base.Green)._throwKeyword, base.Position, 0);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		internal ThrowStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ThrowStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax throwKeyword, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax(kind, errors, annotations, throwKeyword, (expression != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green) : null), null, 0)
		{
		}

		public ThrowStatementSyntax WithThrowKeyword(SyntaxToken throwKeyword)
		{
			return Update(throwKeyword, Expression);
		}

		public ThrowStatementSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(ThrowKeyword, expression);
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
			return visitor.VisitThrowStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitThrowStatement(this);
		}

		public ThrowStatementSyntax Update(SyntaxToken throwKeyword, ExpressionSyntax expression)
		{
			if (throwKeyword != ThrowKeyword || expression != Expression)
			{
				ThrowStatementSyntax throwStatementSyntax = SyntaxFactory.ThrowStatement(throwKeyword, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(throwStatementSyntax, annotations);
				}
				return throwStatementSyntax;
			}
			return this;
		}
	}
}
