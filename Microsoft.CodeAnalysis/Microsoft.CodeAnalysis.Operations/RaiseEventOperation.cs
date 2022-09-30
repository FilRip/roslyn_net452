using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class RaiseEventOperation : Operation, IRaiseEventOperation, IOperation
    {
        public IEventReferenceOperation EventReference { get; }

        public ImmutableArray<IArgumentOperation> Arguments { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.RaiseEvent;

        public RaiseEventOperation(IEventReferenceOperation eventReference, ImmutableArray<IArgumentOperation> arguments, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            EventReference = Operation.SetParentOperation(eventReference, this);
            Arguments = Operation.SetParentOperation(arguments, this);
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
                    if (index < Arguments.Length)
                    {
                        return Arguments[index];
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
                    if (!Arguments.IsEmpty)
                    {
                        return (true, 1, 0);
                    }
                    goto case 2;
                case 1:
                    if (previousIndex + 1 < Arguments.Length)
                    {
                        return (true, 1, previousIndex + 1);
                    }
                    goto case 2;
                case 2:
                    return (false, 2, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitRaiseEvent(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitRaiseEvent(this, argument);
        }
    }
}
