using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class GroupByClauseSyntax : QueryClauseSyntax
	{
		internal SyntaxNode _items;

		internal SyntaxNode _keys;

		internal SyntaxNode _aggregationVariables;

		public SyntaxToken GroupKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax)base.Green)._groupKeyword, base.Position, 0);

		public SeparatedSyntaxList<ExpressionRangeVariableSyntax> Items
		{
			get
			{
				SyntaxNode red = GetRed(ref _items, 1);
				return (red == null) ? default(SeparatedSyntaxList<ExpressionRangeVariableSyntax>) : new SeparatedSyntaxList<ExpressionRangeVariableSyntax>(red, GetChildIndex(1));
			}
		}

		public SyntaxToken ByKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax)base.Green)._byKeyword, GetChildPosition(2), GetChildIndex(2));

		public SeparatedSyntaxList<ExpressionRangeVariableSyntax> Keys
		{
			get
			{
				SyntaxNode red = GetRed(ref _keys, 3);
				return (red == null) ? default(SeparatedSyntaxList<ExpressionRangeVariableSyntax>) : new SeparatedSyntaxList<ExpressionRangeVariableSyntax>(red, GetChildIndex(3));
			}
		}

		public SyntaxToken IntoKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax)base.Green)._intoKeyword, GetChildPosition(4), GetChildIndex(4));

		public SeparatedSyntaxList<AggregationRangeVariableSyntax> AggregationVariables
		{
			get
			{
				SyntaxNode red = GetRed(ref _aggregationVariables, 5);
				return (red == null) ? default(SeparatedSyntaxList<AggregationRangeVariableSyntax>) : new SeparatedSyntaxList<AggregationRangeVariableSyntax>(red, GetChildIndex(5));
			}
		}

		internal GroupByClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal GroupByClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax groupKeyword, SyntaxNode items, KeywordSyntax byKeyword, SyntaxNode keys, KeywordSyntax intoKeyword, SyntaxNode aggregationVariables)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax(kind, errors, annotations, groupKeyword, items?.Green, byKeyword, keys?.Green, intoKeyword, aggregationVariables?.Green), null, 0)
		{
		}

		public GroupByClauseSyntax WithGroupKeyword(SyntaxToken groupKeyword)
		{
			return Update(groupKeyword, Items, ByKeyword, Keys, IntoKeyword, AggregationVariables);
		}

		public GroupByClauseSyntax WithItems(SeparatedSyntaxList<ExpressionRangeVariableSyntax> items)
		{
			return Update(GroupKeyword, items, ByKeyword, Keys, IntoKeyword, AggregationVariables);
		}

		public GroupByClauseSyntax AddItems(params ExpressionRangeVariableSyntax[] items)
		{
			return WithItems(Items.AddRange(items));
		}

		public GroupByClauseSyntax WithByKeyword(SyntaxToken byKeyword)
		{
			return Update(GroupKeyword, Items, byKeyword, Keys, IntoKeyword, AggregationVariables);
		}

		public GroupByClauseSyntax WithKeys(SeparatedSyntaxList<ExpressionRangeVariableSyntax> keys)
		{
			return Update(GroupKeyword, Items, ByKeyword, keys, IntoKeyword, AggregationVariables);
		}

		public GroupByClauseSyntax AddKeys(params ExpressionRangeVariableSyntax[] items)
		{
			return WithKeys(Keys.AddRange(items));
		}

		public GroupByClauseSyntax WithIntoKeyword(SyntaxToken intoKeyword)
		{
			return Update(GroupKeyword, Items, ByKeyword, Keys, intoKeyword, AggregationVariables);
		}

		public GroupByClauseSyntax WithAggregationVariables(SeparatedSyntaxList<AggregationRangeVariableSyntax> aggregationVariables)
		{
			return Update(GroupKeyword, Items, ByKeyword, Keys, IntoKeyword, aggregationVariables);
		}

		public GroupByClauseSyntax AddAggregationVariables(params AggregationRangeVariableSyntax[] items)
		{
			return WithAggregationVariables(AggregationVariables.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _items, 
				3 => _keys, 
				5 => _aggregationVariables, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => GetRed(ref _items, 1), 
				3 => GetRed(ref _keys, 3), 
				5 => GetRed(ref _aggregationVariables, 5), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitGroupByClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitGroupByClause(this);
		}

		public GroupByClauseSyntax Update(SyntaxToken groupKeyword, SeparatedSyntaxList<ExpressionRangeVariableSyntax> items, SyntaxToken byKeyword, SeparatedSyntaxList<ExpressionRangeVariableSyntax> keys, SyntaxToken intoKeyword, SeparatedSyntaxList<AggregationRangeVariableSyntax> aggregationVariables)
		{
			if (groupKeyword != GroupKeyword || items != Items || byKeyword != ByKeyword || keys != Keys || intoKeyword != IntoKeyword || aggregationVariables != AggregationVariables)
			{
				GroupByClauseSyntax groupByClauseSyntax = SyntaxFactory.GroupByClause(groupKeyword, items, byKeyword, keys, intoKeyword, aggregationVariables);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(groupByClauseSyntax, annotations);
				}
				return groupByClauseSyntax;
			}
			return this;
		}
	}
}
