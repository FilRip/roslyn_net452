using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundWithExpression : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Receiver { get; }

        public MethodSymbol? CloneMethod { get; }

        public BoundObjectInitializerExpressionBase InitializerExpression { get; }

        public BoundWithExpression(SyntaxNode syntax, BoundExpression receiver, MethodSymbol? cloneMethod, BoundObjectInitializerExpressionBase initializerExpression, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.WithExpression, syntax, type, hasErrors || receiver.HasErrors() || initializerExpression.HasErrors())
        {
            Receiver = receiver;
            CloneMethod = cloneMethod;
            InitializerExpression = initializerExpression;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitWithExpression(this);
        }

        public BoundWithExpression Update(BoundExpression receiver, MethodSymbol? cloneMethod, BoundObjectInitializerExpressionBase initializerExpression, TypeSymbol type)
        {
            if (receiver != Receiver || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(cloneMethod, CloneMethod) || initializerExpression != InitializerExpression || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundWithExpression boundWithExpression = new BoundWithExpression(Syntax, receiver, cloneMethod, initializerExpression, type, base.HasErrors);
                boundWithExpression.CopyAttributes(this);
                return boundWithExpression;
            }
            return this;
        }
    }
}
