using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class InterpolationOperation : BaseInterpolatedStringContentOperation, IInterpolationOperation, IInterpolatedStringContentOperation, IOperation
    {
        public IOperation Expression { get; }

        public IOperation? Alignment { get; }

        public IOperation? FormatString { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.Interpolation;

        public InterpolationOperation(IOperation expression, IOperation? alignment, IOperation? formatString, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Expression = Operation.SetParentOperation(expression, this);
            Alignment = Operation.SetParentOperation(alignment, this);
            FormatString = Operation.SetParentOperation(formatString, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Expression != null)
                    {
                        return Expression;
                    }
                    break;
                case 1:
                    if (Alignment != null)
                    {
                        return Alignment;
                    }
                    break;
                case 2:
                    if (FormatString != null)
                    {
                        return FormatString;
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
                    if (Expression != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (Alignment != null)
                    {
                        return (true, 1, 0);
                    }
                    goto case 1;
                case 1:
                    if (FormatString != null)
                    {
                        return (true, 2, 0);
                    }
                    goto case 2;
                case 2:
                case 3:
                    return (false, 3, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitInterpolation(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitInterpolation(this, argument);
        }
    }
}