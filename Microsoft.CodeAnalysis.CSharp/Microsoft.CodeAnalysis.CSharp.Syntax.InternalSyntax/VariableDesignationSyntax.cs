using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class VariableDesignationSyntax : CSharpSyntaxNode
    {
        public VariableDesignationSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public VariableDesignationSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected VariableDesignationSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
