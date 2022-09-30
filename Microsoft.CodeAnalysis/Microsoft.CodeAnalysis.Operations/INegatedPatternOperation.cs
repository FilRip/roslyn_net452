namespace Microsoft.CodeAnalysis.Operations
{
    public interface INegatedPatternOperation : IPatternOperation, IOperation
    {
        IPatternOperation Pattern { get; }
    }
}
