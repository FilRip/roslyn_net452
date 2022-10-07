using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class ObjectCreationOperation : Operation, IObjectCreationOperation, IOperation
    {
        public IMethodSymbol? Constructor { get; }

        public IObjectOrCollectionInitializerOperation? Initializer { get; }

        public ImmutableArray<IArgumentOperation> Arguments { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue { get; }

        public override OperationKind Kind => OperationKind.ObjectCreation;

        public ObjectCreationOperation(IMethodSymbol? constructor, IObjectOrCollectionInitializerOperation? initializer, ImmutableArray<IArgumentOperation> arguments, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, ConstantValue? constantValue, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Constructor = constructor;
            Initializer = Operation.SetParentOperation(initializer, this);
            Arguments = Operation.SetParentOperation(arguments, this);
            OperationConstantValue = constantValue;
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (index < Arguments.Length)
                    {
                        return Arguments[index];
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
                    if (!Arguments.IsEmpty) return (true, 0, 0);
                    else goto case 0;
                case 0 when previousIndex + 1 < Arguments.Length:
                    return (true, 0, previousIndex + 1);
                case 0:
                    if (Initializer != null) return (true, 1, 0);
                    else goto case 1;
                case 1:
                case 2:
                    return (false, 2, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitObjectCreation(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitObjectCreation(this, argument);
        }
    }
}
