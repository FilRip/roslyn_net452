// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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
                else
                {
                    return new SymbolInfo(symbols.GetPublicSymbols(), CandidateReason.LateBound);
                }
            }
            else if (resultKind == LookupResultKind.Viable)
            {
                if (symbols.Length > 0)
                {
                    return new SymbolInfo(symbols[0].GetPublicSymbol());
                }
                else
                {
                    return SymbolInfo.None;
                }
            }
            else
            {
                return new SymbolInfo(symbols.GetPublicSymbols(), (symbols.Length > 0) ? resultKind.ToCandidateReason() : CandidateReason.None);
            }
        }
    }
}
