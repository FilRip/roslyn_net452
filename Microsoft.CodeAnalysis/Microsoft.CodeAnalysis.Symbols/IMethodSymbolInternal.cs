namespace Microsoft.CodeAnalysis.Symbols
{
    public interface IMethodSymbolInternal : ISymbolInternal
    {
        bool IsIterator { get; }

        bool IsAsync { get; }

        int CalculateLocalSyntaxOffset(int declaratorPosition, SyntaxTree declaratorTree);

        IMethodSymbolInternal Construct(params ITypeSymbolInternal[] typeArguments);
    }
}
