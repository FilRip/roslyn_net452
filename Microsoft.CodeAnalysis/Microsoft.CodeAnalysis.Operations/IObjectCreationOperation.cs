using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IObjectCreationOperation : IOperation
    {
        IMethodSymbol? Constructor { get; }

        IObjectOrCollectionInitializerOperation? Initializer { get; }

        ImmutableArray<IArgumentOperation> Arguments { get; }
    }
}
