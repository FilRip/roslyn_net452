using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.Debugging;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.Cci
{
    public struct DebugSourceInfo
    {
        public readonly Guid ChecksumAlgorithmId;

        public readonly ImmutableArray<byte> Checksum;

        public readonly ImmutableArray<byte> EmbeddedTextBlob;

        public DebugSourceInfo(ImmutableArray<byte> checksum, SourceHashAlgorithm checksumAlgorithm, ImmutableArray<byte> embeddedTextBlob = default)
            : this(checksum, SourceHashAlgorithms.GetAlgorithmGuid(checksumAlgorithm), embeddedTextBlob)
        {
        }

        public DebugSourceInfo(ImmutableArray<byte> checksum, Guid checksumAlgorithmId, ImmutableArray<byte> embeddedTextBlob = default)
        {
            ChecksumAlgorithmId = checksumAlgorithmId;
            Checksum = checksum;
            EmbeddedTextBlob = embeddedTextBlob;
        }
    }
}
