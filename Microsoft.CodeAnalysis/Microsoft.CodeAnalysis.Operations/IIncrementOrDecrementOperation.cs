#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IIncrementOrDecrementOperation : IOperation
    {
        bool IsPostfix { get; }

        bool IsLifted { get; }

        bool IsChecked { get; }

        IOperation Target { get; }

        IMethodSymbol? OperatorMethod { get; }
    }
}
