using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class DefaultValueOperation : Operation, IDefaultValueOperation, IOperation
    {
        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue { get; }

        public override OperationKind Kind => OperationKind.DefaultValue;

        public DefaultValueOperation(SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, ConstantValue? constantValue, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            OperationConstantValue = constantValue;
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
            visitor.VisitDefaultValue(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitDefaultValue(this, argument);
        }
    }
}