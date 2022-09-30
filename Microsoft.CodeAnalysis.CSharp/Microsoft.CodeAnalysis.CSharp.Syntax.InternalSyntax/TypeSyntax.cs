using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class TypeSyntax : ExpressionSyntax
    {
        public bool IsVar => IsIdentifierName("var");

        public bool IsUnmanaged => IsIdentifierName("unmanaged");

        public bool IsNotNull => IsIdentifierName("notnull");

        public bool IsNint => IsIdentifierName("nint");

        public bool IsNuint => IsIdentifierName("nuint");

        public bool IsRef => base.Kind == SyntaxKind.RefType;

        private bool IsIdentifierName(string id)
        {
            if (this is IdentifierNameSyntax identifierNameSyntax)
            {
                return identifierNameSyntax.Identifier.ToString() == id;
            }
            return false;
        }

        public TypeSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public TypeSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected TypeSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
