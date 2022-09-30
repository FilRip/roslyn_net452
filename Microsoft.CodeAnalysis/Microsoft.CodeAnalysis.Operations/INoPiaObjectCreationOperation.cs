#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface INoPiaObjectCreationOperation : IOperation
    {
        IObjectOrCollectionInitializerOperation? Initializer { get; }
    }
}
