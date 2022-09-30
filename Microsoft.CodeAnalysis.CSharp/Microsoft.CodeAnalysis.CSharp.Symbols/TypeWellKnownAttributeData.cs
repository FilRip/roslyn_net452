namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class TypeWellKnownAttributeData : CommonTypeWellKnownAttributeData, ISkipLocalsInitAttributeTarget
    {
        private NamedTypeSymbol _comImportCoClass;

        private bool _hasSkipLocalsInitAttribute;

        public NamedTypeSymbol ComImportCoClass
        {
            get
            {
                return _comImportCoClass;
            }
            set
            {
                _comImportCoClass = value;
            }
        }

        public bool HasSkipLocalsInitAttribute
        {
            get
            {
                return _hasSkipLocalsInitAttribute;
            }
            set
            {
                _hasSkipLocalsInitAttribute = value;
            }
        }
    }
}
