using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class ExpressionSyntax : ExpressionOrPatternSyntax
    {
        public ExpressionSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public ExpressionSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected ExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
