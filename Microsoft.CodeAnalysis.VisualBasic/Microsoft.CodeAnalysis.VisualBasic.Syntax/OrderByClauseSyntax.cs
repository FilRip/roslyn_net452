using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class OrderByClauseSyntax : QueryClauseSyntax
	{
		internal SyntaxNode _orderings;

		public SyntaxToken OrderKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax)base.Green)._orderKeyword, base.Position, 0);

		public SyntaxToken ByKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax)base.Green)._byKeyword, GetChildPosition(1), GetChildIndex(1));

		public SeparatedSyntaxList<OrderingSyntax> Orderings
		{
			get
			{
				SyntaxNode red = GetRed(ref _orderings, 2);
				return (red == null) ? default(SeparatedSyntaxList<OrderingSyntax>) : new SeparatedSyntaxList<OrderingSyntax>(red, GetChildIndex(2));
			}
		}

		internal OrderByClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal OrderByClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax orderKeyword, KeywordSyntax byKeyword, SyntaxNode orderings)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax(kind, errors, annotations, orderKeyword, byKeyword, orderings?.Green), null, 0)
		{
		}

		public OrderByClauseSyntax WithOrderKeyword(SyntaxToken orderKeyword)
		{
			return Update(orderKeyword, ByKeyword, Orderings);
		}

		public OrderByClauseSyntax WithByKeyword(SyntaxToken byKeyword)
		{
			return Update(OrderKeyword, byKeyword, Orderings);
		}

		public OrderByClauseSyntax WithOrderings(SeparatedSyntaxList<OrderingSyntax> orderings)
		{
			return Update(OrderKeyword, ByKeyword, orderings);
		}

		public OrderByClauseSyntax AddOrderings(params OrderingSyntax[] items)
		{
			return WithOrderings(Orderings.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _orderings;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return GetRed(ref _orderings, 2);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitOrderByClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitOrderByClause(this);
		}

		public OrderByClauseSyntax Update(SyntaxToken orderKeyword, SyntaxToken byKeyword, SeparatedSyntaxList<OrderingSyntax> orderings)
		{
			if (orderKeyword != OrderKeyword || byKeyword != ByKeyword || orderings != Orderings)
			{
				OrderByClauseSyntax orderByClauseSyntax = SyntaxFactory.OrderByClause(orderKeyword, byKeyword, orderings);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(orderByClauseSyntax, annotations);
				}
				return orderByClauseSyntax;
			}
			return this;
		}
	}
}
