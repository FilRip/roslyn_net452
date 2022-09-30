using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundBaseReference : BoundExpression
    {
        public override bool SuppressVirtualCalls => true;

        public new TypeSymbol? Type => base.Type;

        public BoundBaseReference(SyntaxNode syntax, TypeSymbol? type, bool hasErrors)
            : base(BoundKind.BaseReference, syntax, type, hasErrors)
        {
        }

        public BoundBaseReference(SyntaxNode syntax, TypeSymbol? type)
            : base(BoundKind.BaseReference, syntax, type)
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitBaseReference(this);
        }

        public BoundBaseReference Update(TypeSymbol? type)
        {
            if (!TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundBaseReference boundBaseReference = new BoundBaseReference(Syntax, type, base.HasErrors);
                boundBaseReference.CopyAttributes(this);
                return boundBaseReference;
            }
            return this;
        }
    }
}
