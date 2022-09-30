using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic;

namespace Microsoft.CodeAnalysis.Operations
{
	internal interface IBoundNodeWithIOperationChildren
	{
		ImmutableArray<BoundNode> Children { get; }
	}
}
