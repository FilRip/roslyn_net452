using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundQueryClause : BoundQueryClauseBase
	{
		private readonly BoundExpression _UnderlyingExpression;

		public override Symbol ExpressionSymbol => UnderlyingExpression.ExpressionSymbol;

		public override LookupResultKind ResultKind => UnderlyingExpression.ResultKind;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)UnderlyingExpression);

		public BoundExpression UnderlyingExpression => _UnderlyingExpression;

		public BoundQueryClause(SyntaxNode syntax, BoundExpression underlyingExpression, ImmutableArray<RangeVariableSymbol> rangeVariables, TypeSymbol compoundVariableType, ImmutableArray<Binder> binders, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.QueryClause, syntax, rangeVariables, compoundVariableType, binders, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(underlyingExpression))
		{
			_UnderlyingExpression = underlyingExpression;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitQueryClause(this);
		}

		public BoundQueryClause Update(BoundExpression underlyingExpression, ImmutableArray<RangeVariableSymbol> rangeVariables, TypeSymbol compoundVariableType, ImmutableArray<Binder> binders, TypeSymbol type)
		{
			if (underlyingExpression != UnderlyingExpression || rangeVariables != base.RangeVariables || (object)compoundVariableType != base.CompoundVariableType || binders != base.Binders || (object)type != base.Type)
			{
				BoundQueryClause boundQueryClause = new BoundQueryClause(base.Syntax, underlyingExpression, rangeVariables, compoundVariableType, binders, type, base.HasErrors);
				boundQueryClause.CopyAttributes(this);
				return boundQueryClause;
			}
			return this;
		}
	}
}
