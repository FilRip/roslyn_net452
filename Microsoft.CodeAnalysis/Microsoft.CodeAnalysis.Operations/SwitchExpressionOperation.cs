using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class SwitchExpressionOperation : Operation, ISwitchExpressionOperation, IOperation
    {
        public IOperation Value { get; }

        public ImmutableArray<ISwitchExpressionArmOperation> Arms { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.SwitchExpression;

        public SwitchExpressionOperation(IOperation value, ImmutableArray<ISwitchExpressionArmOperation> arms, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Value = Operation.SetParentOperation(value, this);
            Arms = Operation.SetParentOperation(arms, this);
            Type = type;
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
                    if (index < Arms.Length)
                    {
                        return Arms[index];
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
                    if (!Arms.IsEmpty)
                    {
                        return (true, 1, 0);
                    }
                    goto case 2;
                case 1:
                    if (previousIndex + 1 < Arms.Length)
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
            visitor.VisitSwitchExpression(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitSwitchExpression(this, argument);
        }
    }
}
