namespace Microsoft.CodeAnalysis.Operations
{
    public interface IRelationalPatternOperation : IPatternOperation, IOperation
    {
        BinaryOperatorKind OperatorKind { get; }

        IOperation Value { get; }
    }
}
