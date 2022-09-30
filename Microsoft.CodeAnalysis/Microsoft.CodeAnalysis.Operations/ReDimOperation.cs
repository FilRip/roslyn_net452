using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    internal sealed class ReDimOperation : Operation, IReDimOperation, IOperation
    {
        public ImmutableArray<IReDimClauseOperation> Clauses { get; }

        public bool Preserve { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.ReDim;

        internal ReDimOperation(ImmutableArray<IReDimClauseOperation> clauses, bool preserve, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Clauses = Operation.SetParentOperation(clauses, this);
            Preserve = preserve;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && index < Clauses.Length)
            {
                return Clauses[index];
            }
            throw ExceptionUtilities.UnexpectedValue((slot, index));
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            switch (previousSlot)
            {
                case -1:
                    if (!Clauses.IsEmpty)
                    {
                        return (true, 0, 0);
                    }
                    goto case 1;
                case 0:
                    if (previousIndex + 1 < Clauses.Length)
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
            visitor.VisitReDim(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitReDim(this, argument);
        }
    }
}
