using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal interface IBoundSwitchStatement
    {
        BoundNode Value { get; }

        ImmutableArray<BoundStatementList> Cases { get; }
    }
}
