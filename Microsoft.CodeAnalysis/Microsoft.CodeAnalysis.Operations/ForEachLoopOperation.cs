using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class ForEachLoopOperation : BaseLoopOperation, IForEachLoopOperation, ILoopOperation, IOperation
    {
        public IOperation LoopControlVariable { get; }

        public IOperation Collection { get; }

        public ImmutableArray<IOperation> NextVariables { get; }

        public ForEachLoopOperationInfo? Info { get; }

        public bool IsAsynchronous { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.Loop;

        public override LoopKind LoopKind => LoopKind.ForEach;

        public ForEachLoopOperation(IOperation loopControlVariable, IOperation collection, ImmutableArray<IOperation> nextVariables, ForEachLoopOperationInfo? info, bool isAsynchronous, IOperation body, ImmutableArray<ILocalSymbol> locals, ILabelSymbol continueLabel, ILabelSymbol exitLabel, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(body, locals, continueLabel, exitLabel, semanticModel, syntax, isImplicit)
        {
            LoopControlVariable = Operation.SetParentOperation(loopControlVariable, this);
            Collection = Operation.SetParentOperation(collection, this);
            NextVariables = Operation.SetParentOperation(nextVariables, this);
            Info = info;
            IsAsynchronous = isAsynchronous;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Collection != null)
                    {
                        return Collection;
                    }
                    break;
                case 1:
                    if (LoopControlVariable != null)
                    {
                        return LoopControlVariable;
                    }
                    break;
                case 2:
                    if (base.Body != null)
                    {
                        return base.Body;
                    }
                    break;
                case 3:
                    if (index < NextVariables.Length)
                    {
                        return NextVariables[index];
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
                    if (Collection != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (LoopControlVariable != null)
                    {
                        return (true, 1, 0);
                    }
                    goto case 1;
                case 1:
                    if (base.Body != null)
                    {
                        return (true, 2, 0);
                    }
                    goto case 2;
                case 2:
                    if (!NextVariables.IsEmpty)
                    {
                        return (true, 3, 0);
                    }
                    goto case 4;
                case 3:
                    if (previousIndex + 1 < NextVariables.Length)
                    {
                        return (true, 3, previousIndex + 1);
                    }
                    goto case 4;
                case 4:
                    return (false, 4, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitForEachLoop(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitForEachLoop(this, argument);
        }
    }
}
