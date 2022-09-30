using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public abstract class BaseLoopOperation : Operation, ILoopOperation, IOperation
    {
        public abstract LoopKind LoopKind { get; }

        public IOperation Body { get; }

        public ImmutableArray<ILocalSymbol> Locals { get; }

        public ILabelSymbol ContinueLabel { get; }

        public ILabelSymbol ExitLabel { get; }

        protected BaseLoopOperation(IOperation body, ImmutableArray<ILocalSymbol> locals, ILabelSymbol continueLabel, ILabelSymbol exitLabel, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Body = Operation.SetParentOperation(body, this);
            Locals = locals;
            ContinueLabel = continueLabel;
            ExitLabel = exitLabel;
        }
    }
}
