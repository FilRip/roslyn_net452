using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class ReDimClauseOperation : Operation, IReDimClauseOperation, IOperation
    {
        public IOperation Operand { get; }

        public ImmutableArray<IOperation> DimensionSizes { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.ReDimClause;

        public ReDimClauseOperation(IOperation operand, ImmutableArray<IOperation> dimensionSizes, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Operand = Operation.SetParentOperation(operand, this);
            DimensionSizes = Operation.SetParentOperation(dimensionSizes, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Operand != null)
                    {
                        return Operand;
                    }
                    break;
                case 1:
                    if (index < DimensionSizes.Length)
                    {
                        return DimensionSizes[index];
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
                    if (Operand != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (!DimensionSizes.IsEmpty)
                    {
                        return (true, 1, 0);
                    }
                    goto case 2;
                case 1:
                    if (previousIndex + 1 < DimensionSizes.Length)
                    {
                        return (true, 1, previousIndex + 1);
                    }
                    goto case 2;
                case 2:
                    return (false, 2, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitReDimClause(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitReDimClause(this, argument);
        }
    }
}
