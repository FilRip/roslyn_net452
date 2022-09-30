using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class ReturnTypeWellKnownAttributeData : CommonReturnTypeWellKnownAttributeData
    {
        private bool _hasMaybeNullAttribute;

        private bool _hasNotNullAttribute;

        private ImmutableHashSet<string> _notNullIfParameterNotNull = ImmutableHashSet<string>.Empty;

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

        public ImmutableHashSet<string> NotNullIfParameterNotNull => _notNullIfParameterNotNull;

        public void AddNotNullIfParameterNotNull(string parameterName)
        {
            _notNullIfParameterNotNull = _notNullIfParameterNotNull.Add(parameterName);
        }
    }
}
