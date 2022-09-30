namespace Microsoft.CodeAnalysis.Operations
{
    public interface ITypePatternOperation : IPatternOperation, IOperation
    {
        ITypeSymbol MatchedType { get; }
    }
}
