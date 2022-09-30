using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class SymbolsInProgress<T> where T : Symbol
	{
		private readonly ImmutableSetWithInsertionOrder<T> _symbols;

		internal static readonly SymbolsInProgress<T> Empty = new SymbolsInProgress<T>(ImmutableSetWithInsertionOrder<T>.Empty);

		private SymbolsInProgress(ImmutableSetWithInsertionOrder<T> fields)
		{
			_symbols = fields;
		}

		internal SymbolsInProgress<T> Add(T symbol)
		{
			return new SymbolsInProgress<T>(_symbols.Add(symbol));
		}

		internal bool Contains(T symbol)
		{
			return _symbols.Contains(symbol);
		}
	}
}
