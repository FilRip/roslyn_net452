// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Diagnostics;

using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// This class associates a symbol with particular format for display.
    /// It can be passed as an argument for an error message in place where symbol display should go, 
    /// which allows to defer building strings and doing many other things (like loading metadata) 
    /// associated with that until the error message is actually requested.
    /// </summary>
    public sealed class FormattedSymbol : IFormattable
    {
        private readonly ISymbolInternal _symbol;
        private readonly SymbolDisplayFormat _symbolDisplayFormat;

        public FormattedSymbol(ISymbolInternal symbol, SymbolDisplayFormat symbolDisplayFormat)
        {
            Debug.Assert(symbol != null && symbolDisplayFormat != null);

            _symbol = symbol;
            _symbolDisplayFormat = symbolDisplayFormat;
        }

        public override string ToString()
        {
            return _symbol.GetISymbol().ToDisplayString(_symbolDisplayFormat);
        }

        public override bool Equals(object obj)
        {
            return obj is FormattedSymbol other &&
                _symbol.Equals(other._symbol) &&
                _symbolDisplayFormat == other._symbolDisplayFormat;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(
                _symbol.GetHashCode(),
                _symbolDisplayFormat.GetHashCode());
        }

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            return ToString();
        }
    }
}
