using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IArrayCreationOperation : IOperation
    {
        ImmutableArray<IOperation> DimensionSizes { get; }

        IArrayInitializerOperation? Initializer { get; }
    }
}
