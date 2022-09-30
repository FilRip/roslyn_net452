using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class ConversionOperation : Operation, IConversionOperation, IOperation
    {
        public IOperation Operand { get; }

        public IConvertibleConversion ConversionConvertible { get; }

        public CommonConversion Conversion => ConversionConvertible.ToCommonConversion();

        public bool IsTryCast { get; }

        public bool IsChecked { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue { get; }

        public override OperationKind Kind => OperationKind.Conversion;

        public IMethodSymbol? OperatorMethod => Conversion.MethodSymbol;

        public ConversionOperation(IOperation operand, IConvertibleConversion conversion, bool isTryCast, bool isChecked, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, ConstantValue? constantValue, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Operand = Operation.SetParentOperation(operand, this);
            ConversionConvertible = conversion;
            IsTryCast = isTryCast;
            IsChecked = isChecked;
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
            visitor.VisitConversion(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitConversion(this, argument);
        }
    }
}
