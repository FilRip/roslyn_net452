using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class VariableDeclarationOperation : Operation, IVariableDeclarationOperation, IOperation
    {
        public ImmutableArray<IVariableDeclaratorOperation> Declarators { get; }

        public IVariableInitializerOperation? Initializer { get; }

        public ImmutableArray<IOperation> IgnoredDimensions { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.VariableDeclaration;

        public VariableDeclarationOperation(ImmutableArray<IVariableDeclaratorOperation> declarators, IVariableInitializerOperation? initializer, ImmutableArray<IOperation> ignoredDimensions, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Declarators = Operation.SetParentOperation(declarators, this);
            Initializer = Operation.SetParentOperation(initializer, this);
            IgnoredDimensions = Operation.SetParentOperation(ignoredDimensions, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (index < IgnoredDimensions.Length)
                    {
                        return IgnoredDimensions[index];
                    }
                    break;
                case 1:
                    if (index < Declarators.Length)
                    {
                        return Declarators[index];
                    }
                    break;
                case 2:
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
                    if (!IgnoredDimensions.IsEmpty)
                    {
                        return (true, 0, 0);
                    }
                    goto IL_005a;
                case 0:
                    if (previousIndex + 1 < IgnoredDimensions.Length)
                    {
                        return (true, 0, previousIndex + 1);
                    }
                    goto IL_005a;
                case 1:
                    if (previousIndex + 1 < Declarators.Length)
                    {
                        return (true, 1, previousIndex + 1);
                    }
                    goto IL_0091;
                case 2:
                case 3:
                    return (false, 3, 0);
                default:
                    {
                        throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
                    }
                IL_005a:
                    if (!Declarators.IsEmpty)
                    {
                        return (true, 1, 0);
                    }
                    goto IL_0091;
                IL_0091:
                    if (Initializer != null)
                    {
                        return (true, 2, 0);
                    }
                    goto case 2;
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitVariableDeclaration(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitVariableDeclaration(this, argument);
        }
    }
}
