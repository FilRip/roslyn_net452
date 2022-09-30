using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class GroupJoinClauseSyntax : JoinClauseSyntax
	{
		internal SyntaxNode _aggregationVariables;

		public SyntaxToken GroupKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax)base.Green)._groupKeyword, base.Position, 0);

		public new SyntaxToken JoinKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax)base.Green)._joinKeyword, GetChildPosition(1), GetChildIndex(1));

		public override SeparatedSyntaxList<CollectionRangeVariableSyntax> JoinedVariables
		{
			get
			{
				SyntaxNode red = GetRed(ref _joinedVariables, 2);
				return (red == null) ? default(SeparatedSyntaxList<CollectionRangeVariableSyntax>) : new SeparatedSyntaxList<CollectionRangeVariableSyntax>(red, GetChildIndex(2));
			}
		}

		public new SyntaxList<JoinClauseSyntax> AdditionalJoins
		{
			get
			{
				SyntaxNode red = GetRed(ref _additionalJoins, 3);
				return new SyntaxList<JoinClauseSyntax>(red);
			}
		}

		public new SyntaxToken OnKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax)base.Green)._onKeyword, GetChildPosition(4), GetChildIndex(4));

		public override SeparatedSyntaxList<JoinConditionSyntax> JoinConditions
		{
			get
			{
				SyntaxNode red = GetRed(ref _joinConditions, 5);
				return (red == null) ? default(SeparatedSyntaxList<JoinConditionSyntax>) : new SeparatedSyntaxList<JoinConditionSyntax>(red, GetChildIndex(5));
			}
		}

		public SyntaxToken IntoKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax)base.Green)._intoKeyword, GetChildPosition(6), GetChildIndex(6));

		public SeparatedSyntaxList<AggregationRangeVariableSyntax> AggregationVariables
		{
			get
			{
				SyntaxNode red = GetRed(ref _aggregationVariables, 7);
				return (red == null) ? default(SeparatedSyntaxList<AggregationRangeVariableSyntax>) : new SeparatedSyntaxList<AggregationRangeVariableSyntax>(red, GetChildIndex(7));
			}
		}

		internal GroupJoinClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal GroupJoinClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax groupKeyword, KeywordSyntax joinKeyword, SyntaxNode joinedVariables, SyntaxNode additionalJoins, KeywordSyntax onKeyword, SyntaxNode joinConditions, KeywordSyntax intoKeyword, SyntaxNode aggregationVariables)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax(kind, errors, annotations, groupKeyword, joinKeyword, joinedVariables?.Green, additionalJoins?.Green, onKeyword, joinConditions?.Green, intoKeyword, aggregationVariables?.Green), null, 0)
		{
		}

		public GroupJoinClauseSyntax WithGroupKeyword(SyntaxToken groupKeyword)
		{
			return Update(groupKeyword, JoinKeyword, JoinedVariables, AdditionalJoins, OnKeyword, JoinConditions, IntoKeyword, AggregationVariables);
		}

		internal override SyntaxToken GetJoinKeywordCore()
		{
			return JoinKeyword;
		}

		internal override JoinClauseSyntax WithJoinKeywordCore(SyntaxToken joinKeyword)
		{
			return WithJoinKeyword(joinKeyword);
		}

		public new GroupJoinClauseSyntax WithJoinKeyword(SyntaxToken joinKeyword)
		{
			return Update(GroupKeyword, joinKeyword, JoinedVariables, AdditionalJoins, OnKeyword, JoinConditions, IntoKeyword, AggregationVariables);
		}

		internal override JoinClauseSyntax WithJoinedVariablesCore(SeparatedSyntaxList<CollectionRangeVariableSyntax> joinedVariables)
		{
			return WithJoinedVariables(joinedVariables);
		}

		public new GroupJoinClauseSyntax WithJoinedVariables(SeparatedSyntaxList<CollectionRangeVariableSyntax> joinedVariables)
		{
			return Update(GroupKeyword, JoinKeyword, joinedVariables, AdditionalJoins, OnKeyword, JoinConditions, IntoKeyword, AggregationVariables);
		}

		public new GroupJoinClauseSyntax AddJoinedVariables(params CollectionRangeVariableSyntax[] items)
		{
			return WithJoinedVariables(JoinedVariables.AddRange(items));
		}

		internal override JoinClauseSyntax AddJoinedVariablesCore(params CollectionRangeVariableSyntax[] items)
		{
			return AddJoinedVariables(items);
		}

		internal override SyntaxList<JoinClauseSyntax> GetAdditionalJoinsCore()
		{
			return AdditionalJoins;
		}

		internal override JoinClauseSyntax WithAdditionalJoinsCore(SyntaxList<JoinClauseSyntax> additionalJoins)
		{
			return WithAdditionalJoins(additionalJoins);
		}

		public new GroupJoinClauseSyntax WithAdditionalJoins(SyntaxList<JoinClauseSyntax> additionalJoins)
		{
			return Update(GroupKeyword, JoinKeyword, JoinedVariables, additionalJoins, OnKeyword, JoinConditions, IntoKeyword, AggregationVariables);
		}

		public new GroupJoinClauseSyntax AddAdditionalJoins(params JoinClauseSyntax[] items)
		{
			return WithAdditionalJoins(AdditionalJoins.AddRange(items));
		}

		internal override JoinClauseSyntax AddAdditionalJoinsCore(params JoinClauseSyntax[] items)
		{
			return AddAdditionalJoins(items);
		}

		internal override SyntaxToken GetOnKeywordCore()
		{
			return OnKeyword;
		}

		internal override JoinClauseSyntax WithOnKeywordCore(SyntaxToken onKeyword)
		{
			return WithOnKeyword(onKeyword);
		}

		public new GroupJoinClauseSyntax WithOnKeyword(SyntaxToken onKeyword)
		{
			return Update(GroupKeyword, JoinKeyword, JoinedVariables, AdditionalJoins, onKeyword, JoinConditions, IntoKeyword, AggregationVariables);
		}

		internal override JoinClauseSyntax WithJoinConditionsCore(SeparatedSyntaxList<JoinConditionSyntax> joinConditions)
		{
			return WithJoinConditions(joinConditions);
		}

		public new GroupJoinClauseSyntax WithJoinConditions(SeparatedSyntaxList<JoinConditionSyntax> joinConditions)
		{
			return Update(GroupKeyword, JoinKeyword, JoinedVariables, AdditionalJoins, OnKeyword, joinConditions, IntoKeyword, AggregationVariables);
		}

		public new GroupJoinClauseSyntax AddJoinConditions(params JoinConditionSyntax[] items)
		{
			return WithJoinConditions(JoinConditions.AddRange(items));
		}

		internal override JoinClauseSyntax AddJoinConditionsCore(params JoinConditionSyntax[] items)
		{
			return AddJoinConditions(items);
		}

		public GroupJoinClauseSyntax WithIntoKeyword(SyntaxToken intoKeyword)
		{
			return Update(GroupKeyword, JoinKeyword, JoinedVariables, AdditionalJoins, OnKeyword, JoinConditions, intoKeyword, AggregationVariables);
		}

		public GroupJoinClauseSyntax WithAggregationVariables(SeparatedSyntaxList<AggregationRangeVariableSyntax> aggregationVariables)
		{
			return Update(GroupKeyword, JoinKeyword, JoinedVariables, AdditionalJoins, OnKeyword, JoinConditions, IntoKeyword, aggregationVariables);
		}

		public GroupJoinClauseSyntax AddAggregationVariables(params AggregationRangeVariableSyntax[] items)
		{
			return WithAggregationVariables(AggregationVariables.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				2 => _joinedVariables, 
				3 => _additionalJoins, 
				5 => _joinConditions, 
				7 => _aggregationVariables, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				2 => GetRed(ref _joinedVariables, 2), 
				3 => GetRed(ref _additionalJoins, 3), 
				5 => GetRed(ref _joinConditions, 5), 
				7 => GetRed(ref _aggregationVariables, 7), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitGroupJoinClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitGroupJoinClause(this);
		}

		public GroupJoinClauseSyntax Update(SyntaxToken groupKeyword, SyntaxToken joinKeyword, SeparatedSyntaxList<CollectionRangeVariableSyntax> joinedVariables, SyntaxList<JoinClauseSyntax> additionalJoins, SyntaxToken onKeyword, SeparatedSyntaxList<JoinConditionSyntax> joinConditions, SyntaxToken intoKeyword, SeparatedSyntaxList<AggregationRangeVariableSyntax> aggregationVariables)
		{
			if (groupKeyword != GroupKeyword || joinKeyword != JoinKeyword || joinedVariables != JoinedVariables || additionalJoins != AdditionalJoins || onKeyword != OnKeyword || joinConditions != JoinConditions || intoKeyword != IntoKeyword || aggregationVariables != AggregationVariables)
			{
				GroupJoinClauseSyntax groupJoinClauseSyntax = SyntaxFactory.GroupJoinClause(groupKeyword, joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions, intoKeyword, aggregationVariables);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(groupJoinClauseSyntax, annotations);
				}
				return groupJoinClauseSyntax;
			}
			return this;
		}
	}
}
