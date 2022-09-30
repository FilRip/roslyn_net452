using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.CodeAnalysis
{
    public struct UnifiedAssembly<TAssemblySymbol> where TAssemblySymbol : class, IAssemblySymbolInternal
    {
        public readonly AssemblyIdentity OriginalReference;

        public readonly TAssemblySymbol TargetAssembly;

        public UnifiedAssembly(TAssemblySymbol targetAssembly, AssemblyIdentity originalReference)
        {
            OriginalReference = originalReference;
            TargetAssembly = targetAssembly;
        }
    }
}
