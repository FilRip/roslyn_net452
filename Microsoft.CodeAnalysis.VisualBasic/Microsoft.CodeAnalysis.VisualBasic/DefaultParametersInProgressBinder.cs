using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class DefaultParametersInProgressBinder : SymbolsInProgressBinder<ParameterSymbol>
	{
		internal override SymbolsInProgress<ParameterSymbol> DefaultParametersInProgress => inProgress;

		internal DefaultParametersInProgressBinder(SymbolsInProgress<ParameterSymbol> inProgress, Binder next)
			: base(inProgress, next)
		{
		}
	}
}
