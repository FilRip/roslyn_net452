using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis
{
    public interface IMemberNotNullAttributeTarget
    {
        ImmutableArray<string> NotNullMembers { get; }

        ImmutableArray<string> NotNullWhenTrueMembers { get; }

        ImmutableArray<string> NotNullWhenFalseMembers { get; }

        void AddNotNullMember(string memberName);

        void AddNotNullMember(ArrayBuilder<string> memberNames);

        void AddNotNullWhenMember(bool sense, string memberName);

        void AddNotNullWhenMember(bool sense, ArrayBuilder<string> memberNames);
    }
}
