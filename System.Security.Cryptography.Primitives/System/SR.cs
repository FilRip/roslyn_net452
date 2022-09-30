using System.Resources;
using System.Runtime.CompilerServices;
using FxResources.System.Security.Cryptography.Primitives;

namespace System
{
	internal static class SR
	{
		private static ResourceManager s_resourceManager;

		private static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new ResourceManager(ResourceType));

		internal static Type ResourceType { get; } = typeof(FxResources.System.Security.Cryptography.Primitives.SR);


		internal static string Arg_CryptographyException => GetResourceString("Arg_CryptographyException", null);

		internal static string Argument_InvalidOffLen => GetResourceString("Argument_InvalidOffLen", null);

		internal static string Argument_InvalidValue => GetResourceString("Argument_InvalidValue", null);

		internal static string Argument_StreamNotReadable => GetResourceString("Argument_StreamNotReadable", null);

		internal static string Argument_StreamNotWritable => GetResourceString("Argument_StreamNotWritable", null);

		internal static string ArgumentOutOfRange_NeedNonNegNum => GetResourceString("ArgumentOutOfRange_NeedNonNegNum", null);

		internal static string Cryptography_CryptoStream_FlushFinalBlockTwice => GetResourceString("Cryptography_CryptoStream_FlushFinalBlockTwice", null);

		internal static string Cryptography_DefaultAlgorithm_NotSupported => GetResourceString("Cryptography_DefaultAlgorithm_NotSupported", null);

		internal static string Cryptography_HashNotYetFinalized => GetResourceString("Cryptography_HashNotYetFinalized", null);

		internal static string Cryptography_InvalidFeedbackSize => GetResourceString("Cryptography_InvalidFeedbackSize", null);

		internal static string Cryptography_InvalidBlockSize => GetResourceString("Cryptography_InvalidBlockSize", null);

		internal static string Cryptography_InvalidCipherMode => GetResourceString("Cryptography_InvalidCipherMode", null);

		internal static string Cryptography_InvalidIVSize => GetResourceString("Cryptography_InvalidIVSize", null);

		internal static string Cryptography_InvalidKeySize => GetResourceString("Cryptography_InvalidKeySize", null);

		internal static string Cryptography_InvalidPaddingMode => GetResourceString("Cryptography_InvalidPaddingMode", null);

		internal static string NotSupported_UnreadableStream => GetResourceString("NotSupported_UnreadableStream", null);

		internal static string NotSupported_UnseekableStream => GetResourceString("NotSupported_UnseekableStream", null);

		internal static string NotSupported_UnwritableStream => GetResourceString("NotSupported_UnwritableStream", null);

		internal static string HashNameMultipleSetNotSupported => GetResourceString("HashNameMultipleSetNotSupported", null);

		internal static string CryptoConfigNotSupported => GetResourceString("CryptoConfigNotSupported", null);

		internal static string InvalidOperation_IncorrectImplementation => GetResourceString("InvalidOperation_IncorrectImplementation", null);

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

		internal static string Format(string resourceFormat, object p1)
		{
			if (UsingResourceKeys())
			{
				return string.Join(", ", resourceFormat, p1);
			}
			return string.Format(resourceFormat, p1);
		}
	}
}
