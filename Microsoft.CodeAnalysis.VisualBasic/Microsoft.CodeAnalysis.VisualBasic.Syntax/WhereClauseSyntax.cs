using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class WhereClauseSyntax : QueryClauseSyntax
	{
		internal ExpressionSyntax _condition;

		public SyntaxToken WhereKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax)base.Green)._whereKeyword, base.Position, 0);

		public ExpressionSyntax Condition => GetRed(ref _condition, 1);

		internal WhereClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal WhereClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax whereKeyword, ExpressionSyntax condition)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax(kind, errors, annotations, whereKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)condition.Green), null, 0)
		{
		}

		public WhereClauseSyntax WithWhereKeyword(SyntaxToken whereKeyword)
		{
			return Update(whereKeyword, Condition);
		}

		public WhereClauseSyntax WithCondition(ExpressionSyntax condition)
		{
			return Update(WhereKeyword, condition);
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
			return visitor.VisitWhereClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitWhereClause(this);
		}

		public WhereClauseSyntax Update(SyntaxToken whereKeyword, ExpressionSyntax condition)
		{
			if (whereKeyword != WhereKeyword || condition != Condition)
			{
				WhereClauseSyntax whereClauseSyntax = SyntaxFactory.WhereClause(whereKeyword, condition);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(whereClauseSyntax, annotations);
				}
				return whereClauseSyntax;
			}
			return this;
		}
	}
}
