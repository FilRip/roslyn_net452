// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedRecordConstructor : SourceConstructorSymbolBase
    {
        public SynthesizedRecordConstructor(
             SourceMemberContainerTypeSymbol containingType,
             TypeDeclarationSyntax syntax) :
             base(containingType, syntax.Identifier.GetLocation(), syntax, isIterator: false)
        {

            this.MakeFlags(
                MethodKind.Constructor,
                containingType.IsAbstract ? DeclarationModifiers.Protected : DeclarationModifiers.Public,
                returnsVoid: true,
                isExtensionMethod: false,
                isNullableAnalysisEnabled: false); // IsNullableAnalysisEnabled uses containing type instead.
        }

        internal RecordDeclarationSyntax GetSyntax()
        {
            return (RecordDeclarationSyntax)syntaxReferenceOpt.GetSyntax();
        }

        protected override ParameterListSyntax GetParameterList()
        {
            var record = GetSyntax();
            return record.ParameterList!;
        }

        protected override CSharpSyntaxNode? GetInitializer()
        {
            var record = GetSyntax();
            return record.PrimaryConstructorBaseTypeIfClass;
        }

        protected override bool AllowRefOrOut => false;

        internal override bool IsExpressionBodied => false;

        internal override bool IsNullableAnalysisEnabled()
        {
            return ((SourceMemberContainerTypeSymbol)ContainingType).IsNullableEnabledForConstructorsAndInitializers(IsStatic);
        }

        protected override bool IsWithinExpressionOrBlockBody(int position, out int offset)
        {
            offset = -1;
            return false;
        }

        internal override ExecutableCodeBinder TryGetBodyBinder(BinderFactory? binderFactoryOpt = null, bool ignoreAccessibility = false)
        {
            TypeDeclarationSyntax typeDecl = GetSyntax();
            InMethodBinder result = (binderFactoryOpt ?? this.DeclaringCompilation.GetBinderFactory(typeDecl.SyntaxTree)).GetRecordConstructorInMethodBinder(this);
            return new ExecutableCodeBinder(SyntaxNode, this, result.WithAdditionalFlags(ignoreAccessibility ? EBinder.IgnoreAccessibility : EBinder.None));
        }
    }
}
