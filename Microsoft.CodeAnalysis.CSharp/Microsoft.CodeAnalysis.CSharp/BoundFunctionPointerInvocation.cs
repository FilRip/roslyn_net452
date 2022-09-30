using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Operations;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundFunctionPointerInvocation : BoundExpression, IBoundInvalidNode
    {
        private readonly LookupResultKind _ResultKind;

        public FunctionPointerTypeSymbol FunctionPointer => (FunctionPointerTypeSymbol)InvokedExpression.Type;

        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren => CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(InvokedExpression, Arguments);

        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode>.From(((IBoundInvalidNode)this).InvalidNodeChildren);

        public new TypeSymbol Type => base.Type;

        public BoundExpression InvokedExpression { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public override LookupResultKind ResultKind => _ResultKind;

        public BoundFunctionPointerInvocation(SyntaxNode syntax, BoundExpression invokedExpression, ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> argumentRefKindsOpt, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.FunctionPointerInvocation, syntax, type, hasErrors || invokedExpression.HasErrors() || arguments.HasErrors())
        {
            InvokedExpression = invokedExpression;
            Arguments = arguments;
            ArgumentRefKindsOpt = argumentRefKindsOpt;
            _ResultKind = resultKind;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitFunctionPointerInvocation(this);
        }

        public BoundFunctionPointerInvocation Update(BoundExpression invokedExpression, ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> argumentRefKindsOpt, LookupResultKind resultKind, TypeSymbol type)
        {
            if (invokedExpression != InvokedExpression || arguments != Arguments || argumentRefKindsOpt != ArgumentRefKindsOpt || resultKind != ResultKind || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundFunctionPointerInvocation boundFunctionPointerInvocation = new BoundFunctionPointerInvocation(Syntax, invokedExpression, arguments, argumentRefKindsOpt, resultKind, type, base.HasErrors);
                boundFunctionPointerInvocation.CopyAttributes(this);
                return boundFunctionPointerInvocation;
            }
            return this;
        }
    }
}
