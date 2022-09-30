using System.Diagnostics;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSourceDocumentIndex : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public DebugSourceDocument Document { get; }

        public BoundSourceDocumentIndex(SyntaxNode syntax, DebugSourceDocument document, TypeSymbol type, bool hasErrors)
            : base(BoundKind.SourceDocumentIndex, syntax, type, hasErrors)
        {
            Document = document;
        }

        public BoundSourceDocumentIndex(SyntaxNode syntax, DebugSourceDocument document, TypeSymbol type)
            : base(BoundKind.SourceDocumentIndex, syntax, type)
        {
            Document = document;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSourceDocumentIndex(this);
        }

        public BoundSourceDocumentIndex Update(DebugSourceDocument document, TypeSymbol type)
        {
            if (document != Document || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundSourceDocumentIndex boundSourceDocumentIndex = new BoundSourceDocumentIndex(Syntax, document, type, base.HasErrors);
                boundSourceDocumentIndex.CopyAttributes(this);
                return boundSourceDocumentIndex;
            }
            return this;
        }
    }
}
