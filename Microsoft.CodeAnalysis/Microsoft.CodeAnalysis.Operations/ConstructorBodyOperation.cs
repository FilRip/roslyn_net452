using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class ConstructorBodyOperation : BaseMethodBodyBaseOperation, IConstructorBodyOperation, IMethodBodyBaseOperation, IOperation
    {
        public ImmutableArray<ILocalSymbol> Locals { get; }

        public IOperation? Initializer { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.ConstructorBody;

        public ConstructorBodyOperation(ImmutableArray<ILocalSymbol> locals, IOperation? initializer, IBlockOperation? blockBody, IBlockOperation? expressionBody, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(blockBody, expressionBody, semanticModel, syntax, isImplicit)
        {
            Locals = locals;
            Initializer = Operation.SetParentOperation(initializer, this);
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Initializer != null)
                    {
                        return Initializer;
                    }
                    break;
                case 1:
                    if (base.BlockBody != null)
                    {
                        return base.BlockBody;
                    }
                    break;
                case 2:
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
                    if (Initializer != null)
                    {
                        return (true, 0, 0);
                    }
                    goto case 0;
                case 0:
                    if (base.BlockBody != null)
                    {
                        return (true, 1, 0);
                    }
                    goto case 1;
                case 1:
                    if (base.ExpressionBody != null)
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
            visitor.VisitConstructorBodyOperation(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitConstructorBodyOperation(this, argument);
        }
    }
}
