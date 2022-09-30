using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class EnumConversions
    {
        internal static TypeKind ToTypeKind(this DeclarationKind kind)
        {
            switch (kind)
            {
                case DeclarationKind.Class:
                case DeclarationKind.Script:
                case DeclarationKind.ImplicitClass:
                case DeclarationKind.SimpleProgram:
                case DeclarationKind.Record:
                    return TypeKind.Class;
                case DeclarationKind.Submission:
                    return TypeKind.Submission;
                case DeclarationKind.Delegate:
                    return TypeKind.Delegate;
                case DeclarationKind.Enum:
                    return TypeKind.Enum;
                case DeclarationKind.Interface:
                    return TypeKind.Interface;
                case DeclarationKind.Struct:
                case DeclarationKind.RecordStruct:
                    return TypeKind.Struct;
                default:
                    throw ExceptionUtilities.UnexpectedValue(kind);
            }
        }
    }
}
