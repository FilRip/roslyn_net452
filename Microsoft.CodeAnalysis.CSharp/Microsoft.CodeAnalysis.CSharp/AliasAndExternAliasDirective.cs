using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public struct AliasAndExternAliasDirective
    {
        public readonly AliasSymbol Alias;

        public readonly SyntaxReference? ExternAliasDirectiveReference;

        public readonly bool SkipInLookup;

        public ExternAliasDirectiveSyntax? ExternAliasDirective => (ExternAliasDirectiveSyntax)(ExternAliasDirectiveReference?.GetSyntax());

        public AliasAndExternAliasDirective(AliasSymbol alias, ExternAliasDirectiveSyntax? externAliasDirective, bool skipInLookup)
        {
            Alias = alias;
            ExternAliasDirectiveReference = externAliasDirective?.GetReference();
            SkipInLookup = skipInLookup;
        }
    }
}
