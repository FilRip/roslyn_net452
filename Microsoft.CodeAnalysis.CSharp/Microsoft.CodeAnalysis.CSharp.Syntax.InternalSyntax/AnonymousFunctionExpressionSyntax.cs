using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class AnonymousFunctionExpressionSyntax : ExpressionSyntax
    {
        public abstract Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers { get; }

        public abstract BlockSyntax? Block { get; }

        public abstract ExpressionSyntax? ExpressionBody { get; }

        public AnonymousFunctionExpressionSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public AnonymousFunctionExpressionSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected AnonymousFunctionExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
