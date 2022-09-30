using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface ICatchClauseOperation : IOperation
    {
        IOperation? ExceptionDeclarationOrExpression { get; }

        ITypeSymbol ExceptionType { get; }

        ImmutableArray<ILocalSymbol> Locals { get; }

        IOperation? Filter { get; }

        IBlockOperation Handler { get; }
    }
}
