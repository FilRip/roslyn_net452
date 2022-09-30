using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class InvocationOperation : Operation, IInvocationOperation, IOperation
    {
        public IMethodSymbol TargetMethod { get; }

        public IOperation? Instance { get; }

        public bool IsVirtual { get; }

        public ImmutableArray<IArgumentOperation> Arguments { get; }

        public override ITypeSymbol? Type { get; }

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.Invocation;

        public InvocationOperation(IMethodSymbol targetMethod, IOperation? instance, bool isVirtual, ImmutableArray<IArgumentOperation> arguments, SemanticModel? semanticModel, SyntaxNode syntax, ITypeSymbol? type, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            TargetMethod = targetMethod;
            Instance = Operation.SetParentOperation(instance, this);
            IsVirtual = isVirtual;
            Arguments = Operation.SetParentOperation(arguments, this);
            Type = type;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            switch (slot)
            {
                case 0:
                    if (Instance != null)
                    {
                        return Instance;
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
                    if (Instance != null)
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
            visitor.VisitInvocation(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitInvocation(this, argument);
        }
    }
}
