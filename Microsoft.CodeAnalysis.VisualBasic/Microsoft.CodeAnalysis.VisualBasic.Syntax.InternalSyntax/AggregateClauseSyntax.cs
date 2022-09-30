using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class AggregateClauseSyntax : QueryClauseSyntax
	{
		internal readonly KeywordSyntax _aggregateKeyword;

		internal readonly GreenNode _variables;

		internal readonly GreenNode _additionalQueryOperators;

		internal readonly KeywordSyntax _intoKeyword;

		internal readonly GreenNode _aggregationVariables;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax AggregateKeyword => _aggregateKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax> Variables => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CollectionRangeVariableSyntax>(_variables));

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<QueryClauseSyntax> AdditionalQueryOperators => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<QueryClauseSyntax>(_additionalQueryOperators);

		internal KeywordSyntax IntoKeyword => _intoKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax> AggregationVariables => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<AggregationRangeVariableSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AggregationRangeVariableSyntax>(_aggregationVariables));

		internal AggregateClauseSyntax(SyntaxKind kind, KeywordSyntax aggregateKeyword, GreenNode variables, GreenNode additionalQueryOperators, KeywordSyntax intoKeyword, GreenNode aggregationVariables)
			: base(kind)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(aggregateKeyword);
			_aggregateKeyword = aggregateKeyword;
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
			if (additionalQueryOperators != null)
			{
				AdjustFlagsAndWidth(additionalQueryOperators);
				_additionalQueryOperators = additionalQueryOperators;
			}
			AdjustFlagsAndWidth(intoKeyword);
			_intoKeyword = intoKeyword;
			if (aggregationVariables != null)
			{
				AdjustFlagsAndWidth(aggregationVariables);
				_aggregationVariables = aggregationVariables;
			}
		}

		internal AggregateClauseSyntax(SyntaxKind kind, KeywordSyntax aggregateKeyword, GreenNode variables, GreenNode additionalQueryOperators, KeywordSyntax intoKeyword, GreenNode aggregationVariables, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(aggregateKeyword);
			_aggregateKeyword = aggregateKeyword;
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
			if (additionalQueryOperators != null)
			{
				AdjustFlagsAndWidth(additionalQueryOperators);
				_additionalQueryOperators = additionalQueryOperators;
			}
			AdjustFlagsAndWidth(intoKeyword);
			_intoKeyword = intoKeyword;
			if (aggregationVariables != null)
			{
				AdjustFlagsAndWidth(aggregationVariables);
				_aggregationVariables = aggregationVariables;
			}
		}

		internal AggregateClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax aggregateKeyword, GreenNode variables, GreenNode additionalQueryOperators, KeywordSyntax intoKeyword, GreenNode aggregationVariables)
			: base(kind, errors, annotations)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(aggregateKeyword);
			_aggregateKeyword = aggregateKeyword;
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
			if (additionalQueryOperators != null)
			{
				AdjustFlagsAndWidth(additionalQueryOperators);
				_additionalQueryOperators = additionalQueryOperators;
			}
			AdjustFlagsAndWidth(intoKeyword);
			_intoKeyword = intoKeyword;
			if (aggregationVariables != null)
			{
				AdjustFlagsAndWidth(aggregationVariables);
				_aggregationVariables = aggregationVariables;
			}
		}

		internal AggregateClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_aggregateKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_variables = greenNode;
			}
			GreenNode greenNode2 = (GreenNode)reader.ReadValue();
			if (greenNode2 != null)
			{
				AdjustFlagsAndWidth(greenNode2);
				_additionalQueryOperators = greenNode2;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_intoKeyword = keywordSyntax2;
			}
			GreenNode greenNode3 = (GreenNode)reader.ReadValue();
			if (greenNode3 != null)
			{
				AdjustFlagsAndWidth(greenNode3);
				_aggregationVariables = greenNode3;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_aggregateKeyword);
			writer.WriteValue(_variables);
			writer.WriteValue(_additionalQueryOperators);
			writer.WriteValue(_intoKeyword);
			writer.WriteValue(_aggregationVariables);
		}

		static AggregateClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new AggregateClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(AggregateClauseSyntax), (ObjectReader r) => new AggregateClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _aggregateKeyword, 
				1 => _variables, 
				2 => _additionalQueryOperators, 
				3 => _intoKeyword, 
				4 => _aggregationVariables, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new AggregateClauseSyntax(base.Kind, newErrors, GetAnnotations(), _aggregateKeyword, _variables, _additionalQueryOperators, _intoKeyword, _aggregationVariables);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new AggregateClauseSyntax(base.Kind, GetDiagnostics(), annotations, _aggregateKeyword, _variables, _additionalQueryOperators, _intoKeyword, _aggregationVariables);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitAggregateClause(this);
		}
	}
}
