using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp
{
    public interface IBoundInvalidNode
    {
        ImmutableArray<BoundNode> InvalidNodeChildren { get; }
    }
}
