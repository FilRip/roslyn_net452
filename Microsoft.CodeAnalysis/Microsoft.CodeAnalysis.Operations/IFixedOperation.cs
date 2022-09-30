using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IFixedOperation : IOperation
    {
        ImmutableArray<ILocalSymbol> Locals { get; }

        IVariableDeclarationGroupOperation Variables { get; }

        IOperation Body { get; }
    }
}
