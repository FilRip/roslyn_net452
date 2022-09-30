using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class DirectiveTriviaSyntax : StructuredTriviaSyntax
    {
        public abstract SyntaxToken HashToken { get; }

        public abstract SyntaxToken EndOfDirectiveToken { get; }

        public abstract bool IsActive { get; }

        internal override DirectiveStack ApplyDirectives(DirectiveStack stack)
        {
            return stack.Add(new Directive(this));
        }

        public DirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            flags |= NodeFlags.ContainsDirectives;
        }

        public DirectiveTriviaSyntax(SyntaxKind kind)
            : base(kind)
        {
            flags |= NodeFlags.ContainsDirectives;
        }

        protected DirectiveTriviaSyntax(ObjectReader reader)
            : base(reader)
        {
            flags |= NodeFlags.ContainsDirectives;
        }
    }
}
