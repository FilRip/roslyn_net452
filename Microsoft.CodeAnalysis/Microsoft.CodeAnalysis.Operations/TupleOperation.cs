using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class TupleOperation : Operation, ITupleOperation, IOperation
    {
        public ImmutableArray<IOperation> Elements { get; }

        public ITypeSymbol? NaturalType { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.Tuple;

        public TupleOperation(ImmutableArray<IOperation> elements, ITypeSymbol? naturalType, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Elements = Operation.SetParentOperation(elements, this);
            NaturalType = naturalType;
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && index < Elements.Length)
            {
                return Elements[index];
            }
            throw ExceptionUtilities.UnexpectedValue((slot, index));
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            switch (previousSlot)
            {
                case -1:
                    if (!Elements.IsEmpty)
                    {
                        return (true, 0, 0);
                    }
                    goto case 1;
                case 0:
                    if (previousIndex + 1 < Elements.Length)
                    {
                        return (true, 0, previousIndex + 1);
                    }
                    goto case 1;
                case 1:
                    return (false, 1, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitTuple(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitTuple(this, argument);
        }
    }
}
