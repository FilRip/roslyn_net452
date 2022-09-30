using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundSimpleCaseClause : BoundSingleValueCaseClause
	{
		public BoundSimpleCaseClause(SyntaxNode syntax, BoundExpression valueOpt, BoundExpression conditionOpt, bool hasErrors = false)
			: base(BoundKind.SimpleCaseClause, syntax, valueOpt, conditionOpt, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(valueOpt) || BoundNodeExtensions.NonNullAndHasErrors(conditionOpt))
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitSimpleCaseClause(this);
		}

		public BoundSimpleCaseClause Update(BoundExpression valueOpt, BoundExpression conditionOpt)
		{
			if (valueOpt != base.ValueOpt || conditionOpt != base.ConditionOpt)
			{
				BoundSimpleCaseClause boundSimpleCaseClause = new BoundSimpleCaseClause(base.Syntax, valueOpt, conditionOpt, base.HasErrors);
				boundSimpleCaseClause.CopyAttributes(this);
				return boundSimpleCaseClause;
			}
			return this;
		}
	}
}
