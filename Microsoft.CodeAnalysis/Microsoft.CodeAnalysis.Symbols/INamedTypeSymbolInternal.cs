namespace Microsoft.CodeAnalysis.Symbols
{
    public interface INamedTypeSymbolInternal : ITypeSymbolInternal, INamespaceOrTypeSymbolInternal, ISymbolInternal
    {
        INamedTypeSymbolInternal EnumUnderlyingType { get; }
    }
}
