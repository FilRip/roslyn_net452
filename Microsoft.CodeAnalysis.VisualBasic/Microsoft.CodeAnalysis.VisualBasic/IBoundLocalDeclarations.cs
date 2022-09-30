using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal interface IBoundLocalDeclarations
	{
		ImmutableArray<BoundLocalDeclarationBase> Declarations { get; }
	}
}
