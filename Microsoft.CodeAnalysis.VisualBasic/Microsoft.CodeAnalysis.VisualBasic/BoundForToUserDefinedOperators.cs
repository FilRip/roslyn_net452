using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundForToUserDefinedOperators : BoundNode
	{
		private readonly BoundRValuePlaceholder _LeftOperandPlaceholder;

		private readonly BoundRValuePlaceholder _RightOperandPlaceholder;

		private readonly BoundUserDefinedBinaryOperator _Addition;

		private readonly BoundUserDefinedBinaryOperator _Subtraction;

		private readonly BoundExpression _LessThanOrEqual;

		private readonly BoundExpression _GreaterThanOrEqual;

		public BoundRValuePlaceholder LeftOperandPlaceholder => _LeftOperandPlaceholder;

		public BoundRValuePlaceholder RightOperandPlaceholder => _RightOperandPlaceholder;

		public BoundUserDefinedBinaryOperator Addition => _Addition;

		public BoundUserDefinedBinaryOperator Subtraction => _Subtraction;

		public BoundExpression LessThanOrEqual => _LessThanOrEqual;

		public BoundExpression GreaterThanOrEqual => _GreaterThanOrEqual;

		public BoundForToUserDefinedOperators(SyntaxNode syntax, BoundRValuePlaceholder leftOperandPlaceholder, BoundRValuePlaceholder rightOperandPlaceholder, BoundUserDefinedBinaryOperator addition, BoundUserDefinedBinaryOperator subtraction, BoundExpression lessThanOrEqual, BoundExpression greaterThanOrEqual, bool hasErrors = false)
			: base(BoundKind.ForToUserDefinedOperators, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(leftOperandPlaceholder) || BoundNodeExtensions.NonNullAndHasErrors(rightOperandPlaceholder) || BoundNodeExtensions.NonNullAndHasErrors(addition) || BoundNodeExtensions.NonNullAndHasErrors(subtraction) || BoundNodeExtensions.NonNullAndHasErrors(lessThanOrEqual) || BoundNodeExtensions.NonNullAndHasErrors(greaterThanOrEqual))
		{
			_LeftOperandPlaceholder = leftOperandPlaceholder;
			_RightOperandPlaceholder = rightOperandPlaceholder;
			_Addition = addition;
			_Subtraction = subtraction;
			_LessThanOrEqual = lessThanOrEqual;
			_GreaterThanOrEqual = greaterThanOrEqual;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitForToUserDefinedOperators(this);
		}

		public BoundForToUserDefinedOperators Update(BoundRValuePlaceholder leftOperandPlaceholder, BoundRValuePlaceholder rightOperandPlaceholder, BoundUserDefinedBinaryOperator addition, BoundUserDefinedBinaryOperator subtraction, BoundExpression lessThanOrEqual, BoundExpression greaterThanOrEqual)
		{
			if (leftOperandPlaceholder != LeftOperandPlaceholder || rightOperandPlaceholder != RightOperandPlaceholder || addition != Addition || subtraction != Subtraction || lessThanOrEqual != LessThanOrEqual || greaterThanOrEqual != GreaterThanOrEqual)
			{
				BoundForToUserDefinedOperators boundForToUserDefinedOperators = new BoundForToUserDefinedOperators(base.Syntax, leftOperandPlaceholder, rightOperandPlaceholder, addition, subtraction, lessThanOrEqual, greaterThanOrEqual, base.HasErrors);
				boundForToUserDefinedOperators.CopyAttributes(this);
				return boundForToUserDefinedOperators;
			}
			return this;
		}
	}
}
