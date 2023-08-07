using System.Runtime.CompilerServices;
using System.Security;

namespace System.Runtime.Versioning
{
    public static class CompatibilitySwitch
    {
        [SecurityCritical()]
        public static bool IsEnabled(string compatibilitySwitchName)
        {
            try
            {
                return IsEnabledInternalCall(compatibilitySwitchName, true);
            }
            catch (Exception) { /* Nothing to do */ }
            return false;
        }

        [SecurityCritical()]
        public static string GetValue(string compatibilitySwitchName)
        {
            try
            {
                return GetValueInternalCall(compatibilitySwitchName, true);
            }
            catch (Exception) { /* Nothing to do */ }
            return null;
        }

        [SecurityCritical()]
        internal static bool IsEnabledInternal(string compatibilitySwitchName)
        {
            try
            {
                return IsEnabledInternalCall(compatibilitySwitchName, false);
            }
            catch (Exception) { /* Nothing to do */ }
            return false;
        }

        [SecurityCritical()]
        internal static string GetValueInternal(string compatibilitySwitchName)
        {
            try
            {
                return GetValueInternalCall(compatibilitySwitchName, false);
            }
            catch (Exception) { /* Nothing to do */ }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), SecurityCritical()]
        internal static extern string GetAppContextOverridesInternalCall();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool IsEnabledInternalCall(string compatibilitySwitchName, bool onlyDB);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string GetValueInternalCall(string compatibilitySwitchName, bool onlyDB);
    }
}
