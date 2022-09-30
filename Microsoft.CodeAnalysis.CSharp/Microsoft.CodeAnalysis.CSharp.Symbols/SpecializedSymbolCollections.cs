using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class SpecializedSymbolCollections
    {
        private static class PooledSymbolHashSet<TSymbol> where TSymbol : Symbol
        {
            internal static readonly ObjectPool<PooledHashSet<TSymbol>> s_poolInstance = PooledHashSet<TSymbol>.CreatePool(SymbolEqualityComparer.ConsiderEverything);
        }

        private static class PooledSymbolDictionary<TSymbol, V> where TSymbol : Symbol
        {
            internal static readonly ObjectPool<PooledDictionary<TSymbol, V>> s_poolInstance = PooledDictionary<TSymbol, V>.CreatePool(SymbolEqualityComparer.ConsiderEverything);
        }

        public static PooledHashSet<TSymbol> GetPooledSymbolHashSetInstance<TSymbol>() where TSymbol : Symbol
        {
            return PooledSymbolHashSet<TSymbol>.s_poolInstance.Allocate();
        }

        public static PooledDictionary<KSymbol, V> GetPooledSymbolDictionaryInstance<KSymbol, V>() where KSymbol : Symbol
        {
            return PooledSymbolDictionary<KSymbol, V>.s_poolInstance.Allocate();
        }
    }
}
