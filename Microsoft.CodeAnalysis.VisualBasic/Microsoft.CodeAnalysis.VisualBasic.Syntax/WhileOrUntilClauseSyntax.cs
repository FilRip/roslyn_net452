using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class WhileOrUntilClauseSyntax : VisualBasicSyntaxNode
	{
		internal ExpressionSyntax _condition;

		public SyntaxToken WhileOrUntilKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)base.Green)._whileOrUntilKeyword, base.Position, 0);

		public ExpressionSyntax Condition => GetRed(ref _condition, 1);

		internal WhileOrUntilClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal WhileOrUntilClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax whileOrUntilKeyword, ExpressionSyntax condition)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax(kind, errors, annotations, whileOrUntilKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)condition.Green), null, 0)
		{
		}

		public WhileOrUntilClauseSyntax WithWhileOrUntilKeyword(SyntaxToken whileOrUntilKeyword)
		{
			return Update(Kind(), whileOrUntilKeyword, Condition);
		}

		public WhileOrUntilClauseSyntax WithCondition(ExpressionSyntax condition)
		{
			return Update(Kind(), WhileOrUntilKeyword, condition);
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
			return visitor.VisitWhileOrUntilClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitWhileOrUntilClause(this);
		}

		public WhileOrUntilClauseSyntax Update(SyntaxKind kind, SyntaxToken whileOrUntilKeyword, ExpressionSyntax condition)
		{
			if (kind != Kind() || whileOrUntilKeyword != WhileOrUntilKeyword || condition != Condition)
			{
				WhileOrUntilClauseSyntax whileOrUntilClauseSyntax = SyntaxFactory.WhileOrUntilClause(kind, whileOrUntilKeyword, condition);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(whileOrUntilClauseSyntax, annotations);
				}
				return whileOrUntilClauseSyntax;
			}
			return this;
		}
	}
}
