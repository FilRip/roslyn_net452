namespace Microsoft.CodeAnalysis.CSharp
{
    internal struct ExtensionMethodScopes
    {
        private readonly Binder _binder;

        public ExtensionMethodScopes(Binder binder)
        {
            _binder = binder;
        }

        public ExtensionMethodScopeEnumerator GetEnumerator()
        {
            return new ExtensionMethodScopeEnumerator(_binder);
        }
    }
}
