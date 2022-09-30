using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class CrefSyntax : CSharpSyntaxNode
    {
        public CrefSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public CrefSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected CrefSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
