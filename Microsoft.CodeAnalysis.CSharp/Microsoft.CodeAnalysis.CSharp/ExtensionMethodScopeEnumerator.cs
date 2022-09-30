namespace Microsoft.CodeAnalysis.CSharp
{
    internal struct ExtensionMethodScopeEnumerator
    {
        private readonly Binder _binder;

        private ExtensionMethodScope _current;

        public ExtensionMethodScope Current => _current;

        public ExtensionMethodScopeEnumerator(Binder binder)
        {
            _binder = binder;
            _current = default(ExtensionMethodScope);
        }

        public bool MoveNext()
        {
            if (_current.Binder == null)
            {
                _current = GetNextScope(_binder);
            }
            else
            {
                Binder binder = _current.Binder;
                _current = GetNextScope(binder.Next);
            }
            return _current.Binder != null;
        }

        private static ExtensionMethodScope GetNextScope(Binder binder)
        {
            for (Binder binder2 = binder; binder2 != null; binder2 = binder2.Next)
            {
                if (binder2.SupportsExtensionMethods)
                {
                    return new ExtensionMethodScope(binder2);
                }
            }
            return default(ExtensionMethodScope);
        }
    }
}
