using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class ArgumentOperation : Operation, IArgumentOperation, IOperation
    {
        public ArgumentKind ArgumentKind { get; }

        public IParameterSymbol? Parameter { get; }

        public IOperation Value { get; }

        internal IConvertibleConversion InConversionConvertible { get; }

        public CommonConversion InConversion => InConversionConvertible.ToCommonConversion();

        internal IConvertibleConversion OutConversionConvertible { get; }

        public CommonConversion OutConversion => OutConversionConvertible.ToCommonConversion();

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.Argument;

        public ArgumentOperation(ArgumentKind argumentKind, IParameterSymbol? parameter, IOperation value, IConvertibleConversion inConversion, IConvertibleConversion outConversion, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            ArgumentKind = argumentKind;
            Parameter = parameter;
            Value = Operation.SetParentOperation(value, this);
            InConversionConvertible = inConversion;
            OutConversionConvertible = outConversion;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && Value != null)
            {
                return Value;
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
            else if (Value != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitArgument(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitArgument(this, argument);
        }
    }
}
