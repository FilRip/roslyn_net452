using System.Runtime.InteropServices;

namespace Microsoft.CodeAnalysis
{
    public class CommonModuleWellKnownAttributeData : WellKnownAttributeData
    {
        private bool _hasDebuggableAttribute;

        private byte _defaultCharacterSet;

        public bool HasDebuggableAttribute
        {
            get
            {
                return _hasDebuggableAttribute;
            }
            set
            {
                _hasDebuggableAttribute = value;
            }
        }

        public CharSet DefaultCharacterSet
        {
            get
            {
                return (CharSet)_defaultCharacterSet;
            }
            set
            {
                _defaultCharacterSet = (byte)value;
            }
        }

        public bool HasDefaultCharSetAttribute => _defaultCharacterSet != 0;

        public static bool IsValidCharSet(CharSet value)
        {
            if (value >= CharSet.None)
            {
                return value <= CharSet.Auto;
            }
            return false;
        }
    }
}
