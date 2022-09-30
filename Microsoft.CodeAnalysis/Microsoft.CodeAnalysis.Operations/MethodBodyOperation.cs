using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class MethodBodyOperation : BaseMethodBodyBaseOperation, IMethodBodyOperation, IMethodBodyBaseOperation, IOperation
    {
        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.MethodBody;

        public MethodBodyOperation(IBlockOperation? blockBody, IBlockOperation? expressionBody, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(blockBody, expressionBody, semanticModel, syntax, isImplicit)
        {
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (base.BlockBody != null)
                    {
                        return base.BlockBody;
                    }
                    break;
                case 1:
                    if (base.ExpressionBody != null)
                    {
                        return base.ExpressionBody;
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
                    if (base.BlockBody != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (base.ExpressionBody != null)
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
            visitor.VisitMethodBodyOperation(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitMethodBodyOperation(this, argument);
        }
    }
}
