using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class SynthesizedRecordConstructor : SourceConstructorSymbolBase
    {
        protected override bool AllowRefOrOut => false;

        internal override bool IsExpressionBodied => false;

        public SynthesizedRecordConstructor(SourceMemberContainerTypeSymbol containingType, TypeDeclarationSyntax syntax)
            : base(containingType, syntax.Identifier.GetLocation(), syntax, isIterator: false)
        {
            MakeFlags(MethodKind.Constructor, containingType.IsAbstract ? DeclarationModifiers.Protected : DeclarationModifiers.Public, returnsVoid: true, isExtensionMethod: false, isNullableAnalysisEnabled: false);
        }

        internal RecordDeclarationSyntax GetSyntax()
        {
            return (RecordDeclarationSyntax)syntaxReferenceOpt.GetSyntax();
        }

        protected override ParameterListSyntax GetParameterList()
        {
            return GetSyntax().ParameterList;
        }

        protected override CSharpSyntaxNode? GetInitializer()
        {
            return GetSyntax().PrimaryConstructorBaseTypeIfClass;
        }

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
            TypeDeclarationSyntax syntax = GetSyntax();
            InMethodBinder recordConstructorInMethodBinder = (binderFactoryOpt ?? DeclaringCompilation.GetBinderFactory(syntax.SyntaxTree))!.GetRecordConstructorInMethodBinder(this);
            return new ExecutableCodeBinder(SyntaxNode, this, recordConstructorInMethodBinder.WithAdditionalFlags(ignoreAccessibility ? BinderFlags.IgnoreAccessibility : BinderFlags.None));
        }
    }
}
