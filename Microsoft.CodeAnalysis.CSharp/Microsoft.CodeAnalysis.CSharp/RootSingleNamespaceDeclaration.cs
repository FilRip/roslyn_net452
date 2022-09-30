using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class RootSingleNamespaceDeclaration : SingleNamespaceDeclaration
    {
        private readonly ImmutableArray<ReferenceDirective> _referenceDirectives;

        private readonly bool _hasAssemblyAttributes;

        private readonly bool _hasGlobalUsings;

        private readonly bool _hasUsings;

        private readonly bool _hasExternAliases;

        public ImmutableArray<ReferenceDirective> ReferenceDirectives => _referenceDirectives;

        public bool HasAssemblyAttributes => _hasAssemblyAttributes;

        public override bool HasGlobalUsings => _hasGlobalUsings;

        public override bool HasUsings => _hasUsings;

        public override bool HasExternAliases => _hasExternAliases;

        public RootSingleNamespaceDeclaration(bool hasGlobalUsings, bool hasUsings, bool hasExternAliases, SyntaxReference treeNode, ImmutableArray<SingleNamespaceOrTypeDeclaration> children, ImmutableArray<ReferenceDirective> referenceDirectives, bool hasAssemblyAttributes, ImmutableArray<Diagnostic> diagnostics)
            : base(string.Empty, treeNode, new SourceLocation(treeNode), children, diagnostics)
        {
            _referenceDirectives = referenceDirectives;
            _hasAssemblyAttributes = hasAssemblyAttributes;
            _hasGlobalUsings = hasGlobalUsings;
            _hasUsings = hasUsings;
            _hasExternAliases = hasExternAliases;
        }
    }
}
