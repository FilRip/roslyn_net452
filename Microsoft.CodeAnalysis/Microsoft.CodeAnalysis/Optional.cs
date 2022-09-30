namespace Microsoft.CodeAnalysis
{
    public readonly struct Optional<T>
    {
        private readonly bool _hasValue;

        private readonly T _value;

        public bool HasValue => _hasValue;

        public T Value => _value;

        public Optional(T value)
        {
            _hasValue = true;
            _value = value;
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public override string ToString()
        {
            if (!_hasValue)
            {
                return "unspecified";
            }
            T value = _value;
            return ((value != null) ? value.ToString() : null) ?? "null";
        }
    }
}
