using Microsoft.CodeAnalysis.FlowAnalysis;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    internal sealed class IsNullOperation : Operation, IIsNullOperation, IOperation
    {
        public IOperation Operand { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue { get; }

        public override OperationKind Kind => OperationKind.IsNull;

        internal IsNullOperation(IOperation operand, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, ConstantValue? constantValue, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Operand = Operation.SetParentOperation(operand, this);
            OperationConstantValue = constantValue;
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && Operand != null)
            {
                return Operand;
            }
            throw ExceptionUtilities.UnexpectedValue((slot, index));
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            if (previousSlot != -1)
            {
                if ((uint)previousSlot > 1u)
                {
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
                }
            }
            else if (Operand != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitIsNull(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitIsNull(this, argument);
        }

        public IsNullOperation(SyntaxNode syntax, IOperation operand, ITypeSymbol type, ConstantValue? constantValue)
            : this(operand, null, syntax, type, constantValue, isImplicit: true)
        {
        }
    }
}
