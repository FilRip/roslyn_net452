using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public abstract class BasePropertyDeclarationSyntax : MemberDeclarationSyntax
    {
        public abstract TypeSyntax Type { get; }

        public abstract ExplicitInterfaceSpecifierSyntax? ExplicitInterfaceSpecifier { get; }

        public abstract AccessorListSyntax? AccessorList { get; }

        public BasePropertyDeclarationSyntax(SyntaxKind kind, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
        }

        public BasePropertyDeclarationSyntax(SyntaxKind kind)
            : base(kind)
        {
        }

        protected BasePropertyDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
        }
    }
}
