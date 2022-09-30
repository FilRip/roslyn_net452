using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class SelectOrGroupClauseSyntax : CSharpSyntaxNode
    {
        public SelectOrGroupClauseSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public SelectOrGroupClauseSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected SelectOrGroupClauseSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
