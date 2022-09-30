using Microsoft.CodeAnalysis.FlowAnalysis;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    internal sealed class FlowCaptureReferenceOperation : Operation, IFlowCaptureReferenceOperation, IOperation
    {
        public CaptureId Id { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue { get; }

        public override OperationKind Kind => OperationKind.FlowCaptureReference;

        internal FlowCaptureReferenceOperation(CaptureId id, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, ConstantValue? constantValue, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Id = id;
            OperationConstantValue = constantValue;
            Type = type;
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
            visitor.VisitFlowCaptureReference(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitFlowCaptureReference(this, argument);
        }

        public FlowCaptureReferenceOperation(int id, SyntaxNode syntax, ITypeSymbol? type, ConstantValue? constantValue)
            : this(new CaptureId(id), null, syntax, type, constantValue, isImplicit: true)
        {
        }
    }
}
