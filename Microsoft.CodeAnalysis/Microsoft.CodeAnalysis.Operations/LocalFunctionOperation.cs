using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class LocalFunctionOperation : Operation, ILocalFunctionOperation, IOperation
    {
        public IMethodSymbol Symbol { get; }

        public IBlockOperation? Body { get; }

        public IBlockOperation? IgnoredBody { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.LocalFunction;

        public LocalFunctionOperation(IMethodSymbol symbol, IBlockOperation? body, IBlockOperation? ignoredBody, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Symbol = symbol;
            Body = Operation.SetParentOperation(body, this);
            IgnoredBody = Operation.SetParentOperation(ignoredBody, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Body != null)
                    {
                        return Body;
                    }
                    break;
                case 1:
                    if (IgnoredBody != null)
                    {
                        return IgnoredBody;
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
                    if (Body != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (IgnoredBody != null)
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
            visitor.VisitLocalFunction(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitLocalFunction(this, argument);
        }
    }
}
