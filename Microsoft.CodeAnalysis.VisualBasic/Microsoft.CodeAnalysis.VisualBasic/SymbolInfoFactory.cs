using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class SymbolInfoFactory
	{
		internal static SymbolInfo Create(ImmutableArray<Symbol> symbols, LookupResultKind resultKind)
		{
			return Create(StaticCast<ISymbol>.From(symbols), resultKind);
		}

		internal static SymbolInfo Create(ImmutableArray<ISymbol> symbols, LookupResultKind resultKind)
		{
			CandidateReason reason = ((resultKind != LookupResultKind.Good) ? LookupResultKindExtensions.ToCandidateReason(resultKind) : CandidateReason.None);
			return Create(symbols, reason);
		}

		internal static SymbolInfo Create(ImmutableArray<ISymbol> symbols, CandidateReason reason)
		{
			symbols = symbols.NullToEmpty();
			if (symbols.IsEmpty && reason != 0 && reason != CandidateReason.LateBound)
			{
				reason = CandidateReason.None;
			}
			return (symbols.Length != 1 || (reason != 0 && reason != CandidateReason.LateBound)) ? new SymbolInfo(symbols, reason) : new SymbolInfo(symbols[0], reason);
		}
	}
}
