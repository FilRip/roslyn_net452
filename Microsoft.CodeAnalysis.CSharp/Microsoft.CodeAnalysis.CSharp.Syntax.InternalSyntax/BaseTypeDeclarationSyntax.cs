using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class BaseTypeDeclarationSyntax : MemberDeclarationSyntax
    {
        public abstract SyntaxToken Identifier { get; }

        public abstract BaseListSyntax? BaseList { get; }

        public abstract SyntaxToken? OpenBraceToken { get; }

        public abstract SyntaxToken? CloseBraceToken { get; }

        public abstract SyntaxToken? SemicolonToken { get; }

        public BaseTypeDeclarationSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public BaseTypeDeclarationSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected BaseTypeDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
