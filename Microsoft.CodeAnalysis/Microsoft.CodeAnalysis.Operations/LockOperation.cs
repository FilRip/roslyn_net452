using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class LockOperation : Operation, ILockOperation, IOperation
    {
        public IOperation LockedValue { get; }

        public IOperation Body { get; }

        public ILocalSymbol? LockTakenSymbol { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.Lock;

        public LockOperation(IOperation lockedValue, IOperation body, ILocalSymbol? lockTakenSymbol, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            LockedValue = Operation.SetParentOperation(lockedValue, this);
            Body = Operation.SetParentOperation(body, this);
            LockTakenSymbol = lockTakenSymbol;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (LockedValue != null)
                    {
                        return LockedValue;
                    }
                    break;
                case 1:
                    if (Body != null)
                    {
                        return Body;
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
                    if (LockedValue != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (Body != null)
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
            visitor.VisitLock(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitLock(this, argument);
        }
    }
}
