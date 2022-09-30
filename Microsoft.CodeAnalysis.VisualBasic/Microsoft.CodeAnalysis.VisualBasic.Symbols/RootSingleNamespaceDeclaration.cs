using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class RootSingleNamespaceDeclaration : GlobalNamespaceDeclaration
	{
		private ImmutableArray<ReferenceDirective> _referenceDirectives;

		private readonly bool _hasAssemblyAttributes;

		public ImmutableArray<ReferenceDirective> ReferenceDirectives => _referenceDirectives;

		public bool HasAssemblyAttributes => _hasAssemblyAttributes;

		public RootSingleNamespaceDeclaration(bool hasImports, SyntaxReference treeNode, ImmutableArray<SingleNamespaceOrTypeDeclaration> children, ImmutableArray<ReferenceDirective> referenceDirectives, bool hasAssemblyAttributes)
			: base(hasImports, treeNode, treeNode.GetLocation(), children)
		{
			_referenceDirectives = referenceDirectives;
			_hasAssemblyAttributes = hasAssemblyAttributes;
		}
	}
}
