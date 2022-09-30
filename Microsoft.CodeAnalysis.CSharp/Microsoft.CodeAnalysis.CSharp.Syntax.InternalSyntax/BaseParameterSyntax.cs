using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class BaseParameterSyntax : CSharpSyntaxNode
    {
        public abstract Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists { get; }

        public abstract Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers { get; }

        public abstract TypeSyntax? Type { get; }

        public BaseParameterSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public BaseParameterSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected BaseParameterSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
