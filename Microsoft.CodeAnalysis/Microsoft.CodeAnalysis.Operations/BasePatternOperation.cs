#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public abstract class BasePatternOperation : Operation, IPatternOperation, IOperation
    {
        public ITypeSymbol InputType { get; }

        public ITypeSymbol NarrowedType { get; }

        protected BasePatternOperation(ITypeSymbol inputType, ITypeSymbol narrowedType, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            InputType = inputType;
            NarrowedType = narrowedType;
        }
    }
}
