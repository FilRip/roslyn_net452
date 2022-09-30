using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class VariableDeclarationGroupOperation : Operation, IVariableDeclarationGroupOperation, IOperation
    {
        public ImmutableArray<IVariableDeclarationOperation> Declarations { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.VariableDeclarationGroup;

        public VariableDeclarationGroupOperation(ImmutableArray<IVariableDeclarationOperation> declarations, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Declarations = Operation.SetParentOperation(declarations, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && index < Declarations.Length)
            {
                return Declarations[index];
            }
            throw ExceptionUtilities.UnexpectedValue((slot, index));
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            switch (previousSlot)
            {
                case -1:
                    if (!Declarations.IsEmpty)
                    {
                        return (true, 0, 0);
                    }
                    goto case 1;
                case 0:
                    if (previousIndex + 1 < Declarations.Length)
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
            visitor.VisitVariableDeclarationGroup(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitVariableDeclarationGroup(this, argument);
        }
    }
}
