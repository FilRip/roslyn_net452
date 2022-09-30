using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class ExpressionOrPatternSyntax : CSharpSyntaxNode
    {
        public ExpressionOrPatternSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public ExpressionOrPatternSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected ExpressionOrPatternSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
