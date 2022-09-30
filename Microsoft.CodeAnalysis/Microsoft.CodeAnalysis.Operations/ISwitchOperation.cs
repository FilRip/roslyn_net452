using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Operations
{
    public interface ISwitchOperation : IOperation
    {
        ImmutableArray<ILocalSymbol> Locals { get; }

        IOperation Value { get; }

        ImmutableArray<ISwitchCaseOperation> Cases { get; }

        ILabelSymbol ExitLabel { get; }
    }
}
