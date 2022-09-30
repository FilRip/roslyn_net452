using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct AliasAndImportsClausePosition
	{
		public readonly AliasSymbol Alias;

		public readonly int ImportsClausePosition;

		public readonly ImmutableArray<AssemblySymbol> Dependencies;

		public AliasAndImportsClausePosition(AliasSymbol alias, int importsClausePosition, ImmutableArray<AssemblySymbol> dependencies)
		{
			this = default(AliasAndImportsClausePosition);
			Alias = alias;
			ImportsClausePosition = importsClausePosition;
			Dependencies = dependencies;
		}
	}
}
