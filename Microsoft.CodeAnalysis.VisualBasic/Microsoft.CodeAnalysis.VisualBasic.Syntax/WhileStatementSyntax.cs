using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class WhileStatementSyntax : StatementSyntax
	{
		internal ExpressionSyntax _condition;

		public SyntaxToken WhileKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax)base.Green)._whileKeyword, base.Position, 0);

		public ExpressionSyntax Condition => GetRed(ref _condition, 1);

		internal WhileStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal WhileStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax whileKeyword, ExpressionSyntax condition)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax(kind, errors, annotations, whileKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)condition.Green), null, 0)
		{
		}

		public WhileStatementSyntax WithWhileKeyword(SyntaxToken whileKeyword)
		{
			return Update(whileKeyword, Condition);
		}

		public WhileStatementSyntax WithCondition(ExpressionSyntax condition)
		{
			return Update(WhileKeyword, condition);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _condition;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Condition;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitWhileStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitWhileStatement(this);
		}

		public WhileStatementSyntax Update(SyntaxToken whileKeyword, ExpressionSyntax condition)
		{
			if (whileKeyword != WhileKeyword || condition != Condition)
			{
				WhileStatementSyntax whileStatementSyntax = SyntaxFactory.WhileStatement(whileKeyword, condition);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(whileStatementSyntax, annotations);
				}
				return whileStatementSyntax;
			}
			return this;
		}
	}
}
