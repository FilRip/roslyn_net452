using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class SimpleNameSyntax : NameSyntax
    {
        public abstract SyntaxToken Identifier { get; }

        public SimpleNameSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public SimpleNameSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        public SimpleNameSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
