using System;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class LookupSymbolsInfo : AbstractLookupSymbolsInfo<Symbol>
    {
        private const int poolSize = 64;

        private static readonly ObjectPool<LookupSymbolsInfo> s_pool = new ObjectPool<LookupSymbolsInfo>(() => new LookupSymbolsInfo(), 64);

        private LookupSymbolsInfo()
            : base(StringComparer.Ordinal)
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
