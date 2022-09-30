using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class CommonForEachStatementSyntax : StatementSyntax
    {
        public abstract SyntaxToken? AwaitKeyword { get; }

        public abstract SyntaxToken ForEachKeyword { get; }

        public abstract SyntaxToken OpenParenToken { get; }

        public abstract SyntaxToken InKeyword { get; }

        public abstract ExpressionSyntax Expression { get; }

        public abstract SyntaxToken CloseParenToken { get; }

        public abstract StatementSyntax Statement { get; }

        public CommonForEachStatementSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public CommonForEachStatementSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected CommonForEachStatementSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
