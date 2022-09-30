using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IInvocationOperation : IOperation
    {
        IMethodSymbol TargetMethod { get; }

        IOperation? Instance { get; }

        bool IsVirtual { get; }

        ImmutableArray<IArgumentOperation> Arguments { get; }
    }
}
