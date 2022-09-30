#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface ICompoundAssignmentOperation : IAssignmentOperation, IOperation
    {
        CommonConversion InConversion { get; }

        CommonConversion OutConversion { get; }

        BinaryOperatorKind OperatorKind { get; }

        bool IsLifted { get; }

        bool IsChecked { get; }

        IMethodSymbol? OperatorMethod { get; }
    }
}
