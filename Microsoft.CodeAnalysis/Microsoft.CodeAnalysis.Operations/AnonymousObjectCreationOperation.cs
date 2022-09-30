using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class AnonymousObjectCreationOperation : Operation, IAnonymousObjectCreationOperation, IOperation
    {
        public ImmutableArray<IOperation> Initializers { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.AnonymousObjectCreation;

        public AnonymousObjectCreationOperation(ImmutableArray<IOperation> initializers, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Initializers = Operation.SetParentOperation(initializers, this);
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && index < Initializers.Length)
            {
                return Initializers[index];
            }
            throw ExceptionUtilities.UnexpectedValue((slot, index));
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            switch (previousSlot)
            {
                case -1:
                    if (!Initializers.IsEmpty)
                    {
                        return (true, 0, 0);
                    }
                    goto case 1;
                case 0:
                    if (previousIndex + 1 < Initializers.Length)
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
            visitor.VisitAnonymousObjectCreation(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitAnonymousObjectCreation(this, argument);
        }
    }
}
