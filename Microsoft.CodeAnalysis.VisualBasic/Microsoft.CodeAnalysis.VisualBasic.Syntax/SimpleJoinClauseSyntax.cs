using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SimpleJoinClauseSyntax : JoinClauseSyntax
	{
		public new SyntaxToken JoinKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax)base.Green)._joinKeyword, base.Position, 0);

		public override SeparatedSyntaxList<CollectionRangeVariableSyntax> JoinedVariables
		{
			get
			{
				SyntaxNode red = GetRed(ref _joinedVariables, 1);
				return (red == null) ? default(SeparatedSyntaxList<CollectionRangeVariableSyntax>) : new SeparatedSyntaxList<CollectionRangeVariableSyntax>(red, GetChildIndex(1));
			}
		}

		public new SyntaxList<JoinClauseSyntax> AdditionalJoins
		{
			get
			{
				SyntaxNode red = GetRed(ref _additionalJoins, 2);
				return new SyntaxList<JoinClauseSyntax>(red);
			}
		}

		public new SyntaxToken OnKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax)base.Green)._onKeyword, GetChildPosition(3), GetChildIndex(3));

		public override SeparatedSyntaxList<JoinConditionSyntax> JoinConditions
		{
			get
			{
				SyntaxNode red = GetRed(ref _joinConditions, 4);
				return (red == null) ? default(SeparatedSyntaxList<JoinConditionSyntax>) : new SeparatedSyntaxList<JoinConditionSyntax>(red, GetChildIndex(4));
			}
		}

		internal SimpleJoinClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SimpleJoinClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax joinKeyword, SyntaxNode joinedVariables, SyntaxNode additionalJoins, KeywordSyntax onKeyword, SyntaxNode joinConditions)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax(kind, errors, annotations, joinKeyword, joinedVariables?.Green, additionalJoins?.Green, onKeyword, joinConditions?.Green), null, 0)
		{
		}

		internal override SyntaxToken GetJoinKeywordCore()
		{
			return JoinKeyword;
		}

		internal override JoinClauseSyntax WithJoinKeywordCore(SyntaxToken joinKeyword)
		{
			return WithJoinKeyword(joinKeyword);
		}

		public new SimpleJoinClauseSyntax WithJoinKeyword(SyntaxToken joinKeyword)
		{
			return Update(joinKeyword, JoinedVariables, AdditionalJoins, OnKeyword, JoinConditions);
		}

		internal override JoinClauseSyntax WithJoinedVariablesCore(SeparatedSyntaxList<CollectionRangeVariableSyntax> joinedVariables)
		{
			return WithJoinedVariables(joinedVariables);
		}

		public new SimpleJoinClauseSyntax WithJoinedVariables(SeparatedSyntaxList<CollectionRangeVariableSyntax> joinedVariables)
		{
			return Update(JoinKeyword, joinedVariables, AdditionalJoins, OnKeyword, JoinConditions);
		}

		public new SimpleJoinClauseSyntax AddJoinedVariables(params CollectionRangeVariableSyntax[] items)
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

		public new SimpleJoinClauseSyntax WithAdditionalJoins(SyntaxList<JoinClauseSyntax> additionalJoins)
		{
			return Update(JoinKeyword, JoinedVariables, additionalJoins, OnKeyword, JoinConditions);
		}

		public new SimpleJoinClauseSyntax AddAdditionalJoins(params JoinClauseSyntax[] items)
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

		public new SimpleJoinClauseSyntax WithOnKeyword(SyntaxToken onKeyword)
		{
			return Update(JoinKeyword, JoinedVariables, AdditionalJoins, onKeyword, JoinConditions);
		}

		internal override JoinClauseSyntax WithJoinConditionsCore(SeparatedSyntaxList<JoinConditionSyntax> joinConditions)
		{
			return WithJoinConditions(joinConditions);
		}

		public new SimpleJoinClauseSyntax WithJoinConditions(SeparatedSyntaxList<JoinConditionSyntax> joinConditions)
		{
			return Update(JoinKeyword, JoinedVariables, AdditionalJoins, OnKeyword, joinConditions);
		}

		public new SimpleJoinClauseSyntax AddJoinConditions(params JoinConditionSyntax[] items)
		{
			return WithJoinConditions(JoinConditions.AddRange(items));
		}

		internal override JoinClauseSyntax AddJoinConditionsCore(params JoinConditionSyntax[] items)
		{
			return AddJoinConditions(items);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _joinedVariables, 
				2 => _additionalJoins, 
				4 => _joinConditions, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => GetRed(ref _joinedVariables, 1), 
				2 => GetRed(ref _additionalJoins, 2), 
				4 => GetRed(ref _joinConditions, 4), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSimpleJoinClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSimpleJoinClause(this);
		}

		public SimpleJoinClauseSyntax Update(SyntaxToken joinKeyword, SeparatedSyntaxList<CollectionRangeVariableSyntax> joinedVariables, SyntaxList<JoinClauseSyntax> additionalJoins, SyntaxToken onKeyword, SeparatedSyntaxList<JoinConditionSyntax> joinConditions)
		{
			if (joinKeyword != JoinKeyword || joinedVariables != JoinedVariables || additionalJoins != AdditionalJoins || onKeyword != OnKeyword || joinConditions != JoinConditions)
			{
				SimpleJoinClauseSyntax simpleJoinClauseSyntax = SyntaxFactory.SimpleJoinClause(joinKeyword, joinedVariables, additionalJoins, onKeyword, joinConditions);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(simpleJoinClauseSyntax, annotations);
				}
				return simpleJoinClauseSyntax;
			}
			return this;
		}
	}
}
