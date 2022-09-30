using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class BaseFieldDeclarationSyntax : MemberDeclarationSyntax
    {
        public abstract VariableDeclarationSyntax Declaration { get; }

        public abstract SyntaxToken SemicolonToken { get; }

        public BaseFieldDeclarationSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public BaseFieldDeclarationSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected BaseFieldDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
