using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class PooledDictionaryIgnoringNullableModifiersForReferenceTypes
    {
        private static readonly ObjectPool<PooledDictionary<NamedTypeSymbol, NamedTypeSymbol>> s_poolInstance = PooledDictionary<NamedTypeSymbol, NamedTypeSymbol>.CreatePool(Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.IgnoringNullable);

        internal static PooledDictionary<NamedTypeSymbol, NamedTypeSymbol> GetInstance()
        {
            return s_poolInstance.Allocate();
        }
    }
}
