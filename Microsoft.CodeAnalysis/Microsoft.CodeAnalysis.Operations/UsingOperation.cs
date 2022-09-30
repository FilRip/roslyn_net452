using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class UsingOperation : Operation, IUsingOperation, IOperation
    {
        public IOperation Resources { get; }

        public IOperation Body { get; }

        public ImmutableArray<ILocalSymbol> Locals { get; }

        public bool IsAsynchronous { get; }

        public DisposeOperationInfo DisposeInfo { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.Using;

        public UsingOperation(IOperation resources, IOperation body, ImmutableArray<ILocalSymbol> locals, bool isAsynchronous, DisposeOperationInfo disposeInfo, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Resources = Operation.SetParentOperation(resources, this);
            Body = Operation.SetParentOperation(body, this);
            Locals = locals;
            IsAsynchronous = isAsynchronous;
            DisposeInfo = disposeInfo;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Resources != null)
                    {
                        return Resources;
                    }
                    break;
                case 1:
                    if (Body != null)
                    {
                        return Body;
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
                    if (Resources != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (Body != null)
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
            visitor.VisitUsing(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitUsing(this, argument);
        }
    }
}
