using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class CoalesceOperation : Operation, ICoalesceOperation, IOperation
    {
        public IOperation Value { get; }

        public IOperation WhenNull { get; }

        internal IConvertibleConversion ValueConversionConvertible { get; }

        public CommonConversion ValueConversion => ValueConversionConvertible.ToCommonConversion();

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue { get; }

        public override OperationKind Kind => OperationKind.Coalesce;

        public CoalesceOperation(IOperation value, IOperation whenNull, IConvertibleConversion valueConversion, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, ConstantValue? constantValue, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Value = Operation.SetParentOperation(value, this);
            WhenNull = Operation.SetParentOperation(whenNull, this);
            ValueConversionConvertible = valueConversion;
            OperationConstantValue = constantValue;
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Value != null)
                    {
                        return Value;
                    }
                    break;
                case 1:
                    if (WhenNull != null)
                    {
                        return WhenNull;
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
                    if (Value != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (WhenNull != null)
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
            visitor.VisitCoalesce(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitCoalesce(this, argument);
        }
    }
}
