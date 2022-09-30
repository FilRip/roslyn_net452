using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class QueryClauseSyntax : CSharpSyntaxNode
    {
        public QueryClauseSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public QueryClauseSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected QueryClauseSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
