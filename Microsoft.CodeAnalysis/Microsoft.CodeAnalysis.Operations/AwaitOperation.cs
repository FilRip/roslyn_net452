using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class AwaitOperation : Operation, IAwaitOperation, IOperation
    {
        public IOperation Operation { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.Await;

        public AwaitOperation(IOperation operation, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Operation = Microsoft.CodeAnalysis.Operation.SetParentOperation(operation, this);
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && Operation != null)
            {
                return Operation;
            }
            throw ExceptionUtilities.UnexpectedValue((slot, index));
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            if (previousSlot != -1)
            {
                if ((uint)previousSlot > 1u)
                {
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
                }
            }
            else if (Operation != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitAwait(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitAwait(this, argument);
        }
    }
}