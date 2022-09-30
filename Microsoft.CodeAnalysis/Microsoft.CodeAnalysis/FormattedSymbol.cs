using System;

using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    public sealed class FormattedSymbol : IFormattable
    {
        private readonly ISymbolInternal _symbol;

        private readonly SymbolDisplayFormat _symbolDisplayFormat;

        public FormattedSymbol(ISymbolInternal symbol, SymbolDisplayFormat symbolDisplayFormat)
        {
            _symbol = symbol;
            _symbolDisplayFormat = symbolDisplayFormat;
        }

        public override string ToString()
        {
            return _symbol.GetISymbol().ToDisplayString(_symbolDisplayFormat);
        }

        public override bool Equals(object obj)
        {
            if (obj is FormattedSymbol formattedSymbol && _symbol.Equals(formattedSymbol._symbol))
            {
                return _symbolDisplayFormat == formattedSymbol._symbolDisplayFormat;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(_symbol.GetHashCode(), _symbolDisplayFormat.GetHashCode());
        }

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            return ToString();
        }
    }
}
