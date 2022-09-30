namespace Microsoft.CodeAnalysis.Operations
{
    public interface IPatternOperation : IOperation
    {
        ITypeSymbol InputType { get; }

        ITypeSymbol NarrowedType { get; }
    }
}
