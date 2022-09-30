using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class BaseCrefParameterListSyntax : CSharpSyntaxNode
    {
        public abstract Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CrefParameterSyntax> Parameters { get; }

        public BaseCrefParameterListSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public BaseCrefParameterListSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected BaseCrefParameterListSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
