using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class InterpolatedStringContentSyntax : CSharpSyntaxNode
    {
        public InterpolatedStringContentSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public InterpolatedStringContentSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected InterpolatedStringContentSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
