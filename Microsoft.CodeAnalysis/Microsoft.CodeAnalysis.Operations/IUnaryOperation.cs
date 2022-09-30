#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IUnaryOperation : IOperation
    {
        UnaryOperatorKind OperatorKind { get; }

        IOperation Operand { get; }

        bool IsLifted { get; }

        bool IsChecked { get; }

        IMethodSymbol? OperatorMethod { get; }
    }
}
