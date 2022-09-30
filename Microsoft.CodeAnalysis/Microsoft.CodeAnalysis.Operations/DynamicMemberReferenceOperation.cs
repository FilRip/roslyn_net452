using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class DynamicMemberReferenceOperation : Operation, IDynamicMemberReferenceOperation, IOperation
    {
        public IOperation? Instance { get; }

        public string MemberName { get; }

        public ImmutableArray<ITypeSymbol> TypeArguments { get; }

        public ITypeSymbol? ContainingType { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.DynamicMemberReference;

        public DynamicMemberReferenceOperation(IOperation? instance, string memberName, ImmutableArray<ITypeSymbol> typeArguments, ITypeSymbol? containingType, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Instance = Operation.SetParentOperation(instance, this);
            MemberName = memberName;
            TypeArguments = typeArguments;
            ContainingType = containingType;
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && Instance != null)
            {
                return Instance;
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
            else if (Instance != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitDynamicMemberReference(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitDynamicMemberReference(this, argument);
        }
    }
}
