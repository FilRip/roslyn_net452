#nullable enable

namespace Microsoft.CodeAnalysis.Symbols
{
    public interface ISynthesizedMethodBodyImplementationSymbol : ISymbolInternal
    {
        IMethodSymbolInternal? Method { get; }

        bool HasMethodBodyDependency { get; }
    }
}
