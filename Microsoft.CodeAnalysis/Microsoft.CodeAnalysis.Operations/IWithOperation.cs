#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IWithOperation : IOperation
    {
        IOperation Operand { get; }

        IMethodSymbol? CloneMethod { get; }

        IObjectOrCollectionInitializerOperation Initializer { get; }
    }
}
