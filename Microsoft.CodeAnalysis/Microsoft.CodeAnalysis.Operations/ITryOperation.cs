using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface ITryOperation : IOperation
    {
        IBlockOperation Body { get; }

        ImmutableArray<ICatchClauseOperation> Catches { get; }

        IBlockOperation? Finally { get; }

        ILabelSymbol? ExitLabel { get; }
    }
}
