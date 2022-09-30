using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class JoinClauseSyntax : QueryClauseSyntax
	{
		internal SyntaxNode _joinedVariables;

		internal SyntaxNode _additionalJoins;

		internal SyntaxNode _joinConditions;

		public SyntaxToken JoinKeyword => GetJoinKeywordCore();

		public virtual SeparatedSyntaxList<CollectionRangeVariableSyntax> JoinedVariables
		{
			get
			{
				SyntaxNode red = GetRed(ref _joinedVariables, 1);
				return (red == null) ? default(SeparatedSyntaxList<CollectionRangeVariableSyntax>) : new SeparatedSyntaxList<CollectionRangeVariableSyntax>(red, GetChildIndex(1));
			}
		}

		public SyntaxList<JoinClauseSyntax> AdditionalJoins => GetAdditionalJoinsCore();

		public SyntaxToken OnKeyword => GetOnKeywordCore();

		public virtual SeparatedSyntaxList<JoinConditionSyntax> JoinConditions
		{
			get
			{
				SyntaxNode red = GetRed(ref _joinConditions, 4);
				return (red == null) ? default(SeparatedSyntaxList<JoinConditionSyntax>) : new SeparatedSyntaxList<JoinConditionSyntax>(red, GetChildIndex(4));
			}
		}

		internal JoinClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxToken GetJoinKeywordCore()
		{
			return new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax)base.Green)._joinKeyword, base.Position, 0);
		}

		public JoinClauseSyntax WithJoinKeyword(SyntaxToken joinKeyword)
		{
			return WithJoinKeywordCore(joinKeyword);
		}

		internal abstract JoinClauseSyntax WithJoinKeywordCore(SyntaxToken joinKeyword);

		public JoinClauseSyntax WithJoinedVariables(SeparatedSyntaxList<CollectionRangeVariableSyntax> joinedVariables)
		{
			return WithJoinedVariablesCore(joinedVariables);
		}

		internal abstract JoinClauseSyntax WithJoinedVariablesCore(SeparatedSyntaxList<CollectionRangeVariableSyntax> joinedVariables);

		public JoinClauseSyntax AddJoinedVariables(params CollectionRangeVariableSyntax[] items)
		{
			return AddJoinedVariablesCore(items);
		}

		internal abstract JoinClauseSyntax AddJoinedVariablesCore(params CollectionRangeVariableSyntax[] items);

		internal virtual SyntaxList<JoinClauseSyntax> GetAdditionalJoinsCore()
		{
			SyntaxNode red = GetRed(ref _additionalJoins, 2);
			return new SyntaxList<JoinClauseSyntax>(red);
		}

		public JoinClauseSyntax WithAdditionalJoins(SyntaxList<JoinClauseSyntax> additionalJoins)
		{
			return WithAdditionalJoinsCore(additionalJoins);
		}

		internal abstract JoinClauseSyntax WithAdditionalJoinsCore(SyntaxList<JoinClauseSyntax> additionalJoins);

		public JoinClauseSyntax AddAdditionalJoins(params JoinClauseSyntax[] items)
		{
			return AddAdditionalJoinsCore(items);
		}

		internal abstract JoinClauseSyntax AddAdditionalJoinsCore(params JoinClauseSyntax[] items);

		internal virtual SyntaxToken GetOnKeywordCore()
		{
			return new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax)base.Green)._onKeyword, GetChildPosition(3), GetChildIndex(3));
		}

		public JoinClauseSyntax WithOnKeyword(SyntaxToken onKeyword)
		{
			return WithOnKeywordCore(onKeyword);
		}

		internal abstract JoinClauseSyntax WithOnKeywordCore(SyntaxToken onKeyword);

		public JoinClauseSyntax WithJoinConditions(SeparatedSyntaxList<JoinConditionSyntax> joinConditions)
		{
			return WithJoinConditionsCore(joinConditions);
		}

		internal abstract JoinClauseSyntax WithJoinConditionsCore(SeparatedSyntaxList<JoinConditionSyntax> joinConditions);

		public JoinClauseSyntax AddJoinConditions(params JoinConditionSyntax[] items)
		{
			return AddJoinConditionsCore(items);
		}

		internal abstract JoinClauseSyntax AddJoinConditionsCore(params JoinConditionSyntax[] items);
	}
}
