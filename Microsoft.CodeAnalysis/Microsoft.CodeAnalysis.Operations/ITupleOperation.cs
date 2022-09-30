using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface ITupleOperation : IOperation
    {
        ImmutableArray<IOperation> Elements { get; }

        ITypeSymbol? NaturalType { get; }
    }
}
