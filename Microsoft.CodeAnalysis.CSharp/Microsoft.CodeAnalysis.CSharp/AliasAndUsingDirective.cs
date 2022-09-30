using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public struct AliasAndUsingDirective
    {
        public readonly AliasSymbol Alias;

        public readonly SyntaxReference? UsingDirectiveReference;

        public UsingDirectiveSyntax? UsingDirective => (UsingDirectiveSyntax)(UsingDirectiveReference?.GetSyntax());

        public AliasAndUsingDirective(AliasSymbol alias, UsingDirectiveSyntax? usingDirective)
        {
            Alias = alias;
            UsingDirectiveReference = usingDirective?.GetReference();
        }
    }
}
