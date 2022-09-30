using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class AnonymousFunctionOperation : Operation, IAnonymousFunctionOperation, IOperation
    {
        public IMethodSymbol Symbol { get; }

        public IBlockOperation Body { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.AnonymousFunction;

        public AnonymousFunctionOperation(IMethodSymbol symbol, IBlockOperation body, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Symbol = symbol;
            Body = Operation.SetParentOperation(body, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && Body != null)
            {
                return Body;
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
            else if (Body != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitAnonymousFunction(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitAnonymousFunction(this, argument);
        }
    }
}
