using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class ArrayCreationOperation : Operation, IArrayCreationOperation, IOperation
    {
        public ImmutableArray<IOperation> DimensionSizes { get; }

        public IArrayInitializerOperation? Initializer { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.ArrayCreation;

        public ArrayCreationOperation(ImmutableArray<IOperation> dimensionSizes, IArrayInitializerOperation? initializer, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            DimensionSizes = Operation.SetParentOperation(dimensionSizes, this);
            Initializer = Operation.SetParentOperation(initializer, this);
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (index < DimensionSizes.Length)
                    {
                        return DimensionSizes[index];
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
                    if (!DimensionSizes.IsEmpty)
                    {
                        return (true, 0, 0);
                    }
                    goto IL_0053;
                case 0:
                    if (previousIndex + 1 < DimensionSizes.Length)
                    {
                        return (true, 0, previousIndex + 1);
                    }
                    goto IL_0053;
                case 1:
                case 2:
                    return (false, 2, 0);
                default:
                    {
                        throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
                    }
                IL_0053:
                    if (Initializer != null)
                    {
                        return (true, 1, 0);
                    }
                    goto case 1;
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitArrayCreation(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitArrayCreation(this, argument);
        }
    }
}
