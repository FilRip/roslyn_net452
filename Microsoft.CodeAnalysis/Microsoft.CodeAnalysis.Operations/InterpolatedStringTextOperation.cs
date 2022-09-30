using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class InterpolatedStringTextOperation : BaseInterpolatedStringContentOperation, IInterpolatedStringTextOperation, IInterpolatedStringContentOperation, IOperation
    {
        public IOperation Text { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.InterpolatedStringText;

        public InterpolatedStringTextOperation(IOperation text, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Text = Operation.SetParentOperation(text, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && Text != null)
            {
                return Text;
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
            else if (Text != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitInterpolatedStringText(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitInterpolatedStringText(this, argument);
        }
    }
}
