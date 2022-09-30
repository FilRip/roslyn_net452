using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class BlockOperation : Operation, IBlockOperation, IOperation
    {
        public ImmutableArray<IOperation> Operations { get; }

        public ImmutableArray<ILocalSymbol> Locals { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.Block;

        public BlockOperation(ImmutableArray<IOperation> operations, ImmutableArray<ILocalSymbol> locals, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Operations = Operation.SetParentOperation(operations, this);
            Locals = locals;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && index < Operations.Length)
            {
                return Operations[index];
            }
            throw ExceptionUtilities.UnexpectedValue((slot, index));
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            switch (previousSlot)
            {
                case -1:
                    if (!Operations.IsEmpty)
                    {
                        return (true, 0, 0);
                    }
                    goto case 1;
                case 0:
                    if (previousIndex + 1 < Operations.Length)
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
            visitor.VisitBlock(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitBlock(this, argument);
        }

        public static BlockOperation CreateTemporaryBlock(ImmutableArray<IOperation> statements, SemanticModel semanticModel, SyntaxNode syntax)
        {
            return new BlockOperation(statements, semanticModel, syntax);
        }

        private BlockOperation(ImmutableArray<IOperation> statements, SemanticModel semanticModel, SyntaxNode syntax)
            : base(semanticModel, syntax, isImplicit: true)
        {
            Operations = statements;
            Locals = ImmutableArray<ILocalSymbol>.Empty;
        }
    }
}
