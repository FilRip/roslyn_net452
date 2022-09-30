using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class EndOperation : Operation, IEndOperation, IOperation
    {
        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.End;

        public EndOperation(SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
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
            visitor.VisitEnd(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitEnd(this, argument);
        }
    }
}
