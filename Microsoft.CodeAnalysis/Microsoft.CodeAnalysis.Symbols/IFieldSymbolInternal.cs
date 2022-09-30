namespace Microsoft.CodeAnalysis.Symbols
{
    public interface IFieldSymbolInternal : ISymbolInternal
    {
        bool IsVolatile { get; }
    }
}
