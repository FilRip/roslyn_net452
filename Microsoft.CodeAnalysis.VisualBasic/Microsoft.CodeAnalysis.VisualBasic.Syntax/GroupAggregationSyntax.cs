using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class GroupAggregationSyntax : AggregationSyntax
	{
		public SyntaxToken GroupKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax)base.Green)._groupKeyword, base.Position, 0);

		internal GroupAggregationSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal GroupAggregationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax groupKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax(kind, errors, annotations, groupKeyword), null, 0)
		{
		}

		public GroupAggregationSyntax WithGroupKeyword(SyntaxToken groupKeyword)
		{
			return Update(groupKeyword);
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
			return visitor.VisitGroupAggregation(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitGroupAggregation(this);
		}

		public GroupAggregationSyntax Update(SyntaxToken groupKeyword)
		{
			if (groupKeyword != GroupKeyword)
			{
				GroupAggregationSyntax groupAggregationSyntax = SyntaxFactory.GroupAggregation(groupKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(groupAggregationSyntax, annotations);
				}
				return groupAggregationSyntax;
			}
			return this;
		}
	}
}
