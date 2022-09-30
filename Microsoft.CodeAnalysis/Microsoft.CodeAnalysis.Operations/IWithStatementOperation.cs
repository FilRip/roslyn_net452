namespace Microsoft.CodeAnalysis.Operations
{
    internal interface IWithStatementOperation : IOperation
    {
        IOperation Body { get; }

        IOperation Value { get; }
    }
}
