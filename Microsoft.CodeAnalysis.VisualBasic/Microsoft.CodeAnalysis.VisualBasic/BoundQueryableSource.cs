using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundQueryableSource : BoundQueryClauseBase
	{
		private readonly BoundQueryPart _Source;

		private readonly RangeVariableSymbol _RangeVariableOpt;

		public override Symbol ExpressionSymbol => Source.ExpressionSymbol;

		public override LookupResultKind ResultKind => Source.ResultKind;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)Source);

		public BoundQueryPart Source => _Source;

		public RangeVariableSymbol RangeVariableOpt => _RangeVariableOpt;

		public BoundQueryableSource(SyntaxNode syntax, BoundQueryPart source, RangeVariableSymbol rangeVariableOpt, ImmutableArray<RangeVariableSymbol> rangeVariables, TypeSymbol compoundVariableType, ImmutableArray<Binder> binders, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.QueryableSource, syntax, rangeVariables, compoundVariableType, binders, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(source))
		{
			_Source = source;
			_RangeVariableOpt = rangeVariableOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitQueryableSource(this);
		}

		public BoundQueryableSource Update(BoundQueryPart source, RangeVariableSymbol rangeVariableOpt, ImmutableArray<RangeVariableSymbol> rangeVariables, TypeSymbol compoundVariableType, ImmutableArray<Binder> binders, TypeSymbol type)
		{
			if (source != Source || (object)rangeVariableOpt != RangeVariableOpt || rangeVariables != base.RangeVariables || (object)compoundVariableType != base.CompoundVariableType || binders != base.Binders || (object)type != base.Type)
			{
				BoundQueryableSource boundQueryableSource = new BoundQueryableSource(base.Syntax, source, rangeVariableOpt, rangeVariables, compoundVariableType, binders, type, base.HasErrors);
				boundQueryableSource.CopyAttributes(this);
				return boundQueryableSource;
			}
			return this;
		}
	}
}
