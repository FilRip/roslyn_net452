using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class SymbolInfoFactory
    {
        internal static SymbolInfo Create(ImmutableArray<Symbol> symbols, LookupResultKind resultKind, bool isDynamic)
        {
            if (isDynamic)
            {
                if (symbols.Length == 1)
                {
                    return new SymbolInfo(symbols[0].GetPublicSymbol(), CandidateReason.LateBound);
                }
                return new SymbolInfo(symbols.GetPublicSymbols(), CandidateReason.LateBound);
            }
            if (resultKind == LookupResultKind.Viable)
            {
                if (symbols.Length > 0)
                {
                    return new SymbolInfo(symbols[0].GetPublicSymbol());
                }
                return SymbolInfo.None;
            }
            return new SymbolInfo(symbols.GetPublicSymbols(), (symbols.Length > 0) ? resultKind.ToCandidateReason() : CandidateReason.None);
        }
    }
}
