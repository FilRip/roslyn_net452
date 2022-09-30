using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class ParameterWellKnownAttributeData : CommonParameterWellKnownAttributeData
    {
        private bool _hasAllowNullAttribute;

        private bool _hasDisallowNullAttribute;

        private bool _hasMaybeNullAttribute;

        private bool? _maybeNullWhenAttribute;

        private bool _hasNotNullAttribute;

        private bool? _notNullWhenAttribute;

        private bool? _doesNotReturnIfAttribute;

        private bool _hasEnumeratorCancellationAttribute;

        private ImmutableHashSet<string> _notNullIfParameterNotNull = ImmutableHashSet<string>.Empty;

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

        public bool? NotNullWhenAttribute
        {
            get
            {
                return _notNullWhenAttribute;
            }
            set
            {
                _notNullWhenAttribute = value;
            }
        }

        public bool? DoesNotReturnIfAttribute
        {
            get
            {
                return _doesNotReturnIfAttribute;
            }
            set
            {
                _doesNotReturnIfAttribute = value;
            }
        }

        public bool HasEnumeratorCancellationAttribute
        {
            get
            {
                return _hasEnumeratorCancellationAttribute;
            }
            set
            {
                _hasEnumeratorCancellationAttribute = value;
            }
        }

        public ImmutableHashSet<string> NotNullIfParameterNotNull => _notNullIfParameterNotNull;

        public void AddNotNullIfParameterNotNull(string parameterName)
        {
            _notNullIfParameterNotNull = _notNullIfParameterNotNull.Add(parameterName);
        }
    }
}
