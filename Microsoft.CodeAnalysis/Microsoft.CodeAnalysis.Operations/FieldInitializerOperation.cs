using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class FieldInitializerOperation : BaseSymbolInitializerOperation, IFieldInitializerOperation, ISymbolInitializerOperation, IOperation
    {
        public ImmutableArray<IFieldSymbol> InitializedFields { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.FieldInitializer;

        public FieldInitializerOperation(ImmutableArray<IFieldSymbol> initializedFields, ImmutableArray<ILocalSymbol> locals, IOperation value, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(locals, value, semanticModel, syntax, isImplicit)
        {
            InitializedFields = initializedFields;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && base.Value != null)
            {
                return base.Value;
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
            else if (base.Value != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitFieldInitializer(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitFieldInitializer(this, argument);
        }
    }
}
