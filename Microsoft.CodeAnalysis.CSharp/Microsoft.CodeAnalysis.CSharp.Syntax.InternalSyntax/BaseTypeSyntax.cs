using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class BaseTypeSyntax : CSharpSyntaxNode
    {
        public abstract TypeSyntax Type { get; }

        public BaseTypeSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public BaseTypeSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected BaseTypeSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
