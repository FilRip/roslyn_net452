using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class FieldReferenceOperation : BaseMemberReferenceOperation, IFieldReferenceOperation, IMemberReferenceOperation, IOperation
    {
        public IFieldSymbol Field { get; }

        public bool IsDeclaration { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue { get; }

        public override OperationKind Kind => OperationKind.FieldReference;

        public override ISymbol Member => Field;

        public FieldReferenceOperation(IFieldSymbol field, bool isDeclaration, IOperation? instance, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, ConstantValue? constantValue, bool isImplicit)
            : base(instance, semanticModel, syntax, isImplicit)
        {
            Field = field;
            IsDeclaration = isDeclaration;
            OperationConstantValue = constantValue;
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && base.Instance != null)
            {
                return base.Instance;
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
            else if (base.Instance != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitFieldReference(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitFieldReference(this, argument);
        }
    }
}
