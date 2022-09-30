using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class NameSyntax : TypeSyntax
    {
        public NameSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public NameSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected NameSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
