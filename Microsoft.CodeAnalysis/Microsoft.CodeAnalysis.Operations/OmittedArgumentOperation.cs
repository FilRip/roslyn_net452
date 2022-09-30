using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class OmittedArgumentOperation : Operation, IOmittedArgumentOperation, IOperation
    {
        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.OmittedArgument;

        public OmittedArgumentOperation(SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
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
            visitor.VisitOmittedArgument(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitOmittedArgument(this, argument);
        }
    }
}
