namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundSingleValueCaseClause : BoundCaseClause
	{
		private readonly BoundExpression _ValueOpt;

		private readonly BoundExpression _ConditionOpt;

		public BoundExpression ValueOpt => _ValueOpt;

		public BoundExpression ConditionOpt => _ConditionOpt;

		protected BoundSingleValueCaseClause(BoundKind kind, SyntaxNode syntax, BoundExpression valueOpt, BoundExpression conditionOpt, bool hasErrors = false)
			: base(kind, syntax, hasErrors)
		{
			_ValueOpt = valueOpt;
			_ConditionOpt = conditionOpt;
		}
	}
}
