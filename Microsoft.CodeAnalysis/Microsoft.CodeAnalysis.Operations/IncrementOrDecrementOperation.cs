using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class IncrementOrDecrementOperation : Operation, IIncrementOrDecrementOperation, IOperation
    {
        public bool IsPostfix { get; }

        public bool IsLifted { get; }

        public bool IsChecked { get; }

        public IOperation Target { get; }

        public IMethodSymbol? OperatorMethod { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind { get; }

        public IncrementOrDecrementOperation(bool isPostfix, bool isLifted, bool isChecked, IOperation target, IMethodSymbol? operatorMethod, OperationKind kind, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            IsPostfix = isPostfix;
            IsLifted = isLifted;
            IsChecked = isChecked;
            Target = Operation.SetParentOperation(target, this);
            OperatorMethod = operatorMethod;
            Type = type;
            Kind = kind;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && Target != null)
            {
                return Target;
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
            else if (Target != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitIncrementOrDecrement(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitIncrementOrDecrement(this, argument);
        }
    }
}
