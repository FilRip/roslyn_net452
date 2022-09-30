using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundQueryClauseBase : BoundQueryPart
	{
		private readonly ImmutableArray<RangeVariableSymbol> _RangeVariables;

		private readonly TypeSymbol _CompoundVariableType;

		private readonly ImmutableArray<Binder> _Binders;

		public ImmutableArray<RangeVariableSymbol> RangeVariables => _RangeVariables;

		public TypeSymbol CompoundVariableType => _CompoundVariableType;

		public ImmutableArray<Binder> Binders => _Binders;

		protected BoundQueryClauseBase(BoundKind kind, SyntaxNode syntax, ImmutableArray<RangeVariableSymbol> rangeVariables, TypeSymbol compoundVariableType, ImmutableArray<Binder> binders, TypeSymbol type, bool hasErrors)
			: base(kind, syntax, type, hasErrors)
		{
			_RangeVariables = rangeVariables;
			_CompoundVariableType = compoundVariableType;
			_Binders = binders;
		}

		protected BoundQueryClauseBase(BoundKind kind, SyntaxNode syntax, ImmutableArray<RangeVariableSymbol> rangeVariables, TypeSymbol compoundVariableType, ImmutableArray<Binder> binders, TypeSymbol type)
			: base(kind, syntax, type)
		{
			_RangeVariables = rangeVariables;
			_CompoundVariableType = compoundVariableType;
			_Binders = binders;
		}
	}
}
