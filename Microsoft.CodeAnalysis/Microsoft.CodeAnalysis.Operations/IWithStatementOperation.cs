namespace Microsoft.CodeAnalysis.Operations
{
    public interface IWithStatementOperation : IOperation
    {
        IOperation Body { get; }

        IOperation Value { get; }
    }
}
