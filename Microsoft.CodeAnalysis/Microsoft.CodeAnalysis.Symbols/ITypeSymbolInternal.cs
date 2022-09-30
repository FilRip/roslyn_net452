namespace Microsoft.CodeAnalysis.Symbols
{
    public interface ITypeSymbolInternal : INamespaceOrTypeSymbolInternal, ISymbolInternal
    {
        TypeKind TypeKind { get; }

        SpecialType SpecialType { get; }

        bool IsReferenceType { get; }

        bool IsValueType { get; }

        ITypeSymbol GetITypeSymbol();
    }
}
