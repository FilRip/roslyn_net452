using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundAggregateClause : BoundQueryClauseBase
	{
		private readonly BoundQueryClauseBase _CapturedGroupOpt;

		private readonly BoundRValuePlaceholder _GroupPlaceholderOpt;

		private readonly BoundExpression _UnderlyingExpression;

		public override Symbol ExpressionSymbol => UnderlyingExpression.ExpressionSymbol;

		public override LookupResultKind ResultKind => UnderlyingExpression.ResultKind;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)CapturedGroupOpt, (BoundNode)UnderlyingExpression);

		public BoundQueryClauseBase CapturedGroupOpt => _CapturedGroupOpt;

		public BoundRValuePlaceholder GroupPlaceholderOpt => _GroupPlaceholderOpt;

		public BoundExpression UnderlyingExpression => _UnderlyingExpression;

		public BoundAggregateClause(SyntaxNode syntax, BoundQueryClauseBase capturedGroupOpt, BoundRValuePlaceholder groupPlaceholderOpt, BoundExpression underlyingExpression, ImmutableArray<RangeVariableSymbol> rangeVariables, TypeSymbol compoundVariableType, ImmutableArray<Binder> binders, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.AggregateClause, syntax, rangeVariables, compoundVariableType, binders, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(capturedGroupOpt) || BoundNodeExtensions.NonNullAndHasErrors(groupPlaceholderOpt) || BoundNodeExtensions.NonNullAndHasErrors(underlyingExpression))
		{
			_CapturedGroupOpt = capturedGroupOpt;
			_GroupPlaceholderOpt = groupPlaceholderOpt;
			_UnderlyingExpression = underlyingExpression;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitAggregateClause(this);
		}

		public BoundAggregateClause Update(BoundQueryClauseBase capturedGroupOpt, BoundRValuePlaceholder groupPlaceholderOpt, BoundExpression underlyingExpression, ImmutableArray<RangeVariableSymbol> rangeVariables, TypeSymbol compoundVariableType, ImmutableArray<Binder> binders, TypeSymbol type)
		{
			if (capturedGroupOpt != CapturedGroupOpt || groupPlaceholderOpt != GroupPlaceholderOpt || underlyingExpression != UnderlyingExpression || rangeVariables != base.RangeVariables || (object)compoundVariableType != base.CompoundVariableType || binders != base.Binders || (object)type != base.Type)
			{
				BoundAggregateClause boundAggregateClause = new BoundAggregateClause(base.Syntax, capturedGroupOpt, groupPlaceholderOpt, underlyingExpression, rangeVariables, compoundVariableType, binders, type, base.HasErrors);
				boundAggregateClause.CopyAttributes(this);
				return boundAggregateClause;
			}
			return this;
		}
	}
}
