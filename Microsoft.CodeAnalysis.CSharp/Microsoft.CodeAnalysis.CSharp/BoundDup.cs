using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDup : BoundExpression
    {
        public RefKind RefKind { get; }

        public BoundDup(SyntaxNode syntax, RefKind refKind, TypeSymbol? type, bool hasErrors)
            : base(BoundKind.Dup, syntax, type, hasErrors)
        {
            RefKind = refKind;
        }

        public BoundDup(SyntaxNode syntax, RefKind refKind, TypeSymbol? type)
            : base(BoundKind.Dup, syntax, type)
        {
            RefKind = refKind;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDup(this);
        }

        public BoundDup Update(RefKind refKind, TypeSymbol? type)
        {
            if (refKind != RefKind || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundDup boundDup = new BoundDup(Syntax, refKind, type, base.HasErrors);
                boundDup.CopyAttributes(this);
                return boundDup;
            }
            return this;
        }
    }
}
