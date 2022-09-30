using System.Collections.Generic;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class LookupSymbolsInfo : AbstractLookupSymbolsInfo<Symbol>
	{
		private const int s_poolSize = 64;

		private static readonly ObjectPool<LookupSymbolsInfo> s_pool = new ObjectPool<LookupSymbolsInfo>(() => new LookupSymbolsInfo(), 64);

		private LookupSymbolsInfo()
			: base((IEqualityComparer<string>)CaseInsensitiveComparison.Comparer)
		{
		}

		public void Free()
		{
			Clear();
			s_pool.Free(this);
		}

		public static LookupSymbolsInfo GetInstance()
		{
			return s_pool.Allocate();
		}
	}
}
