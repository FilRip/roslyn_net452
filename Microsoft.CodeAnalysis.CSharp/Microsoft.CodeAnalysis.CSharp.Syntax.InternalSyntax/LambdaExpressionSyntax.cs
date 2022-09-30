using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class LambdaExpressionSyntax : AnonymousFunctionExpressionSyntax
    {
        public abstract Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists { get; }

        public abstract SyntaxToken ArrowToken { get; }

        public LambdaExpressionSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public LambdaExpressionSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected LambdaExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
