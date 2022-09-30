namespace Microsoft.CodeAnalysis
{
    public class CommonEventWellKnownAttributeData : WellKnownAttributeData, ISkipLocalsInitAttributeTarget
    {
        private bool _hasSpecialNameAttribute;

        private bool _hasExcludeFromCodeCoverageAttribute;

        private bool _hasSkipLocalsInitAttribute;

        public bool HasSpecialNameAttribute
        {
            get
            {
                return _hasSpecialNameAttribute;
            }
            set
            {
                _hasSpecialNameAttribute = value;
            }
        }

        public bool HasExcludeFromCodeCoverageAttribute
        {
            get
            {
                return _hasExcludeFromCodeCoverageAttribute;
            }
            set
            {
                _hasExcludeFromCodeCoverageAttribute = value;
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
