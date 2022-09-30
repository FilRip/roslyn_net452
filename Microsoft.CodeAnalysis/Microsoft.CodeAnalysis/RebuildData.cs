using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace Microsoft.CodeAnalysis
{
    public sealed class RebuildData
    {
        internal ImmutableArray<string> NonSourceFileDocumentNames { get; }

        internal BlobReader OptionsBlobReader { get; }

        public RebuildData(BlobReader optionsBlobReader, ImmutableArray<string> nonSourceFileDocumentNames)
        {
            OptionsBlobReader = optionsBlobReader;
            NonSourceFileDocumentNames = nonSourceFileDocumentNames;
        }
    }
}
