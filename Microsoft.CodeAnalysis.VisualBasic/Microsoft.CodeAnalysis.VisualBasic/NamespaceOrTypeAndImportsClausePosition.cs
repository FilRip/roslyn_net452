using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct NamespaceOrTypeAndImportsClausePosition
	{
		public readonly NamespaceOrTypeSymbol NamespaceOrType;

		public readonly int ImportsClausePosition;

		public readonly ImmutableArray<AssemblySymbol> Dependencies;

		public NamespaceOrTypeAndImportsClausePosition(NamespaceOrTypeSymbol namespaceOrType, int importsClausePosition, ImmutableArray<AssemblySymbol> dependencies)
		{
			this = default(NamespaceOrTypeAndImportsClausePosition);
			NamespaceOrType = namespaceOrType;
			ImportsClausePosition = importsClausePosition;
			Dependencies = dependencies;
		}
	}
}
