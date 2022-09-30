using System.Collections.Immutable;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public struct DisposeOperationInfo
    {
        public readonly IMethodSymbol? DisposeMethod;

        public readonly ImmutableArray<IArgumentOperation> DisposeArguments;

        public DisposeOperationInfo(IMethodSymbol? disposeMethod, ImmutableArray<IArgumentOperation> disposeArguments)
        {
            DisposeMethod = disposeMethod;
            DisposeArguments = disposeArguments;
        }
    }
}
