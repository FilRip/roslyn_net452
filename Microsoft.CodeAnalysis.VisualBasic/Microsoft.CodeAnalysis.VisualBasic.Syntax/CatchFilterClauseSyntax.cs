using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class CatchFilterClauseSyntax : VisualBasicSyntaxNode
	{
		internal ExpressionSyntax _filter;

		public SyntaxToken WhenKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax)base.Green)._whenKeyword, base.Position, 0);

		public ExpressionSyntax Filter => GetRed(ref _filter, 1);

		internal CatchFilterClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal CatchFilterClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax whenKeyword, ExpressionSyntax filter)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax(kind, errors, annotations, whenKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)filter.Green), null, 0)
		{
		}

		public CatchFilterClauseSyntax WithWhenKeyword(SyntaxToken whenKeyword)
		{
			return Update(whenKeyword, Filter);
		}

		public CatchFilterClauseSyntax WithFilter(ExpressionSyntax filter)
		{
			return Update(WhenKeyword, filter);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _filter;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Filter;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitCatchFilterClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitCatchFilterClause(this);
		}

		public CatchFilterClauseSyntax Update(SyntaxToken whenKeyword, ExpressionSyntax filter)
		{
			if (whenKeyword != WhenKeyword || filter != Filter)
			{
				CatchFilterClauseSyntax catchFilterClauseSyntax = SyntaxFactory.CatchFilterClause(whenKeyword, filter);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(catchFilterClauseSyntax, annotations);
				}
				return catchFilterClauseSyntax;
			}
			return this;
		}
	}
}
