using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class DynamicObjectCreationOperation : HasDynamicArgumentsExpression, IDynamicObjectCreationOperation, IOperation
    {
        public IObjectOrCollectionInitializerOperation? Initializer { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.DynamicObjectCreation;

        public DynamicObjectCreationOperation(IObjectOrCollectionInitializerOperation? initializer, ImmutableArray<IOperation> arguments, ImmutableArray<string> argumentNames, ImmutableArray<RefKind> argumentRefKinds, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(arguments, argumentNames, argumentRefKinds, semanticModel, syntax, type, isImplicit)
        {
            Initializer = Operation.SetParentOperation(initializer, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (index < base.Arguments.Length)
                    {
                        return base.Arguments[index];
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
            visitor.VisitDynamicObjectCreation(this);
        }

        public override TResult? Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument) where TResult : default
        {
            return visitor.VisitDynamicObjectCreation(this, argument);
        }
    }
}
