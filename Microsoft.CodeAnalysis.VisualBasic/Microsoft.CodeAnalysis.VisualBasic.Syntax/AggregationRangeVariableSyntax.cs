using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class AggregationRangeVariableSyntax : VisualBasicSyntaxNode
	{
		internal VariableNameEqualsSyntax _nameEquals;

		internal AggregationSyntax _aggregation;

		public VariableNameEqualsSyntax NameEquals => GetRedAtZero(ref _nameEquals);

		public AggregationSyntax Aggregation => GetRed(ref _aggregation, 1);

		internal AggregationRangeVariableSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal AggregationRangeVariableSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, VariableNameEqualsSyntax nameEquals, AggregationSyntax aggregation)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax(kind, errors, annotations, (nameEquals != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax)nameEquals.Green) : null, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax)aggregation.Green), null, 0)
		{
		}

		public AggregationRangeVariableSyntax WithNameEquals(VariableNameEqualsSyntax nameEquals)
		{
			return Update(nameEquals, Aggregation);
		}

		public AggregationRangeVariableSyntax WithAggregation(AggregationSyntax aggregation)
		{
			return Update(NameEquals, aggregation);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _nameEquals, 
				1 => _aggregation, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => NameEquals, 
				1 => Aggregation, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitAggregationRangeVariable(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitAggregationRangeVariable(this);
		}

		public AggregationRangeVariableSyntax Update(VariableNameEqualsSyntax nameEquals, AggregationSyntax aggregation)
		{
			if (nameEquals != NameEquals || aggregation != Aggregation)
			{
				AggregationRangeVariableSyntax aggregationRangeVariableSyntax = SyntaxFactory.AggregationRangeVariable(nameEquals, aggregation);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(aggregationRangeVariableSyntax, annotations);
				}
				return aggregationRangeVariableSyntax;
			}
			return this;
		}
	}
}
