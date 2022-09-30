using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundRangeVariableAssignment : BoundQueryPart
	{
		private readonly RangeVariableSymbol _RangeVariable;

		private readonly BoundExpression _Value;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)Value);

		public RangeVariableSymbol RangeVariable => _RangeVariable;

		public BoundExpression Value => _Value;

		public BoundRangeVariableAssignment(SyntaxNode syntax, RangeVariableSymbol rangeVariable, BoundExpression value, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.RangeVariableAssignment, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(value))
		{
			_RangeVariable = rangeVariable;
			_Value = value;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitRangeVariableAssignment(this);
		}

		public BoundRangeVariableAssignment Update(RangeVariableSymbol rangeVariable, BoundExpression value, TypeSymbol type)
		{
			if ((object)rangeVariable != RangeVariable || value != Value || (object)type != base.Type)
			{
				BoundRangeVariableAssignment boundRangeVariableAssignment = new BoundRangeVariableAssignment(base.Syntax, rangeVariable, value, type, base.HasErrors);
				boundRangeVariableAssignment.CopyAttributes(this);
				return boundRangeVariableAssignment;
			}
			return this;
		}
	}
}
