using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class BaseObjectCreationExpressionSyntax : ExpressionSyntax
    {
        public abstract SyntaxToken NewKeyword { get; }

        public abstract ArgumentListSyntax? ArgumentList { get; }

        public abstract InitializerExpressionSyntax? Initializer { get; }

        public BaseObjectCreationExpressionSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public BaseObjectCreationExpressionSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected BaseObjectCreationExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
