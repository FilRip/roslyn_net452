using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class TypeParameterConstraintSyntax : CSharpSyntaxNode
    {
        public TypeParameterConstraintSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public TypeParameterConstraintSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected TypeParameterConstraintSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
