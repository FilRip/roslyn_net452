using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class ConditionalDirectiveTriviaSyntax : BranchingDirectiveTriviaSyntax
    {
        public abstract ExpressionSyntax Condition { get; }

        public abstract bool ConditionValue { get; }

        public ConditionalDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public ConditionalDirectiveTriviaSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected ConditionalDirectiveTriviaSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
