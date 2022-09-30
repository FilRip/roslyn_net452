using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class MethodReferenceOperation : BaseMemberReferenceOperation, IMethodReferenceOperation, IMemberReferenceOperation, IOperation
    {
        public IMethodSymbol Method { get; }

        public bool IsVirtual { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.MethodReference;

        public override ISymbol Member => Method;

        public MethodReferenceOperation(IMethodSymbol method, bool isVirtual, IOperation? instance, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(instance, semanticModel, syntax, isImplicit)
        {
            Method = method;
            IsVirtual = isVirtual;
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && base.Instance != null)
            {
                return base.Instance;
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
            else if (base.Instance != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitMethodReference(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitMethodReference(this, argument);
        }
    }
}
