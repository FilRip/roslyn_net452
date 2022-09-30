using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class TypeDeclarationSyntax : BaseTypeDeclarationSyntax
    {
        public abstract SyntaxToken Keyword { get; }

        public abstract TypeParameterListSyntax? TypeParameterList { get; }

        public abstract Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses { get; }

        public abstract Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax> Members { get; }

        public TypeDeclarationSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public TypeDeclarationSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected TypeDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
