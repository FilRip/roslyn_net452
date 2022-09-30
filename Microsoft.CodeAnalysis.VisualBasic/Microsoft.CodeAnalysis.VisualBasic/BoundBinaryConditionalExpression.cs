using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundBinaryConditionalExpression : BoundExpression
	{
		private readonly BoundExpression _TestExpression;

		private readonly BoundExpression _ConvertedTestExpression;

		private readonly BoundRValuePlaceholder _TestExpressionPlaceholder;

		private readonly BoundExpression _ElseExpression;

		private readonly ConstantValue _ConstantValueOpt;

		public BoundExpression TestExpression => _TestExpression;

		public BoundExpression ConvertedTestExpression => _ConvertedTestExpression;

		public BoundRValuePlaceholder TestExpressionPlaceholder => _TestExpressionPlaceholder;

		public BoundExpression ElseExpression => _ElseExpression;

		public override ConstantValue ConstantValueOpt => _ConstantValueOpt;

		public BoundBinaryConditionalExpression(SyntaxNode syntax, BoundExpression testExpression, BoundExpression convertedTestExpression, BoundRValuePlaceholder testExpressionPlaceholder, BoundExpression elseExpression, ConstantValue constantValueOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.BinaryConditionalExpression, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(testExpression) || BoundNodeExtensions.NonNullAndHasErrors(convertedTestExpression) || BoundNodeExtensions.NonNullAndHasErrors(testExpressionPlaceholder) || BoundNodeExtensions.NonNullAndHasErrors(elseExpression))
		{
			_TestExpression = testExpression;
			_ConvertedTestExpression = convertedTestExpression;
			_TestExpressionPlaceholder = testExpressionPlaceholder;
			_ElseExpression = elseExpression;
			_ConstantValueOpt = constantValueOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitBinaryConditionalExpression(this);
		}

		public BoundBinaryConditionalExpression Update(BoundExpression testExpression, BoundExpression convertedTestExpression, BoundRValuePlaceholder testExpressionPlaceholder, BoundExpression elseExpression, ConstantValue constantValueOpt, TypeSymbol type)
		{
			if (testExpression != TestExpression || convertedTestExpression != ConvertedTestExpression || testExpressionPlaceholder != TestExpressionPlaceholder || elseExpression != ElseExpression || (object)constantValueOpt != ConstantValueOpt || (object)type != base.Type)
			{
				BoundBinaryConditionalExpression boundBinaryConditionalExpression = new BoundBinaryConditionalExpression(base.Syntax, testExpression, convertedTestExpression, testExpressionPlaceholder, elseExpression, constantValueOpt, type, base.HasErrors);
				boundBinaryConditionalExpression.CopyAttributes(this);
				return boundBinaryConditionalExpression;
			}
			return this;
		}
	}
}
