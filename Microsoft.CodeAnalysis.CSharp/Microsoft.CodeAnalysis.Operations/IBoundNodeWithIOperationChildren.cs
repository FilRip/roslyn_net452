using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    internal interface IBoundNodeWithIOperationChildren
    {
        ImmutableArray<BoundNode?> Children { get; }
    }
}
