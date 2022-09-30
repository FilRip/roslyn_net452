#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public abstract class BaseInterpolatedStringContentOperation : Operation, IInterpolatedStringContentOperation, IOperation
    {
        protected BaseInterpolatedStringContentOperation(SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
        }
    }
}
