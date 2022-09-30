using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IUsingOperation : IOperation
    {
        IOperation Resources { get; }

        IOperation Body { get; }

        ImmutableArray<ILocalSymbol> Locals { get; }

        bool IsAsynchronous { get; }
    }
}
