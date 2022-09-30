using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis.Internal;

namespace Microsoft.CodeAnalysis.Collections.Internal
{
    internal static class SR
    {
        private static ResourceManager s_resourceManager;

        internal static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new ResourceManager(typeof(Strings)));

        internal static CultureInfo Culture { get; set; }

        internal static string Arg_ArrayPlusOffTooSmall => GetResourceString("Arg_ArrayPlusOffTooSmall");

        internal static string Arg_HTCapacityOverflow => GetResourceString("Arg_HTCapacityOverflow");

        internal static string Arg_KeyNotFoundWithKey => GetResourceString("Arg_KeyNotFoundWithKey");

        internal static string Arg_LongerThanDestArray => GetResourceString("Arg_LongerThanDestArray");

        internal static string Arg_LongerThanSrcArray => GetResourceString("Arg_LongerThanSrcArray");

        internal static string Arg_NonZeroLowerBound => GetResourceString("Arg_NonZeroLowerBound");

        internal static string Arg_RankMultiDimNotSupported => GetResourceString("Arg_RankMultiDimNotSupported");

        internal static string Arg_WrongType => GetResourceString("Arg_WrongType");

        internal static string Argument_AddingDuplicateWithKey => GetResourceString("Argument_AddingDuplicateWithKey");

        internal static string Argument_InvalidArrayType => GetResourceString("Argument_InvalidArrayType");

        internal static string Argument_InvalidOffLen => GetResourceString("Argument_InvalidOffLen");

        internal static string ArgumentOutOfRange_ArrayLB => GetResourceString("ArgumentOutOfRange_ArrayLB");

        internal static string ArgumentOutOfRange_BiggerThanCollection => GetResourceString("ArgumentOutOfRange_BiggerThanCollection");

        internal static string ArgumentOutOfRange_Count => GetResourceString("ArgumentOutOfRange_Count");

        internal static string ArgumentOutOfRange_Index => GetResourceString("ArgumentOutOfRange_Index");

        internal static string ArgumentOutOfRange_ListInsert => GetResourceString("ArgumentOutOfRange_ListInsert");

        internal static string ArgumentOutOfRange_NeedNonNegNum => GetResourceString("ArgumentOutOfRange_NeedNonNegNum");

        internal static string ArgumentOutOfRange_SmallCapacity => GetResourceString("ArgumentOutOfRange_SmallCapacity");

        internal static string InvalidOperation_ConcurrentOperationsNotSupported => GetResourceString("InvalidOperation_ConcurrentOperationsNotSupported");

        internal static string InvalidOperation_EnumFailedVersion => GetResourceString("InvalidOperation_EnumFailedVersion");

        internal static string InvalidOperation_EnumOpCantHappen => GetResourceString("InvalidOperation_EnumOpCantHappen");

        internal static string InvalidOperation_IComparerFailed => GetResourceString("InvalidOperation_IComparerFailed");

        internal static string NotSupported_KeyCollectionSet => GetResourceString("NotSupported_KeyCollectionSet");

        internal static string NotSupported_ValueCollectionSet => GetResourceString("NotSupported_ValueCollectionSet");

        internal static string Rank_MustMatch => GetResourceString("Rank_MustMatch");

        internal static string NotSupported_FixedSizeCollection => GetResourceString("NotSupported_FixedSizeCollection");

        internal static string ArgumentException_OtherNotArrayOfCorrectLength => GetResourceString("ArgumentException_OtherNotArrayOfCorrectLength");

        internal static string Arg_BogusIComparer => GetResourceString("Arg_BogusIComparer");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetResourceString(string resourceKey, string defaultValue = null)
        {
            return ResourceManager.GetString(resourceKey, Culture);
        }
    }
}
