using System.Security.Cryptography;

namespace Internal.Cryptography
{
	internal static class CryptoThrowHelper
	{
		private sealed class WindowsCryptographicException : CryptographicException
		{
			public WindowsCryptographicException(int hr, string message)
				: base(message)
			{
				base.HResult = hr;
			}
		}

		public static CryptographicException ToCryptographicException(this int hr)
		{
			string message = Interop.Kernel32.GetMessage(hr);
			return new WindowsCryptographicException(hr, message);
		}
	}
}
