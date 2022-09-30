using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundCaseStatement : BoundStatement
	{
		private readonly ImmutableArray<BoundCaseClause> _CaseClauses;

		private readonly BoundExpression _ConditionOpt;

		protected override ImmutableArray<BoundNode> Children => StaticCast<BoundNode>.From(CaseClauses).Add(ConditionOpt);

		public ImmutableArray<BoundCaseClause> CaseClauses => _CaseClauses;

		public BoundExpression ConditionOpt => _ConditionOpt;

		public BoundCaseStatement(SyntaxNode syntax, ImmutableArray<BoundCaseClause> caseClauses, BoundExpression conditionOpt, bool hasErrors = false)
			: base(BoundKind.CaseStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(caseClauses) || BoundNodeExtensions.NonNullAndHasErrors(conditionOpt))
		{
			_CaseClauses = caseClauses;
			_ConditionOpt = conditionOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitCaseStatement(this);
		}

		public BoundCaseStatement Update(ImmutableArray<BoundCaseClause> caseClauses, BoundExpression conditionOpt)
		{
			if (caseClauses != CaseClauses || conditionOpt != ConditionOpt)
			{
				BoundCaseStatement boundCaseStatement = new BoundCaseStatement(base.Syntax, caseClauses, conditionOpt, base.HasErrors);
				boundCaseStatement.CopyAttributes(this);
				return boundCaseStatement;
			}
			return this;
		}
	}
}
