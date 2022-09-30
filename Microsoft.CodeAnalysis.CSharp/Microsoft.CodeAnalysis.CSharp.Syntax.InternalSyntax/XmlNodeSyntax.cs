using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class XmlNodeSyntax : CSharpSyntaxNode
    {
        public XmlNodeSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public XmlNodeSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected XmlNodeSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
