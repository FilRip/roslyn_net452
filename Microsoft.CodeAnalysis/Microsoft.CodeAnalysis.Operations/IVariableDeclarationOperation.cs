using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IVariableDeclarationOperation : IOperation
    {
        ImmutableArray<IVariableDeclaratorOperation> Declarators { get; }

        IVariableInitializerOperation? Initializer { get; }

        ImmutableArray<IOperation> IgnoredDimensions { get; }
    }
}
