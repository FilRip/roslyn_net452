using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IConstructorBodyOperation : IMethodBodyBaseOperation, IOperation
    {
        ImmutableArray<ILocalSymbol> Locals { get; }

        IOperation? Initializer { get; }
    }
}
