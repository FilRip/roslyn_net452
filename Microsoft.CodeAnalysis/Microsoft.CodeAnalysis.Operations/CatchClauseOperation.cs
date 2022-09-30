using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class CatchClauseOperation : Operation, ICatchClauseOperation, IOperation
    {
        public IOperation? ExceptionDeclarationOrExpression { get; }

        public ITypeSymbol ExceptionType { get; }

        public ImmutableArray<ILocalSymbol> Locals { get; }

        public IOperation? Filter { get; }

        public IBlockOperation Handler { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.CatchClause;

        public CatchClauseOperation(IOperation? exceptionDeclarationOrExpression, ITypeSymbol exceptionType, ImmutableArray<ILocalSymbol> locals, IOperation? filter, IBlockOperation handler, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            ExceptionDeclarationOrExpression = Operation.SetParentOperation(exceptionDeclarationOrExpression, this);
            ExceptionType = exceptionType;
            Locals = locals;
            Filter = Operation.SetParentOperation(filter, this);
            Handler = Operation.SetParentOperation(handler, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (ExceptionDeclarationOrExpression != null)
                    {
                        return ExceptionDeclarationOrExpression;
                    }
                    break;
                case 1:
                    if (Filter != null)
                    {
                        return Filter;
                    }
                    break;
                case 2:
                    if (Handler != null)
                    {
                        return Handler;
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
                    if (ExceptionDeclarationOrExpression != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (Filter != null)
                    {
                        return (true, 1, 0);
                    }
                    goto case 1;
                case 1:
                    if (Handler != null)
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
            visitor.VisitCatchClause(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitCatchClause(this, argument);
        }
    }
}
