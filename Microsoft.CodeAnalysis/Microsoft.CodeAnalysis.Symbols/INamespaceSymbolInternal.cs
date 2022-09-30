namespace Microsoft.CodeAnalysis.Symbols
{
    public interface INamespaceSymbolInternal : INamespaceOrTypeSymbolInternal, ISymbolInternal
    {
        bool IsGlobalNamespace { get; }
    }
}
