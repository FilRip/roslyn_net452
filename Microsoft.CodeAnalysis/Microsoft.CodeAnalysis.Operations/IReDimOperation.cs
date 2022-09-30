using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IReDimOperation : IOperation
    {
        ImmutableArray<IReDimClauseOperation> Clauses { get; }

        bool Preserve { get; }
    }
}
