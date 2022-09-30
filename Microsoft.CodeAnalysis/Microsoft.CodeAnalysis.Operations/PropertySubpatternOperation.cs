using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class PropertySubpatternOperation : Operation, IPropertySubpatternOperation, IOperation
    {
        public IOperation Member { get; }

        public IPatternOperation Pattern { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.PropertySubpattern;

        public PropertySubpatternOperation(IOperation member, IPatternOperation pattern, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Member = Operation.SetParentOperation(member, this);
            Pattern = Operation.SetParentOperation(pattern, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Member != null)
                    {
                        return Member;
                    }
                    break;
                case 1:
                    if (Pattern != null)
                    {
                        return Pattern;
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
                    if (Member != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (Pattern != null)
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
            visitor.VisitPropertySubpattern(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitPropertySubpattern(this, argument);
        }
    }
}
