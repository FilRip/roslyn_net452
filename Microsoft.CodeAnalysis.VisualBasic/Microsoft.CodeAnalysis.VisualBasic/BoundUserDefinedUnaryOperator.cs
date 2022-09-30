using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundUserDefinedUnaryOperator : BoundExpression
	{
		private readonly UnaryOperatorKind _OperatorKind;

		private readonly BoundExpression _UnderlyingExpression;

		public BoundExpression Operand => Call.Arguments[0];

		public BoundCall Call => (BoundCall)UnderlyingExpression;

		public UnaryOperatorKind OperatorKind => _OperatorKind;

		public BoundExpression UnderlyingExpression => _UnderlyingExpression;

		public BoundUserDefinedUnaryOperator(SyntaxNode syntax, UnaryOperatorKind operatorKind, BoundExpression underlyingExpression, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.UserDefinedUnaryOperator, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(underlyingExpression))
		{
			_OperatorKind = operatorKind;
			_UnderlyingExpression = underlyingExpression;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitUserDefinedUnaryOperator(this);
		}

		public BoundUserDefinedUnaryOperator Update(UnaryOperatorKind operatorKind, BoundExpression underlyingExpression, TypeSymbol type)
		{
			if (operatorKind != OperatorKind || underlyingExpression != UnderlyingExpression || (object)type != base.Type)
			{
				BoundUserDefinedUnaryOperator boundUserDefinedUnaryOperator = new BoundUserDefinedUnaryOperator(base.Syntax, operatorKind, underlyingExpression, type, base.HasErrors);
				boundUserDefinedUnaryOperator.CopyAttributes(this);
				return boundUserDefinedUnaryOperator;
			}
			return this;
		}
	}
}
