using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class MemberInitializerOperation : Operation, IMemberInitializerOperation, IOperation
    {
        public IOperation InitializedMember { get; }

        public IObjectOrCollectionInitializerOperation Initializer { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.MemberInitializer;

        public MemberInitializerOperation(IOperation initializedMember, IObjectOrCollectionInitializerOperation initializer, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            InitializedMember = Operation.SetParentOperation(initializedMember, this);
            Initializer = Operation.SetParentOperation(initializer, this);
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (InitializedMember != null)
                    {
                        return InitializedMember;
                    }
                    break;
                case 1:
                    if (Initializer != null)
                    {
                        return Initializer;
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
                    if (InitializedMember != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (Initializer != null)
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
            visitor.VisitMemberInitializer(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitMemberInitializer(this, argument);
        }
    }
}
