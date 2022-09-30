using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public abstract class BaseSymbolInitializerOperation : Operation, ISymbolInitializerOperation, IOperation
    {
        public ImmutableArray<ILocalSymbol> Locals { get; }

        public IOperation Value { get; }

        protected BaseSymbolInitializerOperation(ImmutableArray<ILocalSymbol> locals, IOperation value, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Locals = locals;
            Value = Operation.SetParentOperation(value, this);
        }
    }
}
