using Microsoft.CodeAnalysis.FlowAnalysis;

using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    internal sealed class FlowAnonymousFunctionOperation : Operation, IFlowAnonymousFunctionOperation, IOperation
    {
        public readonly ControlFlowGraphBuilder.Context Context;

        public readonly IAnonymousFunctionOperation Original;

        public IMethodSymbol Symbol => Original.Symbol;

        public override OperationKind Kind => OperationKind.FlowAnonymousFunction;

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public FlowAnonymousFunctionOperation(in ControlFlowGraphBuilder.Context context, IAnonymousFunctionOperation original, bool isImplicit)
            : base(null, original.Syntax, isImplicit)
        {
            Context = context;
            Original = original;
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
            visitor.VisitFlowAnonymousFunction(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitFlowAnonymousFunction(this, argument);
        }
    }
}
