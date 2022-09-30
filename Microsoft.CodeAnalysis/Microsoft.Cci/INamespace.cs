using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.Cci
{
    public interface INamespace : INamedEntity
    {
        INamespace ContainingNamespace { get; }

        INamespaceSymbolInternal GetInternalSymbol();
    }
}
