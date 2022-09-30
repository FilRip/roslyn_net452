using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface ISwitchExpressionArmOperation : IOperation
    {
        IPatternOperation Pattern { get; }

        IOperation? Guard { get; }

        IOperation Value { get; }

        ImmutableArray<ILocalSymbol> Locals { get; }
    }
}
