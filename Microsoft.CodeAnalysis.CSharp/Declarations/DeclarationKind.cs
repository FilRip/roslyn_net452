// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal enum DeclarationKind : byte
    {
        Namespace,
        Class,
        Interface,
        Struct,
        Enum,
        Delegate,
        Script,
        Submission,
        ImplicitClass,
        SimpleProgram,
        Record,
        RecordStruct
    }

    internal static partial class EnumConversions
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
