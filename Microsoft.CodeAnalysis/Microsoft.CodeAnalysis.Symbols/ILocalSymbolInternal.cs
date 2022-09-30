namespace Microsoft.CodeAnalysis.Symbols
{
    public interface ILocalSymbolInternal : ISymbolInternal
    {
        bool IsImportedFromMetadata { get; }

        SynthesizedLocalKind SynthesizedKind { get; }

        SyntaxNode GetDeclaratorSyntax();
    }
}
