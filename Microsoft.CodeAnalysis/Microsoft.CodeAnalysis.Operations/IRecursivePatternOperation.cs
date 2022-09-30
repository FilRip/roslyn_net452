using System.Collections.Immutable;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IRecursivePatternOperation : IPatternOperation, IOperation
    {
        ITypeSymbol MatchedType { get; }

        ISymbol? DeconstructSymbol { get; }

        ImmutableArray<IPatternOperation> DeconstructionSubpatterns { get; }

        ImmutableArray<IPropertySubpatternOperation> PropertySubpatterns { get; }

        ISymbol? DeclaredSymbol { get; }
    }
}
