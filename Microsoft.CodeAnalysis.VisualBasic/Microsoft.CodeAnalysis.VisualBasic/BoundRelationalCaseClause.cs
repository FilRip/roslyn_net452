using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundRelationalCaseClause : BoundSingleValueCaseClause
	{
		private readonly BinaryOperatorKind _OperatorKind;

		public BinaryOperatorKind OperatorKind => _OperatorKind;

		public BoundRelationalCaseClause(SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression valueOpt, BoundExpression conditionOpt, bool hasErrors = false)
			: base(BoundKind.RelationalCaseClause, syntax, valueOpt, conditionOpt, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(valueOpt) || BoundNodeExtensions.NonNullAndHasErrors(conditionOpt))
		{
			_OperatorKind = operatorKind;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitRelationalCaseClause(this);
		}

		public BoundRelationalCaseClause Update(BinaryOperatorKind operatorKind, BoundExpression valueOpt, BoundExpression conditionOpt)
		{
			if (operatorKind != OperatorKind || valueOpt != base.ValueOpt || conditionOpt != base.ConditionOpt)
			{
				BoundRelationalCaseClause boundRelationalCaseClause = new BoundRelationalCaseClause(base.Syntax, operatorKind, valueOpt, conditionOpt, base.HasErrors);
				boundRelationalCaseClause.CopyAttributes(this);
				return boundRelationalCaseClause;
			}
			return this;
		}
	}
}
