using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class XmlAttributeSyntax : CSharpSyntaxNode
    {
        public abstract XmlNameSyntax Name { get; }

        public abstract SyntaxToken EqualsToken { get; }

        public abstract SyntaxToken StartQuoteToken { get; }

        public abstract SyntaxToken EndQuoteToken { get; }

        public XmlAttributeSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public XmlAttributeSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected XmlAttributeSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
