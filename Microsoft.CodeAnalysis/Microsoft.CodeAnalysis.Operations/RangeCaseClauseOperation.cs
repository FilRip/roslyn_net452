using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    internal sealed class RangeCaseClauseOperation : BaseCaseClauseOperation, IRangeCaseClauseOperation, ICaseClauseOperation, IOperation
    {
        public IOperation MinimumValue { get; }

        public IOperation MaximumValue { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.CaseClause;

        public override CaseKind CaseKind => CaseKind.Range;

        internal RangeCaseClauseOperation(IOperation minimumValue, IOperation maximumValue, ILabelSymbol? label, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(label, semanticModel, syntax, isImplicit)
        {
            MinimumValue = Operation.SetParentOperation(minimumValue, this);
            MaximumValue = Operation.SetParentOperation(maximumValue, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (MinimumValue != null)
                    {
                        return MinimumValue;
                    }
                    break;
                case 1:
                    if (MaximumValue != null)
                    {
                        return MaximumValue;
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
                    if (MinimumValue != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (MaximumValue != null)
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
            visitor.VisitRangeCaseClause(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitRangeCaseClause(this, argument);
        }
    }
}
