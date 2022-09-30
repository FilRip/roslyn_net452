using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class IsTypeOperation : Operation, IIsTypeOperation, IOperation
    {
        public IOperation ValueOperand { get; }

        public ITypeSymbol TypeOperand { get; }

        public bool IsNegated { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.IsType;

        public IsTypeOperation(IOperation valueOperand, ITypeSymbol typeOperand, bool isNegated, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            ValueOperand = Operation.SetParentOperation(valueOperand, this);
            TypeOperand = typeOperand;
            IsNegated = isNegated;
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && ValueOperand != null)
            {
                return ValueOperand;
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
            else if (ValueOperand != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitIsType(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitIsType(this, argument);
        }
    }
}
