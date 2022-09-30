using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class PatternSyntax : ExpressionOrPatternSyntax
    {
        public PatternSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public PatternSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected PatternSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
