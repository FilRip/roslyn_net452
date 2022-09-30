using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class PropertyReferenceOperation : BaseMemberReferenceOperation, IPropertyReferenceOperation, IMemberReferenceOperation, IOperation
    {
        public IPropertySymbol Property { get; }

        public ImmutableArray<IArgumentOperation> Arguments { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.PropertyReference;

        public override ISymbol Member => Property;

        public PropertyReferenceOperation(IPropertySymbol property, ImmutableArray<IArgumentOperation> arguments, IOperation? instance, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(instance, semanticModel, syntax, isImplicit)
        {
            Property = property;
            Arguments = Operation.SetParentOperation(arguments, this);
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (base.Instance != null)
                    {
                        return base.Instance;
                    }
                    break;
                case 1:
                    if (index < Arguments.Length)
                    {
                        return Arguments[index];
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
                    if (base.Instance != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (!Arguments.IsEmpty)
                    {
                        return (true, 1, 0);
                    }
                    goto case 2;
                case 1:
                    if (previousIndex + 1 < Arguments.Length)
                    {
                        return (true, 1, previousIndex + 1);
                    }
                    goto case 2;
                case 2:
                    return (false, 2, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitPropertyReference(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitPropertyReference(this, argument);
        }
    }
}
