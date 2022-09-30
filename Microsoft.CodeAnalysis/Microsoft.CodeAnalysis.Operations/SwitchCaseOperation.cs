using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class SwitchCaseOperation : Operation, ISwitchCaseOperation, IOperation
    {
        public ImmutableArray<ICaseClauseOperation> Clauses { get; }

        public ImmutableArray<IOperation> Body { get; }

        public ImmutableArray<ILocalSymbol> Locals { get; }

        public IOperation? Condition { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.SwitchCase;

        public SwitchCaseOperation(ImmutableArray<ICaseClauseOperation> clauses, ImmutableArray<IOperation> body, ImmutableArray<ILocalSymbol> locals, IOperation? condition, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Clauses = Operation.SetParentOperation(clauses, this);
            Body = Operation.SetParentOperation(body, this);
            Locals = locals;
            Condition = Operation.SetParentOperation(condition, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (index < Clauses.Length)
                    {
                        return Clauses[index];
                    }
                    break;
                case 1:
                    if (index < Body.Length)
                    {
                        return Body[index];
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
                    if (!Clauses.IsEmpty)
                    {
                        return (true, 0, 0);
                    }
                    goto IL_0053;
                case 0:
                    if (previousIndex + 1 < Clauses.Length)
                    {
                        return (true, 0, previousIndex + 1);
                    }
                    goto IL_0053;
                case 1:
                    if (previousIndex + 1 < Body.Length)
                    {
                        return (true, 1, previousIndex + 1);
                    }
                    goto case 2;
                case 2:
                    return (false, 2, 0);
                default:
                    {
                        throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
                    }
                IL_0053:
                    if (!Body.IsEmpty)
                    {
                        return (true, 1, 0);
                    }
                    goto case 2;
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitSwitchCase(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitSwitchCase(this, argument);
        }
    }
}
