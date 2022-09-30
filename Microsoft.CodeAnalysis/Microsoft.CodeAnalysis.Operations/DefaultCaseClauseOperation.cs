using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class DefaultCaseClauseOperation : BaseCaseClauseOperation, IDefaultCaseClauseOperation, ICaseClauseOperation, IOperation
    {
        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.CaseClause;

        public override CaseKind CaseKind => CaseKind.Default;

        public DefaultCaseClauseOperation(ILabelSymbol? label, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(label, semanticModel, syntax, isImplicit)
        {
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            throw ExceptionUtilities.UnexpectedValue((slot, index));
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            return (false, int.MinValue, int.MinValue);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitDefaultCaseClause(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitDefaultCaseClause(this, argument);
        }
    }
}
