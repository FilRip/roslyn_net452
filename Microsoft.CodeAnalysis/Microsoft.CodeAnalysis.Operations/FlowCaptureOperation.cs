using Microsoft.CodeAnalysis.FlowAnalysis;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    internal sealed class FlowCaptureOperation : Operation, IFlowCaptureOperation, IOperation
    {
        public CaptureId Id { get; }

        public IOperation Value { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.FlowCapture;

        internal FlowCaptureOperation(CaptureId id, IOperation value, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Id = id;
            Value = Operation.SetParentOperation(value, this);
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
            visitor.VisitFlowCapture(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitFlowCapture(this, argument);
        }

        public FlowCaptureOperation(int id, SyntaxNode syntax, IOperation value)
            : this(new CaptureId(id), value, null, syntax, isImplicit: true)
        {
        }
    }
}
