using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class SwitchOperation : Operation, ISwitchOperation, IOperation
    {
        public ImmutableArray<ILocalSymbol> Locals { get; }

        public IOperation Value { get; }

        public ImmutableArray<ISwitchCaseOperation> Cases { get; }

        public ILabelSymbol ExitLabel { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.Switch;

        public SwitchOperation(ImmutableArray<ILocalSymbol> locals, IOperation value, ImmutableArray<ISwitchCaseOperation> cases, ILabelSymbol exitLabel, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Locals = locals;
            Value = Operation.SetParentOperation(value, this);
            Cases = Operation.SetParentOperation(cases, this);
            ExitLabel = exitLabel;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Value != null)
                    {
                        return Value;
                    }
                    break;
                case 1:
                    if (index < Cases.Length)
                    {
                        return Cases[index];
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
                    if (Value != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (!Cases.IsEmpty)
                    {
                        return (true, 1, 0);
                    }
                    goto case 2;
                case 1:
                    if (previousIndex + 1 < Cases.Length)
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
            visitor.VisitSwitch(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitSwitch(this, argument);
        }
    }
}
