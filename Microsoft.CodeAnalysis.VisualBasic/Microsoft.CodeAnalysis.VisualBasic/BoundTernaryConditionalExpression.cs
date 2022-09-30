using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundTernaryConditionalExpression : BoundExpression
	{
		private readonly BoundExpression _Condition;

		private readonly BoundExpression _WhenTrue;

		private readonly BoundExpression _WhenFalse;

		private readonly ConstantValue _ConstantValueOpt;

		public BoundExpression Condition => _Condition;

		public BoundExpression WhenTrue => _WhenTrue;

		public BoundExpression WhenFalse => _WhenFalse;

		public override ConstantValue ConstantValueOpt => _ConstantValueOpt;

		public BoundTernaryConditionalExpression(SyntaxNode syntax, BoundExpression condition, BoundExpression whenTrue, BoundExpression whenFalse, ConstantValue constantValueOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.TernaryConditionalExpression, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(condition) || BoundNodeExtensions.NonNullAndHasErrors(whenTrue) || BoundNodeExtensions.NonNullAndHasErrors(whenFalse))
		{
			_Condition = condition;
			_WhenTrue = whenTrue;
			_WhenFalse = whenFalse;
			_ConstantValueOpt = constantValueOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitTernaryConditionalExpression(this);
		}

		public BoundTernaryConditionalExpression Update(BoundExpression condition, BoundExpression whenTrue, BoundExpression whenFalse, ConstantValue constantValueOpt, TypeSymbol type)
		{
			if (condition != Condition || whenTrue != WhenTrue || whenFalse != WhenFalse || (object)constantValueOpt != ConstantValueOpt || (object)type != base.Type)
			{
				BoundTernaryConditionalExpression boundTernaryConditionalExpression = new BoundTernaryConditionalExpression(base.Syntax, condition, whenTrue, whenFalse, constantValueOpt, type, base.HasErrors);
				boundTernaryConditionalExpression.CopyAttributes(this);
				return boundTernaryConditionalExpression;
			}
			return this;
		}
	}
}
