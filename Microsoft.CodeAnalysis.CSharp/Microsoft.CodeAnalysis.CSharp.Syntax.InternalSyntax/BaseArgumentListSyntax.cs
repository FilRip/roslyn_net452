using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class BaseArgumentListSyntax : CSharpSyntaxNode
    {
        public abstract Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> Arguments { get; }

        public BaseArgumentListSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public BaseArgumentListSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected BaseArgumentListSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
