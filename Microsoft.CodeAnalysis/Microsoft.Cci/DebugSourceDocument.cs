using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

#nullable enable

namespace Microsoft.Cci
{
    public sealed class DebugSourceDocument
    {
        public static readonly Guid CorSymLanguageTypeCSharp = new Guid("{3f5162f8-07c6-11d3-9053-00c04fa302a1}");

        internal static readonly Guid CorSymLanguageTypeBasic = new Guid("{3a12d0b8-c26c-11d0-b442-00a0244a1dd2}");

        private static readonly Guid s_corSymLanguageVendorMicrosoft = new Guid("{994b45c4-e6e9-11d2-903f-00c04fa302a1}");

        private static readonly Guid s_corSymDocumentTypeText = new Guid("{5a869d0b-6611-11d3-bd2a-0000f80849bd}");

        private readonly string _location;

        private readonly Guid _language;

        private readonly bool _isComputedChecksum;

        private readonly Task<DebugSourceInfo>? _sourceInfo;

        public Guid DocumentType => s_corSymDocumentTypeText;

        public Guid Language => _language;

        public Guid LanguageVendor => s_corSymLanguageVendorMicrosoft;

        public string Location => _location;

        public bool IsComputedChecksum => _isComputedChecksum;

        public DebugSourceDocument(string location, Guid language)
        {
            _location = location;
            _language = language;
        }

        public DebugSourceDocument(string location, Guid language, Func<DebugSourceInfo> sourceInfo)
            : this(location, language)
        {
            _sourceInfo = Task.Run(sourceInfo);
            _isComputedChecksum = true;
        }

        public DebugSourceDocument(string location, Guid language, ImmutableArray<byte> checksum, Guid algorithm)
            : this(location, language)
        {
            _sourceInfo = Task.FromResult(new DebugSourceInfo(checksum, algorithm));
        }

        public DebugSourceInfo GetSourceInfo()
        {
            return _sourceInfo?.Result ?? default(DebugSourceInfo);
        }
    }
}
