using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class SwitchLabelSyntax : CSharpSyntaxNode
    {
        public abstract SyntaxToken Keyword { get; }

        public abstract SyntaxToken ColonToken { get; }

        public SwitchLabelSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public SwitchLabelSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected SwitchLabelSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
