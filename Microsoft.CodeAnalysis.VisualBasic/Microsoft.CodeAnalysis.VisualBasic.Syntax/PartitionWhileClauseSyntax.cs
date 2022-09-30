using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class PartitionWhileClauseSyntax : QueryClauseSyntax
	{
		internal ExpressionSyntax _condition;

		public SyntaxToken SkipOrTakeKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax)base.Green)._skipOrTakeKeyword, base.Position, 0);

		public SyntaxToken WhileKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax)base.Green)._whileKeyword, GetChildPosition(1), GetChildIndex(1));

		public ExpressionSyntax Condition => GetRed(ref _condition, 2);

		internal PartitionWhileClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal PartitionWhileClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax skipOrTakeKeyword, KeywordSyntax whileKeyword, ExpressionSyntax condition)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax(kind, errors, annotations, skipOrTakeKeyword, whileKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)condition.Green), null, 0)
		{
		}

		public PartitionWhileClauseSyntax WithSkipOrTakeKeyword(SyntaxToken skipOrTakeKeyword)
		{
			return Update(Kind(), skipOrTakeKeyword, WhileKeyword, Condition);
		}

		public PartitionWhileClauseSyntax WithWhileKeyword(SyntaxToken whileKeyword)
		{
			return Update(Kind(), SkipOrTakeKeyword, whileKeyword, Condition);
		}

		public PartitionWhileClauseSyntax WithCondition(ExpressionSyntax condition)
		{
			return Update(Kind(), SkipOrTakeKeyword, WhileKeyword, condition);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _condition;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return Condition;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitPartitionWhileClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitPartitionWhileClause(this);
		}

		public PartitionWhileClauseSyntax Update(SyntaxKind kind, SyntaxToken skipOrTakeKeyword, SyntaxToken whileKeyword, ExpressionSyntax condition)
		{
			if (kind != Kind() || skipOrTakeKeyword != SkipOrTakeKeyword || whileKeyword != WhileKeyword || condition != Condition)
			{
				PartitionWhileClauseSyntax partitionWhileClauseSyntax = SyntaxFactory.PartitionWhileClause(kind, skipOrTakeKeyword, whileKeyword, condition);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(partitionWhileClauseSyntax, annotations);
				}
				return partitionWhileClauseSyntax;
			}
			return this;
		}
	}
}
