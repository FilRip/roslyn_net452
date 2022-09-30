namespace Microsoft.CodeAnalysis.CSharp
{
    internal struct ExtensionMethodScope
    {
        public readonly Binder Binder;

        public ExtensionMethodScope(Binder binder)
        {
            Binder = binder;
        }
    }
}
