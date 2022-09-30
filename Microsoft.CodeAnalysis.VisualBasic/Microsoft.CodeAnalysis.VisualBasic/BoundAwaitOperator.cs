using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundAwaitOperator : BoundExpression
	{
		private readonly BoundExpression _Operand;

		private readonly BoundRValuePlaceholder _AwaitableInstancePlaceholder;

		private readonly BoundExpression _GetAwaiter;

		private readonly BoundLValuePlaceholder _AwaiterInstancePlaceholder;

		private readonly BoundExpression _IsCompleted;

		private readonly BoundExpression _GetResult;

		public BoundExpression Operand => _Operand;

		public BoundRValuePlaceholder AwaitableInstancePlaceholder => _AwaitableInstancePlaceholder;

		public BoundExpression GetAwaiter => _GetAwaiter;

		public BoundLValuePlaceholder AwaiterInstancePlaceholder => _AwaiterInstancePlaceholder;

		public BoundExpression IsCompleted => _IsCompleted;

		public BoundExpression GetResult => _GetResult;

		public BoundAwaitOperator(SyntaxNode syntax, BoundExpression operand, BoundRValuePlaceholder awaitableInstancePlaceholder, BoundExpression getAwaiter, BoundLValuePlaceholder awaiterInstancePlaceholder, BoundExpression isCompleted, BoundExpression getResult, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.AwaitOperator, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(operand) || BoundNodeExtensions.NonNullAndHasErrors(awaitableInstancePlaceholder) || BoundNodeExtensions.NonNullAndHasErrors(getAwaiter) || BoundNodeExtensions.NonNullAndHasErrors(awaiterInstancePlaceholder) || BoundNodeExtensions.NonNullAndHasErrors(isCompleted) || BoundNodeExtensions.NonNullAndHasErrors(getResult))
		{
			_Operand = operand;
			_AwaitableInstancePlaceholder = awaitableInstancePlaceholder;
			_GetAwaiter = getAwaiter;
			_AwaiterInstancePlaceholder = awaiterInstancePlaceholder;
			_IsCompleted = isCompleted;
			_GetResult = getResult;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitAwaitOperator(this);
		}

		public BoundAwaitOperator Update(BoundExpression operand, BoundRValuePlaceholder awaitableInstancePlaceholder, BoundExpression getAwaiter, BoundLValuePlaceholder awaiterInstancePlaceholder, BoundExpression isCompleted, BoundExpression getResult, TypeSymbol type)
		{
			if (operand != Operand || awaitableInstancePlaceholder != AwaitableInstancePlaceholder || getAwaiter != GetAwaiter || awaiterInstancePlaceholder != AwaiterInstancePlaceholder || isCompleted != IsCompleted || getResult != GetResult || (object)type != base.Type)
			{
				BoundAwaitOperator boundAwaitOperator = new BoundAwaitOperator(base.Syntax, operand, awaitableInstancePlaceholder, getAwaiter, awaiterInstancePlaceholder, isCompleted, getResult, type, base.HasErrors);
				boundAwaitOperator.CopyAttributes(this);
				return boundAwaitOperator;
			}
			return this;
		}
	}
}
