using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class FixedOperation : Operation, IFixedOperation, IOperation
    {
        public ImmutableArray<ILocalSymbol> Locals { get; }

        public IVariableDeclarationGroupOperation Variables { get; }

        public IOperation Body { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.None;

        public FixedOperation(ImmutableArray<ILocalSymbol> locals, IVariableDeclarationGroupOperation variables, IOperation body, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Locals = locals;
            Variables = Operation.SetParentOperation(variables, this);
            Body = Operation.SetParentOperation(body, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Variables != null)
                    {
                        return Variables;
                    }
                    break;
                case 1:
                    if (Body != null)
                    {
                        return Body;
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
                    if (Variables != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (Body != null)
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
            visitor.VisitFixed(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitFixed(this, argument);
        }
    }
}
