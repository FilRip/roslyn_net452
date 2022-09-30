using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class DistinctClauseSyntax : QueryClauseSyntax
	{
		public SyntaxToken DistinctKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax)base.Green)._distinctKeyword, base.Position, 0);

		internal DistinctClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal DistinctClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax distinctKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax(kind, errors, annotations, distinctKeyword), null, 0)
		{
		}

		public DistinctClauseSyntax WithDistinctKeyword(SyntaxToken distinctKeyword)
		{
			return Update(distinctKeyword);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitDistinctClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitDistinctClause(this);
		}

		public DistinctClauseSyntax Update(SyntaxToken distinctKeyword)
		{
			if (distinctKeyword != DistinctKeyword)
			{
				DistinctClauseSyntax distinctClauseSyntax = SyntaxFactory.DistinctClause(distinctKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(distinctClauseSyntax, annotations);
				}
				return distinctClauseSyntax;
			}
			return this;
		}
	}
}
