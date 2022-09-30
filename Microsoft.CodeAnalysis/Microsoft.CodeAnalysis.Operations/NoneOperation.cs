using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class NoneOperation : Operation
    {
        internal ImmutableArray<IOperation> Children { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue { get; }

        public override OperationKind Kind => OperationKind.None;

        public NoneOperation(ImmutableArray<IOperation> children, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, ConstantValue? constantValue, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Children = Operation.SetParentOperation(children, this);
            Type = type;
            OperationConstantValue = constantValue;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && index < Children.Length)
            {
                return Children[index];
            }
            throw ExceptionUtilities.UnexpectedValue((slot, index));
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            switch (previousSlot)
            {
                case -1:
                    if (!Children.IsEmpty)
                    {
                        return (true, 0, 0);
                    }
                    goto case 1;
                case 0:
                    if (previousIndex + 1 < Children.Length)
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
            visitor.VisitNoneOperation(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitNoneOperation(this, argument);
        }
    }
}
