#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public abstract class BaseMethodBodyBaseOperation : Operation, IMethodBodyBaseOperation, IOperation
    {
        public IBlockOperation? BlockBody { get; }

        public IBlockOperation? ExpressionBody { get; }

        protected BaseMethodBodyBaseOperation(IBlockOperation? blockBody, IBlockOperation? expressionBody, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            BlockBody = Operation.SetParentOperation(blockBody, this);
            ExpressionBody = Operation.SetParentOperation(expressionBody, this);
        }
    }
}
