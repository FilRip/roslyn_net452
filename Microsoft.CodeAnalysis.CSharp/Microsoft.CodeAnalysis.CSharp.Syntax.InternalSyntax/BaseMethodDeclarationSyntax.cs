using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class BaseMethodDeclarationSyntax : MemberDeclarationSyntax
    {
        public abstract ParameterListSyntax ParameterList { get; }

        public abstract BlockSyntax? Body { get; }

        public abstract ArrowExpressionClauseSyntax? ExpressionBody { get; }

        public abstract SyntaxToken? SemicolonToken { get; }

        public BaseMethodDeclarationSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public BaseMethodDeclarationSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected BaseMethodDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
