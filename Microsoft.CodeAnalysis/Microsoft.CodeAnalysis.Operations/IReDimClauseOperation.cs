using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IReDimClauseOperation : IOperation
    {
        IOperation Operand { get; }

        ImmutableArray<IOperation> DimensionSizes { get; }
    }
}
