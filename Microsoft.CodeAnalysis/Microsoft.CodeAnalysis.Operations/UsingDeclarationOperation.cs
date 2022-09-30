using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public sealed class UsingDeclarationOperation : Operation, IUsingDeclarationOperation, IOperation
    {
        public IVariableDeclarationGroupOperation DeclarationGroup { get; }

        public bool IsAsynchronous { get; }

        public DisposeOperationInfo DisposeInfo { get; }

        public override ITypeSymbol? Type => null;

        internal override ConstantValue? OperationConstantValue => null;

        public override OperationKind Kind => OperationKind.UsingDeclaration;

        public UsingDeclarationOperation(IVariableDeclarationGroupOperation declarationGroup, bool isAsynchronous, DisposeOperationInfo disposeInfo, SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
            : base(semanticModel, syntax, isImplicit)
        {
            DeclarationGroup = Operation.SetParentOperation(declarationGroup, this);
            IsAsynchronous = isAsynchronous;
            DisposeInfo = disposeInfo;
        }

        protected override IOperation GetCurrent(int slot, int index)
        {
            if (slot == 0 && DeclarationGroup != null)
            {
                return DeclarationGroup;
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
            else if (DeclarationGroup != null)
            {
                return (true, 0, 0);
            }
            return (false, 1, 0);
        }

        public override void Accept(OperationVisitor visitor)
        {
            visitor.VisitUsingDeclaration(this);
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitUsingDeclaration(this, argument);
        }
    }
}
