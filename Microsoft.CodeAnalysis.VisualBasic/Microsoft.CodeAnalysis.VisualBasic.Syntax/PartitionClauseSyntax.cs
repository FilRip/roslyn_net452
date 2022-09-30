using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class PartitionClauseSyntax : QueryClauseSyntax
	{
		internal ExpressionSyntax _count;

		public SyntaxToken SkipOrTakeKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax)base.Green)._skipOrTakeKeyword, base.Position, 0);

		public ExpressionSyntax Count => GetRed(ref _count, 1);

		internal PartitionClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal PartitionClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax skipOrTakeKeyword, ExpressionSyntax count)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax(kind, errors, annotations, skipOrTakeKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)count.Green), null, 0)
		{
		}

		public PartitionClauseSyntax WithSkipOrTakeKeyword(SyntaxToken skipOrTakeKeyword)
		{
			return Update(Kind(), skipOrTakeKeyword, Count);
		}

		public PartitionClauseSyntax WithCount(ExpressionSyntax count)
		{
			return Update(Kind(), SkipOrTakeKeyword, count);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _count;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Count;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitPartitionClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitPartitionClause(this);
		}

		public PartitionClauseSyntax Update(SyntaxKind kind, SyntaxToken skipOrTakeKeyword, ExpressionSyntax count)
		{
			if (kind != Kind() || skipOrTakeKeyword != SkipOrTakeKeyword || count != Count)
			{
				PartitionClauseSyntax partitionClauseSyntax = SyntaxFactory.PartitionClause(kind, skipOrTakeKeyword, count);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(partitionClauseSyntax, annotations);
				}
				return partitionClauseSyntax;
			}
			return this;
		}
	}
}
