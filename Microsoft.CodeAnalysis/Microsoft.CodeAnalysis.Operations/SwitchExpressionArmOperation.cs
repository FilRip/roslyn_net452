using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class SwitchExpressionArmOperation : Operation, ISwitchExpressionArmOperation, IOperation
    {
        public IPatternOperation Pattern { get; }

        public IOperation? Guard { get; }

        public IOperation Value { get; }

        public ImmutableArray<ILocalSymbol> Locals { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.SwitchExpressionArm;

        public SwitchExpressionArmOperation(IPatternOperation pattern, IOperation? guard, IOperation value, ImmutableArray<ILocalSymbol> locals, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Pattern = Operation.SetParentOperation(pattern, this);
            Guard = Operation.SetParentOperation(guard, this);
            Value = Operation.SetParentOperation(value, this);
            Locals = locals;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Pattern != null)
                    {
                        return Pattern;
                    }
                    break;
                case 1:
                    if (Guard != null)
                    {
                        return Guard;
                    }
                    break;
                case 2:
                    if (Value != null)
                    {
                        return Value;
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
                    if (Pattern != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (Guard != null)
                    {
                        return (true, 1, 0);
                    }
                    goto case 1;
                case 1:
                    if (Value != null)
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
            visitor.VisitSwitchExpressionArm(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitSwitchExpressionArm(this, argument);
        }
    }
}
