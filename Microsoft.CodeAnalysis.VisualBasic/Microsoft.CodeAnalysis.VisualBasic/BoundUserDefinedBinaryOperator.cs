using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundUserDefinedBinaryOperator : BoundExpression
	{
		private readonly BinaryOperatorKind _OperatorKind;

		private readonly BoundExpression _UnderlyingExpression;

		private readonly bool _Checked;

		public BoundExpression Left => Call.Arguments[0];

		public BoundExpression Right => Call.Arguments[1];

		public BoundCall Call => (BoundCall)UnderlyingExpression;

		public BinaryOperatorKind OperatorKind => _OperatorKind;

		public BoundExpression UnderlyingExpression => _UnderlyingExpression;

		public bool Checked => _Checked;

		public BoundUserDefinedBinaryOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression underlyingExpression, bool @checked, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.UserDefinedBinaryOperator, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(underlyingExpression))
		{
			_OperatorKind = operatorKind;
			_UnderlyingExpression = underlyingExpression;
			_Checked = @checked;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitUserDefinedBinaryOperator(this);
		}

		public BoundUserDefinedBinaryOperator Update(BinaryOperatorKind operatorKind, BoundExpression underlyingExpression, bool @checked, TypeSymbol type)
		{
			if (operatorKind != OperatorKind || underlyingExpression != UnderlyingExpression || @checked != Checked || (object)type != base.Type)
			{
				BoundUserDefinedBinaryOperator boundUserDefinedBinaryOperator = new BoundUserDefinedBinaryOperator(base.Syntax, operatorKind, underlyingExpression, @checked, type, base.HasErrors);
				boundUserDefinedBinaryOperator.CopyAttributes(this);
				return boundUserDefinedBinaryOperator;
			}
			return this;
		}
	}
}
