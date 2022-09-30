using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class SimpleAssignmentOperation : BaseAssignmentOperation, ISimpleAssignmentOperation, IAssignmentOperation, IOperation
    {
        public bool IsRef { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue { get; }

        public override OperationKind Kind => OperationKind.SimpleAssignment;

        public SimpleAssignmentOperation(bool isRef, IOperation target, IOperation value, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, ConstantValue? constantValue, bool isImplicit)
            : base(target, value, semanticModel, syntax, isImplicit)
        {
            IsRef = isRef;
            OperationConstantValue = constantValue;
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (base.Target != null)
                    {
                        return base.Target;
                    }
                    break;
                case 1:
                    if (base.Value != null)
                    {
                        return base.Value;
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
                    if (base.Target != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (base.Value != null)
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
            visitor.VisitSimpleAssignment(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitSimpleAssignment(this, argument);
        }
    }
}
