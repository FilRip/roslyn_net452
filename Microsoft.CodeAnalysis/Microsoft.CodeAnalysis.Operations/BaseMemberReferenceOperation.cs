#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public abstract class BaseMemberReferenceOperation : Operation, IMemberReferenceOperation, IOperation
    {
        public IOperation? Instance { get; }

        public abstract ISymbol Member { get; }

        protected BaseMemberReferenceOperation(IOperation? instance, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Instance = Operation.SetParentOperation(instance, this);
        }
    }
}
