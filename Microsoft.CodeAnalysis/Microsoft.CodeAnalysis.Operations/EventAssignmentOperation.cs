using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class EventAssignmentOperation : Operation, IEventAssignmentOperation, IOperation
    {
        public IOperation EventReference { get; }

        public IOperation HandlerValue { get; }

        public bool Adds { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.EventAssignment;

        public EventAssignmentOperation(IOperation eventReference, IOperation handlerValue, bool adds, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            EventReference = Operation.SetParentOperation(eventReference, this);
            HandlerValue = Operation.SetParentOperation(handlerValue, this);
            Adds = adds;
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (EventReference != null)
                    {
                        return EventReference;
                    }
                    break;
                case 1:
                    if (HandlerValue != null)
                    {
                        return HandlerValue;
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
                    if (EventReference != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (HandlerValue != null)
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
            visitor.VisitEventAssignment(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitEventAssignment(this, argument);
        }
    }
}
