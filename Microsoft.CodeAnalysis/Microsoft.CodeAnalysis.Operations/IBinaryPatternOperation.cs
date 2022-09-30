namespace Microsoft.CodeAnalysis.Operations
{
    public interface IBinaryPatternOperation : IPatternOperation, IOperation
    {
        BinaryOperatorKind OperatorKind { get; }

        IPatternOperation LeftPattern { get; }

        IPatternOperation RightPattern { get; }
    }
}
