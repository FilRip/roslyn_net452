using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class TypeParameterObjectCreationOperation : Operation, ITypeParameterObjectCreationOperation, IOperation
    {
        public IObjectOrCollectionInitializerOperation? Initializer { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.TypeParameterObjectCreation;

        public TypeParameterObjectCreationOperation(IObjectOrCollectionInitializerOperation? initializer, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Initializer = Operation.SetParentOperation(initializer, this);
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && Initializer != null)
            {
                return Initializer;
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
            else if (Initializer != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitTypeParameterObjectCreation(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitTypeParameterObjectCreation(this, argument);
        }
    }
}
