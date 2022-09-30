using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class TypeOfOperation : Operation, ITypeOfOperation, IOperation
    {
        public ITypeSymbol TypeOperand { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.TypeOf;

        public TypeOfOperation(ITypeSymbol typeOperand, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            TypeOperand = typeOperand;
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            throw ExceptionUtilities.UnexpectedValue((slot, index));
        }

        protected override (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex)
        {
            return (false, int.MinValue, int.MinValue);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitTypeOf(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitTypeOf(this, argument);
        }
    }
}
