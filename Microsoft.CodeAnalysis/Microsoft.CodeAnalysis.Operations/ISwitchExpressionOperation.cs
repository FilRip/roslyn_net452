using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Operations
{
    public interface ISwitchExpressionOperation : IOperation
    {
        IOperation Value { get; }

        ImmutableArray<ISwitchExpressionArmOperation> Arms { get; }
    }
}
