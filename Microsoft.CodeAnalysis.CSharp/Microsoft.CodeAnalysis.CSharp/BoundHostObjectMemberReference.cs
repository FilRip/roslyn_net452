using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundHostObjectMemberReference : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundHostObjectMemberReference(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.HostObjectMemberReference, syntax, type, hasErrors)
        {
        }

        public BoundHostObjectMemberReference(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.HostObjectMemberReference, syntax, type)
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitHostObjectMemberReference(this);
        }

        public BoundHostObjectMemberReference Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundHostObjectMemberReference boundHostObjectMemberReference = new BoundHostObjectMemberReference(Syntax, type, base.HasErrors);
                boundHostObjectMemberReference.CopyAttributes(this);
                return boundHostObjectMemberReference;
            }
            return this;
        }
    }
}
