using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    internal sealed class AggregateQueryOperation : Operation, IAggregateQueryOperation, IOperation
    {
        public IOperation Group { get; }

        public IOperation Aggregation { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.None;

        internal AggregateQueryOperation(IOperation group, IOperation aggregation, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Group = Operation.SetParentOperation(group, this);
            Aggregation = Operation.SetParentOperation(aggregation, this);
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Group != null)
                    {
                        return Group;
                    }
                    break;
                case 1:
                    if (Aggregation != null)
                    {
                        return Aggregation;
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
                    if (Group != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (Aggregation != null)
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
            visitor.VisitAggregateQuery(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitAggregateQuery(this, argument);
        }
    }
}
