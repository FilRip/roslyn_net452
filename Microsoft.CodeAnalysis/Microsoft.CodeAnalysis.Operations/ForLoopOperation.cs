using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class ForLoopOperation : BaseLoopOperation, IForLoopOperation, ILoopOperation, IOperation
    {
        public ImmutableArray<IOperation> Before { get; }

        public ImmutableArray<ILocalSymbol> ConditionLocals { get; }

        public IOperation? Condition { get; }

        public ImmutableArray<IOperation> AtLoopBottom { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.Loop;

        public override LoopKind LoopKind => LoopKind.For;

        public ForLoopOperation(ImmutableArray<IOperation> before, ImmutableArray<ILocalSymbol> conditionLocals, IOperation? condition, ImmutableArray<IOperation> atLoopBottom, IOperation body, ImmutableArray<ILocalSymbol> locals, ILabelSymbol continueLabel, ILabelSymbol exitLabel, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(body, locals, continueLabel, exitLabel, semanticModel, syntax, isImplicit)
        {
            Before = Operation.SetParentOperation(before, this);
            ConditionLocals = conditionLocals;
            Condition = Operation.SetParentOperation(condition, this);
            AtLoopBottom = Operation.SetParentOperation(atLoopBottom, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (index < Before.Length)
                    {
                        return Before[index];
                    }
                    break;
                case 1:
                    if (Condition != null)
                    {
                        return Condition;
                    }
                    break;
                case 2:
                    if (base.Body != null)
                    {
                        return base.Body;
                    }
                    break;
                case 3:
                    if (index < AtLoopBottom.Length)
                    {
                        return AtLoopBottom[index];
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
                    if (!Before.IsEmpty) return (true, 0, 0);
                    else goto case 0;
                case 0 when previousIndex + 1 < Before.Length:
                    return (true, 0, previousIndex + 1);
                case 0:
                    if (Condition != null) return (true, 1, 0);
                    else goto case 1;
                case 1:
                    if (Body != null) return (true, 2, 0);
                    else goto case 2;
                case 2:
                    if (!AtLoopBottom.IsEmpty) return (true, 3, 0);
                    else goto case 3;
                case 3 when previousIndex + 1 < AtLoopBottom.Length:
                    return (true, 3, previousIndex + 1);
                case 3:
                case 4:
                    return (false, 4, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitForLoop(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitForLoop(this, argument);
        }
    }
}
