using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class AggregateClauseSyntax : QueryClauseSyntax
	{
		internal SyntaxNode _variables;

		internal SyntaxNode _additionalQueryOperators;

		internal SyntaxNode _aggregationVariables;

		public SyntaxToken AggregateKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax)base.Green)._aggregateKeyword, base.Position, 0);

		public SeparatedSyntaxList<CollectionRangeVariableSyntax> Variables
		{
			get
			{
				SyntaxNode red = GetRed(ref _variables, 1);
				return (red == null) ? default(SeparatedSyntaxList<CollectionRangeVariableSyntax>) : new SeparatedSyntaxList<CollectionRangeVariableSyntax>(red, GetChildIndex(1));
			}
		}

		public SyntaxList<QueryClauseSyntax> AdditionalQueryOperators
		{
			get
			{
				SyntaxNode red = GetRed(ref _additionalQueryOperators, 2);
				return new SyntaxList<QueryClauseSyntax>(red);
			}
		}

		public SyntaxToken IntoKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax)base.Green)._intoKeyword, GetChildPosition(3), GetChildIndex(3));

		public SeparatedSyntaxList<AggregationRangeVariableSyntax> AggregationVariables
		{
			get
			{
				SyntaxNode red = GetRed(ref _aggregationVariables, 4);
				return (red == null) ? default(SeparatedSyntaxList<AggregationRangeVariableSyntax>) : new SeparatedSyntaxList<AggregationRangeVariableSyntax>(red, GetChildIndex(4));
			}
		}

		internal AggregateClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal AggregateClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax aggregateKeyword, SyntaxNode variables, SyntaxNode additionalQueryOperators, KeywordSyntax intoKeyword, SyntaxNode aggregationVariables)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax(kind, errors, annotations, aggregateKeyword, variables?.Green, additionalQueryOperators?.Green, intoKeyword, aggregationVariables?.Green), null, 0)
		{
		}

		public AggregateClauseSyntax WithAggregateKeyword(SyntaxToken aggregateKeyword)
		{
			return Update(aggregateKeyword, Variables, AdditionalQueryOperators, IntoKeyword, AggregationVariables);
		}

		public AggregateClauseSyntax WithVariables(SeparatedSyntaxList<CollectionRangeVariableSyntax> variables)
		{
			return Update(AggregateKeyword, variables, AdditionalQueryOperators, IntoKeyword, AggregationVariables);
		}

		public AggregateClauseSyntax AddVariables(params CollectionRangeVariableSyntax[] items)
		{
			return WithVariables(Variables.AddRange(items));
		}

		public AggregateClauseSyntax WithAdditionalQueryOperators(SyntaxList<QueryClauseSyntax> additionalQueryOperators)
		{
			return Update(AggregateKeyword, Variables, additionalQueryOperators, IntoKeyword, AggregationVariables);
		}

		public AggregateClauseSyntax AddAdditionalQueryOperators(params QueryClauseSyntax[] items)
		{
			return WithAdditionalQueryOperators(AdditionalQueryOperators.AddRange(items));
		}

		public AggregateClauseSyntax WithIntoKeyword(SyntaxToken intoKeyword)
		{
			return Update(AggregateKeyword, Variables, AdditionalQueryOperators, intoKeyword, AggregationVariables);
		}

		public AggregateClauseSyntax WithAggregationVariables(SeparatedSyntaxList<AggregationRangeVariableSyntax> aggregationVariables)
		{
			return Update(AggregateKeyword, Variables, AdditionalQueryOperators, IntoKeyword, aggregationVariables);
		}

		public AggregateClauseSyntax AddAggregationVariables(params AggregationRangeVariableSyntax[] items)
		{
			return WithAggregationVariables(AggregationVariables.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _variables, 
				2 => _additionalQueryOperators, 
				4 => _aggregationVariables, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => GetRed(ref _variables, 1), 
				2 => GetRed(ref _additionalQueryOperators, 2), 
				4 => GetRed(ref _aggregationVariables, 4), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitAggregateClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitAggregateClause(this);
		}

		public AggregateClauseSyntax Update(SyntaxToken aggregateKeyword, SeparatedSyntaxList<CollectionRangeVariableSyntax> variables, SyntaxList<QueryClauseSyntax> additionalQueryOperators, SyntaxToken intoKeyword, SeparatedSyntaxList<AggregationRangeVariableSyntax> aggregationVariables)
		{
			if (aggregateKeyword != AggregateKeyword || variables != Variables || additionalQueryOperators != AdditionalQueryOperators || intoKeyword != IntoKeyword || aggregationVariables != AggregationVariables)
			{
				AggregateClauseSyntax aggregateClauseSyntax = SyntaxFactory.AggregateClause(aggregateKeyword, variables, additionalQueryOperators, intoKeyword, aggregationVariables);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(aggregateClauseSyntax, annotations);
				}
				return aggregateClauseSyntax;
			}
			return this;
		}
	}
}
