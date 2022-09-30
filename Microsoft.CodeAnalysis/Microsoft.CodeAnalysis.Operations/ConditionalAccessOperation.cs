using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class ConditionalAccessOperation : Operation, IConditionalAccessOperation, IOperation
    {
        public IOperation Operation { get; }

        public IOperation WhenNotNull { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.ConditionalAccess;

        public ConditionalAccessOperation(IOperation operation, IOperation whenNotNull, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Operation = Microsoft.CodeAnalysis.Operation.SetParentOperation(operation, this);
            WhenNotNull = Microsoft.CodeAnalysis.Operation.SetParentOperation(whenNotNull, this);
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Operation != null)
                    {
                        return Operation;
                    }
                    break;
                case 1:
                    if (WhenNotNull != null)
                    {
                        return WhenNotNull;
                    }
                    break;
            }
            throw ExceptionUtilities.UnexpectedValue((slot, index));
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            switch (previousSlot)
            {
                case -1:
                    if (Operation != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (WhenNotNull != null)
                    {
                        return (true, 1, 0);
                    }
                    goto case 1;
                case 1:
                case 2:
                    return (false, 2, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitConditionalAccess(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitConditionalAccess(this, argument);
        }
    }
}
