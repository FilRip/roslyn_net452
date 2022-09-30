using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class BaseParameterListSyntax : CSharpSyntaxNode
    {
        public abstract Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> Parameters { get; }

        public BaseParameterListSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public BaseParameterListSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected BaseParameterListSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
