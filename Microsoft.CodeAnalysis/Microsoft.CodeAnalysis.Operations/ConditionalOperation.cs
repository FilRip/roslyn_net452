using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class ConditionalOperation : Operation, IConditionalOperation, IOperation
    {
        public IOperation Condition { get; }

        public IOperation WhenTrue { get; }

        public IOperation? WhenFalse { get; }

        public bool IsRef { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue { get; }

        public override OperationKind Kind => OperationKind.Conditional;

        public ConditionalOperation(IOperation condition, IOperation whenTrue, IOperation? whenFalse, bool isRef, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, ConstantValue? constantValue, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Condition = Operation.SetParentOperation(condition, this);
            WhenTrue = Operation.SetParentOperation(whenTrue, this);
            WhenFalse = Operation.SetParentOperation(whenFalse, this);
            IsRef = isRef;
            OperationConstantValue = constantValue;
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Condition != null)
                    {
                        return Condition;
                    }
                    break;
                case 1:
                    if (WhenTrue != null)
                    {
                        return WhenTrue;
                    }
                    break;
                case 2:
                    if (WhenFalse != null)
                    {
                        return WhenFalse;
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
                    if (Condition != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (WhenTrue != null)
                    {
                        return (true, 1, 0);
                    }
                    goto case 1;
                case 1:
                    if (WhenFalse != null)
                    {
                        return (true, 2, 0);
                    }
                    goto case 2;
                case 2:
                case 3:
                    return (false, 3, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitConditional(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitConditional(this, argument);
        }
    }
}
