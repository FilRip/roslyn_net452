using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundConditionalAccess : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Receiver { get; }

        public BoundExpression AccessExpression { get; }

        public BoundConditionalAccess(SyntaxNode syntax, BoundExpression receiver, BoundExpression accessExpression, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ConditionalAccess, syntax, type, hasErrors || receiver.HasErrors() || accessExpression.HasErrors())
        {
            Receiver = receiver;
            AccessExpression = accessExpression;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitConditionalAccess(this);
        }

        public BoundConditionalAccess Update(BoundExpression receiver, BoundExpression accessExpression, TypeSymbol type)
        {
            if (receiver != Receiver || accessExpression != AccessExpression || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundConditionalAccess boundConditionalAccess = new BoundConditionalAccess(Syntax, receiver, accessExpression, type, base.HasErrors);
                boundConditionalAccess.CopyAttributes(this);
                return boundConditionalAccess;
            }
            return this;
        }
    }
}
