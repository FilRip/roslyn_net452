using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundRangeVariable : BoundExpression
	{
		private readonly RangeVariableSymbol _RangeVariable;

		public override Symbol ExpressionSymbol => RangeVariable;

		public RangeVariableSymbol RangeVariable => _RangeVariable;

		public BoundRangeVariable(SyntaxNode syntax, RangeVariableSymbol rangeVariable, TypeSymbol type, bool hasErrors)
			: base(BoundKind.RangeVariable, syntax, type, hasErrors)
		{
			_RangeVariable = rangeVariable;
		}

		public BoundRangeVariable(SyntaxNode syntax, RangeVariableSymbol rangeVariable, TypeSymbol type)
			: base(BoundKind.RangeVariable, syntax, type)
		{
			_RangeVariable = rangeVariable;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitRangeVariable(this);
		}

		public BoundRangeVariable Update(RangeVariableSymbol rangeVariable, TypeSymbol type)
		{
			if ((object)rangeVariable != RangeVariable || (object)type != base.Type)
			{
				BoundRangeVariable boundRangeVariable = new BoundRangeVariable(base.Syntax, rangeVariable, type, base.HasErrors);
				boundRangeVariable.CopyAttributes(this);
				return boundRangeVariable;
			}
			return this;
		}
	}
}
