using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class InstanceExpressionSyntax : ExpressionSyntax
    {
        public InstanceExpressionSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public InstanceExpressionSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected InstanceExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
