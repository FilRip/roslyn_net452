namespace Microsoft.CodeAnalysis.Operations
{
    public interface IPropertySubpatternOperation : IOperation
    {
        IOperation Member { get; }

        IPatternOperation Pattern { get; }
    }
}
