using System.Collections.Immutable;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public interface IDynamicMemberReferenceOperation : IOperation
    {
        IOperation? Instance { get; }

        string MemberName { get; }

        ImmutableArray<ITypeSymbol> TypeArguments { get; }

        ITypeSymbol? ContainingType { get; }
    }
}
