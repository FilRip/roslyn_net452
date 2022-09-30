using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class PatternCaseClauseOperation : BaseCaseClauseOperation, IPatternCaseClauseOperation, ICaseClauseOperation, IOperation
    {
        public new ILabelSymbol Label => base.Label;

        public IPatternOperation Pattern { get; }

        public IOperation? Guard { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.CaseClause;

        public override CaseKind CaseKind => CaseKind.Pattern;

        public PatternCaseClauseOperation(ILabelSymbol label, IPatternOperation pattern, IOperation? guard, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(label, semanticModel, syntax, isImplicit)
        {
            Pattern = Operation.SetParentOperation(pattern, this);
            Guard = Operation.SetParentOperation(guard, this);
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
                case 2:
                    return (false, 2, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitPatternCaseClause(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitPatternCaseClause(this, argument);
        }
    }
}
