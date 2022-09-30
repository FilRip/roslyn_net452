namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class ModuleWellKnownAttributeData : CommonModuleWellKnownAttributeData, ISkipLocalsInitAttributeTarget
    {
        private bool _hasSkipLocalsInitAttribute;

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
