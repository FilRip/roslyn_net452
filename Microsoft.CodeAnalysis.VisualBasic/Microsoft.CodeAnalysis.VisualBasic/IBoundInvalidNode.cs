using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal interface IBoundInvalidNode
	{
		ImmutableArray<BoundNode> InvalidNodeChildren { get; }
	}
}
