#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IRangeOperation : IOperation
    {
        IOperation? LeftOperand { get; }

        IOperation? RightOperand { get; }

        bool IsLifted { get; }

        IMethodSymbol? Method { get; }
    }
}
