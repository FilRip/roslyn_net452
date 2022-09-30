using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    internal sealed class RelationalCaseClauseOperation : BaseCaseClauseOperation, IRelationalCaseClauseOperation, ICaseClauseOperation, IOperation
    {
        public IOperation Value { get; }

        public BinaryOperatorKind Relation { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.CaseClause;

        public override CaseKind CaseKind => CaseKind.Relational;

        internal RelationalCaseClauseOperation(IOperation value, BinaryOperatorKind relation, ILabelSymbol? label, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(label, semanticModel, syntax, isImplicit)
        {
            Value = Operation.SetParentOperation(value, this);
            Relation = relation;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && Value != null)
            {
                return Value;
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
            else if (Value != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitRelationalCaseClause(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitRelationalCaseClause(this, argument);
        }
    }
}
