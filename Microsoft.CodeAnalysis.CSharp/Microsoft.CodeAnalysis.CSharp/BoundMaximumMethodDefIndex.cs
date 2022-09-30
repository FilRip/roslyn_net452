using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundMaximumMethodDefIndex : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundMaximumMethodDefIndex(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.MaximumMethodDefIndex, syntax, type, hasErrors)
        {
        }

        public BoundMaximumMethodDefIndex(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.MaximumMethodDefIndex, syntax, type)
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitMaximumMethodDefIndex(this);
        }

        public BoundMaximumMethodDefIndex Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundMaximumMethodDefIndex boundMaximumMethodDefIndex = new BoundMaximumMethodDefIndex(Syntax, type, base.HasErrors);
                boundMaximumMethodDefIndex.CopyAttributes(this);
                return boundMaximumMethodDefIndex;
            }
            return this;
        }
    }
}
