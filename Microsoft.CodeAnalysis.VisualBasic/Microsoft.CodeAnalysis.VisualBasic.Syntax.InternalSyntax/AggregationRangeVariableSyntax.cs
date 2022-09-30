using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class AggregationRangeVariableSyntax : VisualBasicSyntaxNode
	{
		internal readonly VariableNameEqualsSyntax _nameEquals;

		internal readonly AggregationSyntax _aggregation;

		internal static Func<ObjectReader, object> CreateInstance;

		internal VariableNameEqualsSyntax NameEquals => _nameEquals;

		internal AggregationSyntax Aggregation => _aggregation;

		internal AggregationRangeVariableSyntax(SyntaxKind kind, VariableNameEqualsSyntax nameEquals, AggregationSyntax aggregation)
			: base(kind)
		{
			base._slotCount = 2;
			if (nameEquals != null)
			{
				AdjustFlagsAndWidth(nameEquals);
				_nameEquals = nameEquals;
			}
			AdjustFlagsAndWidth(aggregation);
			_aggregation = aggregation;
		}

		internal AggregationRangeVariableSyntax(SyntaxKind kind, VariableNameEqualsSyntax nameEquals, AggregationSyntax aggregation, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			if (nameEquals != null)
			{
				AdjustFlagsAndWidth(nameEquals);
				_nameEquals = nameEquals;
			}
			AdjustFlagsAndWidth(aggregation);
			_aggregation = aggregation;
		}

		internal AggregationRangeVariableSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, VariableNameEqualsSyntax nameEquals, AggregationSyntax aggregation)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			if (nameEquals != null)
			{
				AdjustFlagsAndWidth(nameEquals);
				_nameEquals = nameEquals;
			}
			AdjustFlagsAndWidth(aggregation);
			_aggregation = aggregation;
		}

		internal AggregationRangeVariableSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			VariableNameEqualsSyntax variableNameEqualsSyntax = (VariableNameEqualsSyntax)reader.ReadValue();
			if (variableNameEqualsSyntax != null)
			{
				AdjustFlagsAndWidth(variableNameEqualsSyntax);
				_nameEquals = variableNameEqualsSyntax;
			}
			AggregationSyntax aggregationSyntax = (AggregationSyntax)reader.ReadValue();
			if (aggregationSyntax != null)
			{
				AdjustFlagsAndWidth(aggregationSyntax);
				_aggregation = aggregationSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_nameEquals);
			writer.WriteValue(_aggregation);
		}

		static AggregationRangeVariableSyntax()
		{
			CreateInstance = (ObjectReader o) => new AggregationRangeVariableSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(AggregationRangeVariableSyntax), (ObjectReader r) => new AggregationRangeVariableSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _nameEquals, 
				1 => _aggregation, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new AggregationRangeVariableSyntax(base.Kind, newErrors, GetAnnotations(), _nameEquals, _aggregation);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new AggregationRangeVariableSyntax(base.Kind, GetDiagnostics(), annotations, _nameEquals, _aggregation);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitAggregationRangeVariable(this);
		}
	}
}
