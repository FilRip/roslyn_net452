using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class DoStatementSyntax : StatementSyntax
	{
		internal WhileOrUntilClauseSyntax _whileOrUntilClause;

		public SyntaxToken DoKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax)base.Green)._doKeyword, base.Position, 0);

		public WhileOrUntilClauseSyntax WhileOrUntilClause => GetRed(ref _whileOrUntilClause, 1);

		internal DoStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal DoStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax doKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(kind, errors, annotations, doKeyword, (whileOrUntilClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)whileOrUntilClause.Green) : null), null, 0)
		{
		}

		public DoStatementSyntax WithDoKeyword(SyntaxToken doKeyword)
		{
			return Update(Kind(), doKeyword, WhileOrUntilClause);
		}

		public DoStatementSyntax WithWhileOrUntilClause(WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			return Update(Kind(), DoKeyword, whileOrUntilClause);
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
			return visitor.VisitDoStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitDoStatement(this);
		}

		public DoStatementSyntax Update(SyntaxKind kind, SyntaxToken doKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			if (kind != Kind() || doKeyword != DoKeyword || whileOrUntilClause != WhileOrUntilClause)
			{
				DoStatementSyntax doStatementSyntax = SyntaxFactory.DoStatement(kind, doKeyword, whileOrUntilClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(doStatementSyntax, annotations);
				}
				return doStatementSyntax;
			}
			return this;
		}
	}
}
