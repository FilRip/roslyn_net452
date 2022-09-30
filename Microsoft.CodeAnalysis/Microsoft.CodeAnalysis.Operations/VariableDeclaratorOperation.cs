using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class VariableDeclaratorOperation : Operation, IVariableDeclaratorOperation, IOperation
    {
        public ILocalSymbol Symbol { get; }

        public IVariableInitializerOperation? Initializer { get; }

        public ImmutableArray<IOperation> IgnoredArguments { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.VariableDeclarator;

        public VariableDeclaratorOperation(ILocalSymbol symbol, IVariableInitializerOperation? initializer, ImmutableArray<IOperation> ignoredArguments, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Symbol = symbol;
            Initializer = Operation.SetParentOperation(initializer, this);
            IgnoredArguments = Operation.SetParentOperation(ignoredArguments, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (index < IgnoredArguments.Length)
                    {
                        return IgnoredArguments[index];
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
                    if (!IgnoredArguments.IsEmpty)
                    {
                        return (true, 0, 0);
                    }
                    goto IL_0053;
                case 0:
                    if (previousIndex + 1 < IgnoredArguments.Length)
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
            visitor.VisitVariableDeclarator(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitVariableDeclarator(this, argument);
        }
    }
}
