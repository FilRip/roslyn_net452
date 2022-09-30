using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IForLoopOperation : ILoopOperation, IOperation
    {
        ImmutableArray<IOperation> Before { get; }

        ImmutableArray<ILocalSymbol> ConditionLocals { get; }

        IOperation? Condition { get; }

        ImmutableArray<IOperation> AtLoopBottom { get; }
    }
}
