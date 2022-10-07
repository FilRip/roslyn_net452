using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class BinaryPatternOperation : BasePatternOperation, IBinaryPatternOperation, IPatternOperation, IOperation
    {
        public BinaryOperatorKind OperatorKind { get; }

        public IPatternOperation LeftPattern { get; }

        public IPatternOperation RightPattern { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.BinaryPattern;

        public BinaryPatternOperation(BinaryOperatorKind operatorKind, IPatternOperation leftPattern, IPatternOperation rightPattern, ITypeSymbol inputType, ITypeSymbol narrowedType, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(inputType, narrowedType, semanticModel, syntax, isImplicit)
        {
            OperatorKind = operatorKind;
            LeftPattern = Operation.SetParentOperation(leftPattern, this);
            RightPattern = Operation.SetParentOperation(rightPattern, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (LeftPattern != null)
                    {
                        return LeftPattern;
                    }
                    break;
                case 1:
                    if (RightPattern != null)
                    {
                        return RightPattern;
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
                    if (LeftPattern != null) return (true, 0, 0);
                    else goto case 0;
                case 0:
                    if (RightPattern != null) return (true, 1, 0);
                    else goto case 1;
                case 1:
                case 2:
                    return (false, 2, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitBinaryPattern(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitBinaryPattern(this, argument);
        }
    }
}
