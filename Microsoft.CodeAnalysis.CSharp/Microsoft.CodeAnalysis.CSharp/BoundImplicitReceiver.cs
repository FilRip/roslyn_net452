using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundImplicitReceiver : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundImplicitReceiver(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.ImplicitReceiver, syntax, type, hasErrors)
        {
        }

        public BoundImplicitReceiver(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.ImplicitReceiver, syntax, type)
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitImplicitReceiver(this);
        }

        public BoundImplicitReceiver Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundImplicitReceiver boundImplicitReceiver = new BoundImplicitReceiver(Syntax, type, base.HasErrors);
                boundImplicitReceiver.CopyAttributes(this);
                return boundImplicitReceiver;
            }
            return this;
        }
    }
}
