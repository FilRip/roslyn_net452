using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class StructuredTriviaSyntax : CSharpSyntaxNode
    {
        public StructuredTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] diagnostics = null, SyntaxAnnotation[] annotations = null)
            : base(kind, diagnostics, annotations)
        {
            Initialize();
        }

        public StructuredTriviaSyntax(ObjectReader reader)
            : base(reader)
        {
            Initialize();
        }

        private void Initialize()
        {
            flags |= NodeFlags.ContainsStructuredTrivia;
            if (base.Kind == SyntaxKind.SkippedTokensTrivia)
            {
                flags |= NodeFlags.ContainsSkippedText;
            }
        }
    }
}
