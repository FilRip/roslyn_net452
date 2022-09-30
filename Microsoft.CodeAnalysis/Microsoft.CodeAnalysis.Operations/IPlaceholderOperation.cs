namespace Microsoft.CodeAnalysis.Operations
{
    public interface IPlaceholderOperation : IOperation
    {
        PlaceholderKind PlaceholderKind { get; }
    }
}
