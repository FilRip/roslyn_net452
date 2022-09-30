using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class LexicalOrderSymbolComparer : IComparer<Symbol>
	{
		public static readonly LexicalOrderSymbolComparer Instance = new LexicalOrderSymbolComparer();

		private LexicalOrderSymbolComparer()
		{
		}

		public int Compare(Symbol x, Symbol y)
		{
			if ((object)x == y)
			{
				return 0;
			}
			LexicalSortKey xSortKey = x.GetLexicalSortKey();
			LexicalSortKey ySortKey = y.GetLexicalSortKey();
			int num = LexicalSortKey.Compare(ref xSortKey, ref ySortKey);
			if (num != 0)
			{
				return num;
			}
			num = ((ISymbol)x).Kind.ToSortOrder() - ((ISymbol)y).Kind.ToSortOrder();
			if (num != 0)
			{
				return num;
			}
			return CaseInsensitiveComparison.Compare(x.Name, y.Name);
		}

		int IComparer<Symbol>.Compare(Symbol x, Symbol y)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Compare
			return this.Compare(x, y);
		}
	}
}
