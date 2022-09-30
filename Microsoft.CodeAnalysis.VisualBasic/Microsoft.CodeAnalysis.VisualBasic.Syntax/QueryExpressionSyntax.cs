using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class QueryExpressionSyntax : ExpressionSyntax
	{
		internal SyntaxNode _clauses;

		public SyntaxList<QueryClauseSyntax> Clauses
		{
			get
			{
				SyntaxNode redAtZero = GetRedAtZero(ref _clauses);
				return new SyntaxList<QueryClauseSyntax>(redAtZero);
			}
		}

		internal QueryExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal QueryExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode clauses)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax(kind, errors, annotations, clauses?.Green), null, 0)
		{
		}

		public QueryExpressionSyntax WithClauses(SyntaxList<QueryClauseSyntax> clauses)
		{
			return Update(clauses);
		}

		public QueryExpressionSyntax AddClauses(params QueryClauseSyntax[] items)
		{
			return WithClauses(Clauses.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 0)
			{
				return _clauses;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 0)
			{
				return GetRedAtZero(ref _clauses);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitQueryExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitQueryExpression(this);
		}

		public QueryExpressionSyntax Update(SyntaxList<QueryClauseSyntax> clauses)
		{
			if (clauses != Clauses)
			{
				QueryExpressionSyntax queryExpressionSyntax = SyntaxFactory.QueryExpression(clauses);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(queryExpressionSyntax, annotations);
				}
				return queryExpressionSyntax;
			}
			return this;
		}
	}
}
