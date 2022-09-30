using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class StatementSyntax : CSharpSyntaxNode
    {
        public abstract Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists { get; }

        public StatementSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public StatementSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected StatementSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
