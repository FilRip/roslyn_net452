#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public struct DecodeWellKnownAttributeArguments<TAttributeSyntax, TAttributeData, TAttributeLocation> where TAttributeSyntax : SyntaxNode where TAttributeData : AttributeData
    {
        private WellKnownAttributeData? _lazyDecodeData;

        public readonly bool HasDecodedData
        {
            get
            {
                if (_lazyDecodeData != null)
                {
                    return true;
                }
                return false;
            }
        }

        public readonly WellKnownAttributeData DecodedData => _lazyDecodeData;

        public TAttributeSyntax? AttributeSyntaxOpt { get; set; }

        public TAttributeData Attribute { get; set; }

        public int Index { get; set; }

        public int AttributesCount { get; set; }

        public BindingDiagnosticBag Diagnostics { get; set; }

        public TAttributeLocation SymbolPart { get; set; }

        public T GetOrCreateData<T>() where T : WellKnownAttributeData, new()
        {
            if (_lazyDecodeData == null)
            {
                _lazyDecodeData = new T();
            }
            return (T)_lazyDecodeData;
        }
    }
}
