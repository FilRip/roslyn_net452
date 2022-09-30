using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundUserDefinedShortCircuitingOperator : BoundExpression
	{
		private readonly BoundExpression _LeftOperand;

		private readonly BoundRValuePlaceholder _LeftOperandPlaceholder;

		private readonly BoundExpression _LeftTest;

		private readonly BoundUserDefinedBinaryOperator _BitwiseOperator;

		public BoundExpression LeftOperand => _LeftOperand;

		public BoundRValuePlaceholder LeftOperandPlaceholder => _LeftOperandPlaceholder;

		public BoundExpression LeftTest => _LeftTest;

		public BoundUserDefinedBinaryOperator BitwiseOperator => _BitwiseOperator;

		public BoundUserDefinedShortCircuitingOperator(SyntaxNode syntax, BoundExpression leftOperand, BoundRValuePlaceholder leftOperandPlaceholder, BoundExpression leftTest, BoundUserDefinedBinaryOperator bitwiseOperator, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.UserDefinedShortCircuitingOperator, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(leftOperand) || BoundNodeExtensions.NonNullAndHasErrors(leftOperandPlaceholder) || BoundNodeExtensions.NonNullAndHasErrors(leftTest) || BoundNodeExtensions.NonNullAndHasErrors(bitwiseOperator))
		{
			_LeftOperand = leftOperand;
			_LeftOperandPlaceholder = leftOperandPlaceholder;
			_LeftTest = leftTest;
			_BitwiseOperator = bitwiseOperator;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitUserDefinedShortCircuitingOperator(this);
		}

		public BoundUserDefinedShortCircuitingOperator Update(BoundExpression leftOperand, BoundRValuePlaceholder leftOperandPlaceholder, BoundExpression leftTest, BoundUserDefinedBinaryOperator bitwiseOperator, TypeSymbol type)
		{
			if (leftOperand != LeftOperand || leftOperandPlaceholder != LeftOperandPlaceholder || leftTest != LeftTest || bitwiseOperator != BitwiseOperator || (object)type != base.Type)
			{
				BoundUserDefinedShortCircuitingOperator boundUserDefinedShortCircuitingOperator = new BoundUserDefinedShortCircuitingOperator(base.Syntax, leftOperand, leftOperandPlaceholder, leftTest, bitwiseOperator, type, base.HasErrors);
				boundUserDefinedShortCircuitingOperator.CopyAttributes(this);
				return boundUserDefinedShortCircuitingOperator;
			}
			return this;
		}
	}
}
