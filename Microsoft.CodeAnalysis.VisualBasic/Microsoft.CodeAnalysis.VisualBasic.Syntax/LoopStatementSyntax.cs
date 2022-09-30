using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class LoopStatementSyntax : StatementSyntax
	{
		internal WhileOrUntilClauseSyntax _whileOrUntilClause;

		public SyntaxToken LoopKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)base.Green)._loopKeyword, base.Position, 0);

		public WhileOrUntilClauseSyntax WhileOrUntilClause => GetRed(ref _whileOrUntilClause, 1);

		internal LoopStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal LoopStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax loopKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax(kind, errors, annotations, loopKeyword, (whileOrUntilClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)whileOrUntilClause.Green) : null), null, 0)
		{
		}

		public LoopStatementSyntax WithLoopKeyword(SyntaxToken loopKeyword)
		{
			return Update(Kind(), loopKeyword, WhileOrUntilClause);
		}

		public LoopStatementSyntax WithWhileOrUntilClause(WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			return Update(Kind(), LoopKeyword, whileOrUntilClause);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _whileOrUntilClause;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return WhileOrUntilClause;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitLoopStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitLoopStatement(this);
		}

		public LoopStatementSyntax Update(SyntaxKind kind, SyntaxToken loopKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			if (kind != Kind() || loopKeyword != LoopKeyword || whileOrUntilClause != WhileOrUntilClause)
			{
				LoopStatementSyntax loopStatementSyntax = SyntaxFactory.LoopStatement(kind, loopKeyword, whileOrUntilClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(loopStatementSyntax, annotations);
				}
				return loopStatementSyntax;
			}
			return this;
		}
	}
}
