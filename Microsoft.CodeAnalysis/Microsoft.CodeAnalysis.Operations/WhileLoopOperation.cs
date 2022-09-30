using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class WhileLoopOperation : BaseLoopOperation, IWhileLoopOperation, ILoopOperation, IOperation
    {
        public IOperation? Condition { get; }

        public bool ConditionIsTop { get; }

        public bool ConditionIsUntil { get; }

        public IOperation? IgnoredCondition { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.Loop;

        public override LoopKind LoopKind => LoopKind.While;

        public WhileLoopOperation(IOperation? condition, bool conditionIsTop, bool conditionIsUntil, IOperation? ignoredCondition, IOperation body, ImmutableArray<ILocalSymbol> locals, ILabelSymbol continueLabel, ILabelSymbol exitLabel, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(body, locals, continueLabel, exitLabel, semanticModel, syntax, isImplicit)
        {
            Condition = Operation.SetParentOperation(condition, this);
            ConditionIsTop = conditionIsTop;
            ConditionIsUntil = conditionIsUntil;
            IgnoredCondition = Operation.SetParentOperation(ignoredCondition, this);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitWhileLoop(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitWhileLoop(this, argument);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (!ConditionIsTop)
            {
                return getCurrentSwitchBottom();
            }
            return getCurrentSwitchTop();
            IOperation getCurrentSwitchBottom()
            {
                switch (slot)
                {
                    case 0:
                        if (base.Body != null)
                        {
                            return base.Body;
                        }
                        break;
                    case 1:
                        if (Condition != null)
                        {
                            return Condition;
                        }
                        break;
                    case 2:
                        if (IgnoredCondition != null)
                        {
                            return IgnoredCondition;
                        }
                        break;
                }
                throw ExceptionUtilities.UnexpectedValue((slot, index));
            }
            IOperation getCurrentSwitchTop()
            {
                switch (slot)
                {
                    case 0:
                        if (Condition != null)
                        {
                            return Condition;
                        }
                        break;
                    case 1:
                        if (base.Body != null)
                        {
                            return base.Body;
                        }
                        break;
                    case 2:
                        if (IgnoredCondition != null)
                        {
                            return IgnoredCondition;
                        }
                        break;
                }
                throw ExceptionUtilities.UnexpectedValue((slot, index));
            }
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            if (!ConditionIsTop)
            {
                return moveNextConditionIsBottom();
            }
            return moveNextConditionIsTop();
            (bool hasNext, int nextSlot, int nextIndex) moveNextConditionIsBottom()
            {
                switch (previousSlot)
                {
                    case -1:
                        if (base.Body != null)
                        {
                            return (true, 0, 0);
                        }
                        goto case 0;
                    case 0:
                        if (Condition != null)
                        {
                            return (true, 1, 0);
                        }
                        goto case 1;
                    case 1:
                        if (IgnoredCondition != null)
                        {
                            return (true, 2, 0);
                        }
                        goto case 2;
                    case 2:
                    case 3:
                        return (false, 3, 0);
                    default:
                        throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
                }
            }
            (bool hasNext, int nextSlot, int nextIndex) moveNextConditionIsTop()
            {
                switch (previousSlot)
                {
                    case -1:
                        if (Condition != null)
                        {
                            return (true, 0, 0);
                        }
                        goto case 0;
                    case 0:
                        if (base.Body != null)
                        {
                            return (true, 1, 0);
                        }
                        goto case 1;
                    case 1:
                        if (IgnoredCondition != null)
                        {
                            return (true, 2, 0);
                        }
                        goto case 2;
                    case 2:
                    case 3:
                        return (false, 3, 0);
                    default:
                        throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
                }
            }
        }
    }
}
