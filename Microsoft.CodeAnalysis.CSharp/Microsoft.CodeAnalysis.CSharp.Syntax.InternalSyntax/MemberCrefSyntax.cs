using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class MemberCrefSyntax : CrefSyntax
    {
        public MemberCrefSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public MemberCrefSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected MemberCrefSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
