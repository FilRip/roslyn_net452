using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class EnumConversions
    {
        internal static DeclarationKind ToDeclarationKind(this SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.ClassDeclaration => DeclarationKind.Class,
                SyntaxKind.InterfaceDeclaration => DeclarationKind.Interface,
                SyntaxKind.StructDeclaration => DeclarationKind.Struct,
                SyntaxKind.NamespaceDeclaration => DeclarationKind.Namespace,
                SyntaxKind.EnumDeclaration => DeclarationKind.Enum,
                SyntaxKind.DelegateDeclaration => DeclarationKind.Delegate,
                SyntaxKind.RecordDeclaration => DeclarationKind.Record,
                SyntaxKind.RecordStructDeclaration => DeclarationKind.RecordStruct,
                _ => throw ExceptionUtilities.UnexpectedValue(kind),
            };
        }
    }
}
