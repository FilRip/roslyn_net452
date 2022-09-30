#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IDeclarationPatternOperation : IPatternOperation, IOperation
    {
        ITypeSymbol? MatchedType { get; }

        bool MatchesNull { get; }

        ISymbol? DeclaredSymbol { get; }
    }
}
