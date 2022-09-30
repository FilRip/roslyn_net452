using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundRangeCaseClause : BoundCaseClause
	{
		private readonly BoundExpression _LowerBoundOpt;

		private readonly BoundExpression _UpperBoundOpt;

		private readonly BoundExpression _LowerBoundConditionOpt;

		private readonly BoundExpression _UpperBoundConditionOpt;

		public BoundExpression LowerBoundOpt => _LowerBoundOpt;

		public BoundExpression UpperBoundOpt => _UpperBoundOpt;

		public BoundExpression LowerBoundConditionOpt => _LowerBoundConditionOpt;

		public BoundExpression UpperBoundConditionOpt => _UpperBoundConditionOpt;

		public BoundRangeCaseClause(SyntaxNode syntax, BoundExpression lowerBoundOpt, BoundExpression upperBoundOpt, BoundExpression lowerBoundConditionOpt, BoundExpression upperBoundConditionOpt, bool hasErrors = false)
			: base(BoundKind.RangeCaseClause, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(lowerBoundOpt) || BoundNodeExtensions.NonNullAndHasErrors(upperBoundOpt) || BoundNodeExtensions.NonNullAndHasErrors(lowerBoundConditionOpt) || BoundNodeExtensions.NonNullAndHasErrors(upperBoundConditionOpt))
		{
			_LowerBoundOpt = lowerBoundOpt;
			_UpperBoundOpt = upperBoundOpt;
			_LowerBoundConditionOpt = lowerBoundConditionOpt;
			_UpperBoundConditionOpt = upperBoundConditionOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitRangeCaseClause(this);
		}

		public BoundRangeCaseClause Update(BoundExpression lowerBoundOpt, BoundExpression upperBoundOpt, BoundExpression lowerBoundConditionOpt, BoundExpression upperBoundConditionOpt)
		{
			if (lowerBoundOpt != LowerBoundOpt || upperBoundOpt != UpperBoundOpt || lowerBoundConditionOpt != LowerBoundConditionOpt || upperBoundConditionOpt != UpperBoundConditionOpt)
			{
				BoundRangeCaseClause boundRangeCaseClause = new BoundRangeCaseClause(base.Syntax, lowerBoundOpt, upperBoundOpt, lowerBoundConditionOpt, upperBoundConditionOpt, base.HasErrors);
				boundRangeCaseClause.CopyAttributes(this);
				return boundRangeCaseClause;
			}
			return this;
		}
	}
}
