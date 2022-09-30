using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class BinaryOperation : Operation, IBinaryOperation, IOperation
    {
        public BinaryOperatorKind OperatorKind { get; }

        public IOperation LeftOperand { get; }

        public IOperation RightOperand { get; }

        public bool IsLifted { get; }

        public bool IsChecked { get; }

        public bool IsCompareText { get; }

        public IMethodSymbol? OperatorMethod { get; }

        public IMethodSymbol? UnaryOperatorMethod { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue { get; }

        public override OperationKind Kind => OperationKind.Binary;

        public BinaryOperation(BinaryOperatorKind operatorKind, IOperation leftOperand, IOperation rightOperand, bool isLifted, bool isChecked, bool isCompareText, IMethodSymbol? operatorMethod, IMethodSymbol? unaryOperatorMethod, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, ConstantValue? constantValue, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            OperatorKind = operatorKind;
            LeftOperand = Operation.SetParentOperation(leftOperand, this);
            RightOperand = Operation.SetParentOperation(rightOperand, this);
            IsLifted = isLifted;
            IsChecked = isChecked;
            IsCompareText = isCompareText;
            OperatorMethod = operatorMethod;
            UnaryOperatorMethod = unaryOperatorMethod;
            OperationConstantValue = constantValue;
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (LeftOperand != null)
                    {
                        return LeftOperand;
                    }
                    break;
                case 1:
                    if (RightOperand != null)
                    {
                        return RightOperand;
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
                    if (LeftOperand != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (RightOperand != null)
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
            visitor.VisitBinaryOperator(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitBinaryOperator(this, argument);
        }
    }
}
