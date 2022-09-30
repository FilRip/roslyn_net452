using System.Collections.Immutable;

using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.CodeAnalysis
{
    public sealed class ModuleReferences<TAssemblySymbol> where TAssemblySymbol : class, IAssemblySymbolInternal
    {
        public readonly ImmutableArray<AssemblyIdentity> Identities;

        public readonly ImmutableArray<TAssemblySymbol> Symbols;

        public readonly ImmutableArray<UnifiedAssembly<TAssemblySymbol>> UnifiedAssemblies;

        public ModuleReferences(ImmutableArray<AssemblyIdentity> identities, ImmutableArray<TAssemblySymbol> symbols, ImmutableArray<UnifiedAssembly<TAssemblySymbol>> unifiedAssemblies)
        {
            Identities = identities;
            Symbols = symbols;
            UnifiedAssemblies = unifiedAssemblies;
        }
    }
}
