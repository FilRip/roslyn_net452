namespace Microsoft.CodeAnalysis.Operations
{
    public interface IUsingDeclarationOperation : IOperation
    {
        IVariableDeclarationGroupOperation DeclarationGroup { get; }

        bool IsAsynchronous { get; }
    }
}
