using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class NegatedPatternOperation : BasePatternOperation, INegatedPatternOperation, IPatternOperation, IOperation
    {
        public IPatternOperation Pattern { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.NegatedPattern;

        public NegatedPatternOperation(IPatternOperation pattern, ITypeSymbol inputType, ITypeSymbol narrowedType, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(inputType, narrowedType, semanticModel, syntax, isImplicit)
        {
            Pattern = Operation.SetParentOperation(pattern, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && Pattern != null)
            {
                return Pattern;
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
            else if (Pattern != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitNegatedPattern(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitNegatedPattern(this, argument);
        }
    }
}
