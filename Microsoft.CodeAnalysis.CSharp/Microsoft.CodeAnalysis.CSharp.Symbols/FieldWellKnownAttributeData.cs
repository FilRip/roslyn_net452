namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class FieldWellKnownAttributeData : CommonFieldWellKnownAttributeData
    {
        private bool _hasAllowNullAttribute;

        private bool _hasDisallowNullAttribute;

        private bool _hasMaybeNullAttribute;

        private bool? _maybeNullWhenAttribute;

        private bool _hasNotNullAttribute;

        public bool HasAllowNullAttribute
        {
            get
            {
                return _hasAllowNullAttribute;
            }
            set
            {
                _hasAllowNullAttribute = value;
            }
        }

        public bool HasDisallowNullAttribute
        {
            get
            {
                return _hasDisallowNullAttribute;
            }
            set
            {
                _hasDisallowNullAttribute = value;
            }
        }

        public bool HasMaybeNullAttribute
        {
            get
            {
                return _hasMaybeNullAttribute;
            }
            set
            {
                _hasMaybeNullAttribute = value;
            }
        }

        public bool? MaybeNullWhenAttribute
        {
            get
            {
                return _maybeNullWhenAttribute;
            }
            set
            {
                _maybeNullWhenAttribute = value;
            }
        }

        public bool HasNotNullAttribute
        {
            get
            {
                return _hasNotNullAttribute;
            }
            set
            {
                _hasNotNullAttribute = value;
            }
        }
    }
}
