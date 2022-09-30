using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class BranchingDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        public abstract bool BranchTaken { get; }

        public BranchingDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public BranchingDirectiveTriviaSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected BranchingDirectiveTriviaSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
