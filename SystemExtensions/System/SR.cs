using System.Resources;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class SR
	{
		private static ResourceManager s_resourceManager;

		private static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new ResourceManager(ResourceType));

		internal static Type ResourceType { get; } = typeof(SystemExtensions.Properties.Resources);


		internal static string ArgumentNull_Array => GetResourceString("ArgumentNull_Array", null);

		internal static string ArgumentOutOfRange_NeedNonNegNum => GetResourceString("ArgumentOutOfRange_NeedNonNegNum", null);

		internal static string ArgumentOutOfRange_IndexCount => GetResourceString("ArgumentOutOfRange_IndexCount", null);

		internal static string ArgumentOutOfRange_IndexCountBuffer => GetResourceString("ArgumentOutOfRange_IndexCountBuffer", null);

		internal static string NotSupported_NoCodepageData => GetResourceString("NotSupported_NoCodepageData", null);

		internal static string Argument_EncodingConversionOverflowBytes => GetResourceString("Argument_EncodingConversionOverflowBytes", null);

		internal static string Argument_InvalidCharSequenceNoIndex => GetResourceString("Argument_InvalidCharSequenceNoIndex", null);

		internal static string ArgumentOutOfRange_GetByteCountOverflow => GetResourceString("ArgumentOutOfRange_GetByteCountOverflow", null);

		internal static string Argument_EncodingConversionOverflowChars => GetResourceString("Argument_EncodingConversionOverflowChars", null);

		internal static string ArgumentOutOfRange_GetCharCountOverflow => GetResourceString("ArgumentOutOfRange_GetCharCountOverflow", null);

		internal static string Argument_EncoderFallbackNotEmpty => GetResourceString("Argument_EncoderFallbackNotEmpty", null);

		internal static string Argument_RecursiveFallback => GetResourceString("Argument_RecursiveFallback", null);

		internal static string Argument_RecursiveFallbackBytes => GetResourceString("Argument_RecursiveFallbackBytes", null);

		internal static string ArgumentOutOfRange_Range => GetResourceString("ArgumentOutOfRange_Range", null);

		internal static string Argument_CodepageNotSupported => GetResourceString("Argument_CodepageNotSupported", null);

		internal static string ArgumentOutOfRange_Index => GetResourceString("ArgumentOutOfRange_Index", null);

		internal static string MissingEncodingNameResource => GetResourceString("MissingEncodingNameResource", null);

		internal static string Globalization_cp_37 => GetResourceString("Globalization_cp_37", null);

		internal static string Globalization_cp_437 => GetResourceString("Globalization_cp_437", null);

		internal static string Globalization_cp_500 => GetResourceString("Globalization_cp_500", null);

		internal static string Globalization_cp_708 => GetResourceString("Globalization_cp_708", null);

		internal static string Globalization_cp_720 => GetResourceString("Globalization_cp_720", null);

		internal static string Globalization_cp_737 => GetResourceString("Globalization_cp_737", null);

		internal static string Globalization_cp_775 => GetResourceString("Globalization_cp_775", null);

		internal static string Globalization_cp_850 => GetResourceString("Globalization_cp_850", null);

		internal static string Globalization_cp_852 => GetResourceString("Globalization_cp_852", null);

		internal static string Globalization_cp_855 => GetResourceString("Globalization_cp_855", null);

		internal static string Globalization_cp_857 => GetResourceString("Globalization_cp_857", null);

		internal static string Globalization_cp_858 => GetResourceString("Globalization_cp_858", null);

		internal static string Globalization_cp_860 => GetResourceString("Globalization_cp_860", null);

		internal static string Globalization_cp_861 => GetResourceString("Globalization_cp_861", null);

		internal static string Globalization_cp_862 => GetResourceString("Globalization_cp_862", null);

		internal static string Globalization_cp_863 => GetResourceString("Globalization_cp_863", null);

		internal static string Globalization_cp_864 => GetResourceString("Globalization_cp_864", null);

		internal static string Globalization_cp_865 => GetResourceString("Globalization_cp_865", null);

		internal static string Globalization_cp_866 => GetResourceString("Globalization_cp_866", null);

		internal static string Globalization_cp_869 => GetResourceString("Globalization_cp_869", null);

		internal static string Globalization_cp_870 => GetResourceString("Globalization_cp_870", null);

		internal static string Globalization_cp_874 => GetResourceString("Globalization_cp_874", null);

		internal static string Globalization_cp_875 => GetResourceString("Globalization_cp_875", null);

		internal static string Globalization_cp_932 => GetResourceString("Globalization_cp_932", null);

		internal static string Globalization_cp_936 => GetResourceString("Globalization_cp_936", null);

		internal static string Globalization_cp_949 => GetResourceString("Globalization_cp_949", null);

		internal static string Globalization_cp_950 => GetResourceString("Globalization_cp_950", null);

		internal static string Globalization_cp_1026 => GetResourceString("Globalization_cp_1026", null);

		internal static string Globalization_cp_1047 => GetResourceString("Globalization_cp_1047", null);

		internal static string Globalization_cp_1140 => GetResourceString("Globalization_cp_1140", null);

		internal static string Globalization_cp_1141 => GetResourceString("Globalization_cp_1141", null);

		internal static string Globalization_cp_1142 => GetResourceString("Globalization_cp_1142", null);

		internal static string Globalization_cp_1143 => GetResourceString("Globalization_cp_1143", null);

		internal static string Globalization_cp_1144 => GetResourceString("Globalization_cp_1144", null);

		internal static string Globalization_cp_1145 => GetResourceString("Globalization_cp_1145", null);

		internal static string Globalization_cp_1146 => GetResourceString("Globalization_cp_1146", null);

		internal static string Globalization_cp_1147 => GetResourceString("Globalization_cp_1147", null);

		internal static string Globalization_cp_1148 => GetResourceString("Globalization_cp_1148", null);

		internal static string Globalization_cp_1149 => GetResourceString("Globalization_cp_1149", null);

		internal static string Globalization_cp_1250 => GetResourceString("Globalization_cp_1250", null);

		internal static string Globalization_cp_1251 => GetResourceString("Globalization_cp_1251", null);

		internal static string Globalization_cp_1252 => GetResourceString("Globalization_cp_1252", null);

		internal static string Globalization_cp_1253 => GetResourceString("Globalization_cp_1253", null);

		internal static string Globalization_cp_1254 => GetResourceString("Globalization_cp_1254", null);

		internal static string Globalization_cp_1255 => GetResourceString("Globalization_cp_1255", null);

		internal static string Globalization_cp_1256 => GetResourceString("Globalization_cp_1256", null);

		internal static string Globalization_cp_1257 => GetResourceString("Globalization_cp_1257", null);

		internal static string Globalization_cp_1258 => GetResourceString("Globalization_cp_1258", null);

		internal static string Globalization_cp_1361 => GetResourceString("Globalization_cp_1361", null);

		internal static string Globalization_cp_10000 => GetResourceString("Globalization_cp_10000", null);

		internal static string Globalization_cp_10001 => GetResourceString("Globalization_cp_10001", null);

		internal static string Globalization_cp_10002 => GetResourceString("Globalization_cp_10002", null);

		internal static string Globalization_cp_10003 => GetResourceString("Globalization_cp_10003", null);

		internal static string Globalization_cp_10004 => GetResourceString("Globalization_cp_10004", null);

		internal static string Globalization_cp_10005 => GetResourceString("Globalization_cp_10005", null);

		internal static string Globalization_cp_10006 => GetResourceString("Globalization_cp_10006", null);

		internal static string Globalization_cp_10007 => GetResourceString("Globalization_cp_10007", null);

		internal static string Globalization_cp_10008 => GetResourceString("Globalization_cp_10008", null);

		internal static string Globalization_cp_10010 => GetResourceString("Globalization_cp_10010", null);

		internal static string Globalization_cp_10017 => GetResourceString("Globalization_cp_10017", null);

		internal static string Globalization_cp_10021 => GetResourceString("Globalization_cp_10021", null);

		internal static string Globalization_cp_10029 => GetResourceString("Globalization_cp_10029", null);

		internal static string Globalization_cp_10079 => GetResourceString("Globalization_cp_10079", null);

		internal static string Globalization_cp_10081 => GetResourceString("Globalization_cp_10081", null);

		internal static string Globalization_cp_10082 => GetResourceString("Globalization_cp_10082", null);

		internal static string Globalization_cp_20000 => GetResourceString("Globalization_cp_20000", null);

		internal static string Globalization_cp_20001 => GetResourceString("Globalization_cp_20001", null);

		internal static string Globalization_cp_20002 => GetResourceString("Globalization_cp_20002", null);

		internal static string Globalization_cp_20003 => GetResourceString("Globalization_cp_20003", null);

		internal static string Globalization_cp_20004 => GetResourceString("Globalization_cp_20004", null);

		internal static string Globalization_cp_20005 => GetResourceString("Globalization_cp_20005", null);

		internal static string Globalization_cp_20105 => GetResourceString("Globalization_cp_20105", null);

		internal static string Globalization_cp_20106 => GetResourceString("Globalization_cp_20106", null);

		internal static string Globalization_cp_20107 => GetResourceString("Globalization_cp_20107", null);

		internal static string Globalization_cp_20108 => GetResourceString("Globalization_cp_20108", null);

		internal static string Globalization_cp_20261 => GetResourceString("Globalization_cp_20261", null);

		internal static string Globalization_cp_20269 => GetResourceString("Globalization_cp_20269", null);

		internal static string Globalization_cp_20273 => GetResourceString("Globalization_cp_20273", null);

		internal static string Globalization_cp_20277 => GetResourceString("Globalization_cp_20277", null);

		internal static string Globalization_cp_20278 => GetResourceString("Globalization_cp_20278", null);

		internal static string Globalization_cp_20280 => GetResourceString("Globalization_cp_20280", null);

		internal static string Globalization_cp_20284 => GetResourceString("Globalization_cp_20284", null);

		internal static string Globalization_cp_20285 => GetResourceString("Globalization_cp_20285", null);

		internal static string Globalization_cp_20290 => GetResourceString("Globalization_cp_20290", null);

		internal static string Globalization_cp_20297 => GetResourceString("Globalization_cp_20297", null);

		internal static string Globalization_cp_20420 => GetResourceString("Globalization_cp_20420", null);

		internal static string Globalization_cp_20423 => GetResourceString("Globalization_cp_20423", null);

		internal static string Globalization_cp_20424 => GetResourceString("Globalization_cp_20424", null);

		internal static string Globalization_cp_20833 => GetResourceString("Globalization_cp_20833", null);

		internal static string Globalization_cp_20838 => GetResourceString("Globalization_cp_20838", null);

		internal static string Globalization_cp_20866 => GetResourceString("Globalization_cp_20866", null);

		internal static string Globalization_cp_20871 => GetResourceString("Globalization_cp_20871", null);

		internal static string Globalization_cp_20880 => GetResourceString("Globalization_cp_20880", null);

		internal static string Globalization_cp_20905 => GetResourceString("Globalization_cp_20905", null);

		internal static string Globalization_cp_20924 => GetResourceString("Globalization_cp_20924", null);

		internal static string Globalization_cp_20932 => GetResourceString("Globalization_cp_20932", null);

		internal static string Globalization_cp_20936 => GetResourceString("Globalization_cp_20936", null);

		internal static string Globalization_cp_20949 => GetResourceString("Globalization_cp_20949", null);

		internal static string Globalization_cp_21025 => GetResourceString("Globalization_cp_21025", null);

		internal static string Globalization_cp_21027 => GetResourceString("Globalization_cp_21027", null);

		internal static string Globalization_cp_21866 => GetResourceString("Globalization_cp_21866", null);

		internal static string Globalization_cp_28592 => GetResourceString("Globalization_cp_28592", null);

		internal static string Globalization_cp_28593 => GetResourceString("Globalization_cp_28593", null);

		internal static string Globalization_cp_28594 => GetResourceString("Globalization_cp_28594", null);

		internal static string Globalization_cp_28595 => GetResourceString("Globalization_cp_28595", null);

		internal static string Globalization_cp_28596 => GetResourceString("Globalization_cp_28596", null);

		internal static string Globalization_cp_28597 => GetResourceString("Globalization_cp_28597", null);

		internal static string Globalization_cp_28598 => GetResourceString("Globalization_cp_28598", null);

		internal static string Globalization_cp_28599 => GetResourceString("Globalization_cp_28599", null);

		internal static string Globalization_cp_28603 => GetResourceString("Globalization_cp_28603", null);

		internal static string Globalization_cp_28605 => GetResourceString("Globalization_cp_28605", null);

		internal static string Globalization_cp_29001 => GetResourceString("Globalization_cp_29001", null);

		internal static string Globalization_cp_38598 => GetResourceString("Globalization_cp_38598", null);

		internal static string Globalization_cp_50000 => GetResourceString("Globalization_cp_50000", null);

		internal static string Globalization_cp_50220 => GetResourceString("Globalization_cp_50220", null);

		internal static string Globalization_cp_50221 => GetResourceString("Globalization_cp_50221", null);

		internal static string Globalization_cp_50222 => GetResourceString("Globalization_cp_50222", null);

		internal static string Globalization_cp_50225 => GetResourceString("Globalization_cp_50225", null);

		internal static string Globalization_cp_50227 => GetResourceString("Globalization_cp_50227", null);

		internal static string Globalization_cp_50229 => GetResourceString("Globalization_cp_50229", null);

		internal static string Globalization_cp_50930 => GetResourceString("Globalization_cp_50930", null);

		internal static string Globalization_cp_50931 => GetResourceString("Globalization_cp_50931", null);

		internal static string Globalization_cp_50933 => GetResourceString("Globalization_cp_50933", null);

		internal static string Globalization_cp_50935 => GetResourceString("Globalization_cp_50935", null);

		internal static string Globalization_cp_50937 => GetResourceString("Globalization_cp_50937", null);

		internal static string Globalization_cp_50939 => GetResourceString("Globalization_cp_50939", null);

		internal static string Globalization_cp_51932 => GetResourceString("Globalization_cp_51932", null);

		internal static string Globalization_cp_51936 => GetResourceString("Globalization_cp_51936", null);

		internal static string Globalization_cp_51949 => GetResourceString("Globalization_cp_51949", null);

		internal static string Globalization_cp_52936 => GetResourceString("Globalization_cp_52936", null);

		internal static string Globalization_cp_54936 => GetResourceString("Globalization_cp_54936", null);

		internal static string Globalization_cp_57002 => GetResourceString("Globalization_cp_57002", null);

		internal static string Globalization_cp_57003 => GetResourceString("Globalization_cp_57003", null);

		internal static string Globalization_cp_57004 => GetResourceString("Globalization_cp_57004", null);

		internal static string Globalization_cp_57005 => GetResourceString("Globalization_cp_57005", null);

		internal static string Globalization_cp_57006 => GetResourceString("Globalization_cp_57006", null);

		internal static string Globalization_cp_57007 => GetResourceString("Globalization_cp_57007", null);

		internal static string Globalization_cp_57008 => GetResourceString("Globalization_cp_57008", null);

		internal static string Globalization_cp_57009 => GetResourceString("Globalization_cp_57009", null);

		internal static string Globalization_cp_57010 => GetResourceString("Globalization_cp_57010", null);

		internal static string Globalization_cp_57011 => GetResourceString("Globalization_cp_57011", null);

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool UsingResourceKeys()
		{
			return false;
		}

		internal static string GetResourceString(string resourceKey, string defaultString)
		{
			string text = null;
			try
			{
				text = ResourceManager.GetString(resourceKey);
			}
			catch (MissingManifestResourceException)
			{
			}
			if (defaultString != null && resourceKey.Equals(text, StringComparison.Ordinal))
			{
				return defaultString;
			}
			return text;
		}

		internal static string Format(string resourceFormat, params object[] args)
		{
			if (args != null)
			{
				if (UsingResourceKeys())
				{
					return resourceFormat + string.Join(", ", args);
				}
				return string.Format(resourceFormat, args);
			}
			return resourceFormat;
		}

		internal static string Format(string resourceFormat, object p1)
		{
			if (UsingResourceKeys())
			{
				return string.Join(", ", resourceFormat, p1);
			}
			return string.Format(resourceFormat, p1);
		}

		internal static string Format(string resourceFormat, object p1, object p2)
		{
			if (UsingResourceKeys())
			{
				return string.Join(", ", resourceFormat, p1, p2);
			}
			return string.Format(resourceFormat, p1, p2);
		}

		internal static string Format(string resourceFormat, object p1, object p2, object p3)
		{
			if (UsingResourceKeys())
			{
				return string.Join(", ", resourceFormat, p1, p2, p3);
			}
			return string.Format(resourceFormat, p1, p2, p3);
		}
	}
}
