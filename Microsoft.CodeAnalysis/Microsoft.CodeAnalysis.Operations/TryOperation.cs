using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class TryOperation : Operation, ITryOperation, IOperation
    {
        public IBlockOperation Body { get; }

        public ImmutableArray<ICatchClauseOperation> Catches { get; }

        public IBlockOperation? Finally { get; }

        public ILabelSymbol? ExitLabel { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.Try;

        public TryOperation(IBlockOperation body, ImmutableArray<ICatchClauseOperation> catches, IBlockOperation? @finally, ILabelSymbol? exitLabel, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            Body = Operation.SetParentOperation(body, this);
            Catches = Operation.SetParentOperation(catches, this);
            Finally = Operation.SetParentOperation(@finally, this);
            ExitLabel = exitLabel;
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
                    if (index < Catches.Length)
                    {
                        return Catches[index];
                    }
                    break;
                case 2:
                    if (Finally != null)
                    {
                        return Finally;
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
                    if (Body != null) return (true, 0, 0);
                    else goto case 0;
                case 0:
                    if (!Catches.IsEmpty) return (true, 1, 0);
                    else goto case 1;
                case 1 when previousIndex + 1 < Catches.Length:
                    return (true, 1, previousIndex + 1);
                case 1:
                    if (Finally != null) return (true, 2, 0);
                    else goto case 2;
                case 2:
                case 3:
                    return (false, 3, 0);
                default:
                    throw ExceptionUtilities.UnexpectedValue((previousSlot, previousIndex));
            }
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitTry(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitTry(this, argument);
        }
    }
}
