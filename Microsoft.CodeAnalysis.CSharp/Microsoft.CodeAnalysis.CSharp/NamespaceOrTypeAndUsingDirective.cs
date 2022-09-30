using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal struct NamespaceOrTypeAndUsingDirective
    {
        public readonly NamespaceOrTypeSymbol NamespaceOrType;

        public readonly SyntaxReference? UsingDirectiveReference;

        public readonly ImmutableArray<AssemblySymbol> Dependencies;

        public UsingDirectiveSyntax? UsingDirective => (UsingDirectiveSyntax)(UsingDirectiveReference?.GetSyntax());

        public NamespaceOrTypeAndUsingDirective(NamespaceOrTypeSymbol namespaceOrType, UsingDirectiveSyntax? usingDirective, ImmutableArray<AssemblySymbol> dependencies)
        {
            NamespaceOrType = namespaceOrType;
            UsingDirectiveReference = usingDirective?.GetReference();
            Dependencies = dependencies.NullToEmpty();
        }
    }
}
