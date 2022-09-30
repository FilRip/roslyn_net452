#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public abstract class BaseAssignmentOperation : Operation, IAssignmentOperation, IOperation
    {
        public IOperation Target { get; }

        public IOperation Value { get; }

        protected BaseAssignmentOperation(IOperation target, IOperation value, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Target = Operation.SetParentOperation(target, this);
            Value = Operation.SetParentOperation(value, this);
        }
    }
}
