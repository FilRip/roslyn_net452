using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundConditionalReceiver : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public int Id { get; }

        public BoundConditionalReceiver(SyntaxNode syntax, int id, TypeSymbol type, bool hasErrors)
            : base(BoundKind.ConditionalReceiver, syntax, type, hasErrors)
        {
            Id = id;
        }

        public BoundConditionalReceiver(SyntaxNode syntax, int id, TypeSymbol type)
            : base(BoundKind.ConditionalReceiver, syntax, type)
        {
            Id = id;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitConditionalReceiver(this);
        }

        public BoundConditionalReceiver Update(int id, TypeSymbol type)
        {
            if (id != Id || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundConditionalReceiver boundConditionalReceiver = new BoundConditionalReceiver(Syntax, id, type, base.HasErrors);
                boundConditionalReceiver.CopyAttributes(this);
                return boundConditionalReceiver;
            }
            return this;
        }
    }
}
