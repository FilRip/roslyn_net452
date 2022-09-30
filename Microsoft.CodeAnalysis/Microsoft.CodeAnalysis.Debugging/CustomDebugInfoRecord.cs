using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Debugging
{
    internal struct CustomDebugInfoRecord
    {
        public readonly CustomDebugInfoKind Kind;

        public readonly byte Version;

        public readonly ImmutableArray<byte> Data;

        public CustomDebugInfoRecord(CustomDebugInfoKind kind, byte version, ImmutableArray<byte> data)
        {
            Kind = kind;
            Version = version;
            Data = data;
        }
    }
}
