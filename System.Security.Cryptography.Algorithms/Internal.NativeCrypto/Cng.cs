using System;
using System.Runtime.InteropServices;
using Internal.Cryptography;

namespace Internal.NativeCrypto
{
	internal static class Cng
	{
		[Flags]
		public enum OpenAlgorithmProviderFlags
		{
			NONE = 0,
			BCRYPT_ALG_HANDLE_HMAC_FLAG = 8
		}

		private struct BCRYPT_KEY_DATA_BLOB_HEADER
		{
			public uint dwMagic;

			public uint dwVersion;

			public uint cbKeyData;
		}

		private enum NTSTATUS : uint
		{
			STATUS_SUCCESS = 0u,
			STATUS_NOT_FOUND = 3221226021u,
			STATUS_INVALID_PARAMETER = 3221225485u,
			STATUS_NO_MEMORY = 3221225495u
		}

		private static class Interop
		{
			[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
			public static extern NTSTATUS BCryptOpenAlgorithmProvider(out SafeAlgorithmHandle phAlgorithm, string pszAlgId, string pszImplementation, int dwFlags);

			[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
			public static extern NTSTATUS BCryptSetProperty(SafeAlgorithmHandle hObject, string pszProperty, string pbInput, int cbInput, int dwFlags);

			[DllImport("BCrypt.dll", CharSet = CharSet.Unicode, EntryPoint = "BCryptSetProperty")]
			private static extern NTSTATUS BCryptSetIntPropertyPrivate(SafeBCryptHandle hObject, string pszProperty, ref int pdwInput, int cbInput, int dwFlags);

			public static NTSTATUS BCryptSetIntProperty(SafeBCryptHandle hObject, string pszProperty, ref int pdwInput, int dwFlags)
			{
				return BCryptSetIntPropertyPrivate(hObject, pszProperty, ref pdwInput, 4, dwFlags);
			}

			[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
			public static extern NTSTATUS BCryptImportKey(SafeAlgorithmHandle hAlgorithm, IntPtr hImportKey, string pszBlobType, out SafeKeyHandle hKey, IntPtr pbKeyObject, int cbKeyObject, byte[] pbInput, int cbInput, int dwFlags);

			[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
			public unsafe static extern NTSTATUS BCryptEncrypt(SafeKeyHandle hKey, byte* pbInput, int cbInput, IntPtr paddingInfo, [In][Out] byte[] pbIV, int cbIV, byte* pbOutput, int cbOutput, out int cbResult, int dwFlags);

			[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
			public unsafe static extern NTSTATUS BCryptDecrypt(SafeKeyHandle hKey, byte* pbInput, int cbInput, IntPtr paddingInfo, [In][Out] byte[] pbIV, int cbIV, byte* pbOutput, int cbOutput, out int cbResult, int dwFlags);
		}

		public static SafeAlgorithmHandle BCryptOpenAlgorithmProvider(string pszAlgId, string pszImplementation, OpenAlgorithmProviderFlags dwFlags)
		{
			SafeAlgorithmHandle phAlgorithm = null;
			NTSTATUS nTSTATUS = Interop.BCryptOpenAlgorithmProvider(out phAlgorithm, pszAlgId, pszImplementation, (int)dwFlags);
			if (nTSTATUS != 0)
			{
				throw CreateCryptographicException(nTSTATUS);
			}
			return phAlgorithm;
		}

		public unsafe static SafeKeyHandle BCryptImportKey(this SafeAlgorithmHandle hAlg, byte[] key)
		{
			int num = key.Length;
			int num2 = sizeof(BCRYPT_KEY_DATA_BLOB_HEADER) + num;
			byte[] array = new byte[num2];
			fixed (byte* ptr = array)
			{
				BCRYPT_KEY_DATA_BLOB_HEADER* ptr2 = (BCRYPT_KEY_DATA_BLOB_HEADER*)ptr;
				ptr2->dwMagic = 1296188491u;
				ptr2->dwVersion = 1u;
				ptr2->cbKeyData = (uint)num;
			}
			Buffer.BlockCopy(key, 0, array, sizeof(BCRYPT_KEY_DATA_BLOB_HEADER), num);
			SafeKeyHandle hKey;
			NTSTATUS nTSTATUS = Interop.BCryptImportKey(hAlg, IntPtr.Zero, "KeyDataBlob", out hKey, IntPtr.Zero, 0, array, num2, 0);
			if (nTSTATUS != 0)
			{
				throw CreateCryptographicException(nTSTATUS);
			}
			return hKey;
		}

		public static void SetCipherMode(this SafeAlgorithmHandle hAlg, string cipherMode)
		{
			NTSTATUS nTSTATUS = Interop.BCryptSetProperty(hAlg, "ChainingMode", cipherMode, (cipherMode.Length + 1) * 2, 0);
			if (nTSTATUS != 0)
			{
				throw CreateCryptographicException(nTSTATUS);
			}
		}

		public static void SetEffectiveKeyLength(this SafeAlgorithmHandle hAlg, int effectiveKeyLength)
		{
			NTSTATUS nTSTATUS = Interop.BCryptSetIntProperty(hAlg, "EffectiveKeyLength", ref effectiveKeyLength, 0);
			if (nTSTATUS != 0)
			{
				throw CreateCryptographicException(nTSTATUS);
			}
		}

		public unsafe static int BCryptEncrypt(this SafeKeyHandle hKey, byte[] input, int inputOffset, int inputCount, byte[] iv, byte[] output, int outputOffset, int outputCount)
		{
			fixed (byte* ptr = input)
			{
				fixed (byte* ptr2 = output)
				{
					int cbResult;
					NTSTATUS nTSTATUS = Interop.BCryptEncrypt(hKey, ptr + inputOffset, inputCount, IntPtr.Zero, iv, (iv != null) ? iv.Length : 0, ptr2 + outputOffset, outputCount, out cbResult, 0);
					if (nTSTATUS != 0)
					{
						throw CreateCryptographicException(nTSTATUS);
					}
					return cbResult;
				}
			}
		}

		public unsafe static int BCryptDecrypt(this SafeKeyHandle hKey, byte[] input, int inputOffset, int inputCount, byte[] iv, byte[] output, int outputOffset, int outputCount)
		{
			fixed (byte* ptr = input)
			{
				fixed (byte* ptr2 = output)
				{
					int cbResult;
					NTSTATUS nTSTATUS = Interop.BCryptDecrypt(hKey, ptr + inputOffset, inputCount, IntPtr.Zero, iv, (iv != null) ? iv.Length : 0, ptr2 + outputOffset, outputCount, out cbResult, 0);
					if (nTSTATUS != 0)
					{
						throw CreateCryptographicException(nTSTATUS);
					}
					return cbResult;
				}
			}
		}

		private static Exception CreateCryptographicException(NTSTATUS ntStatus)
		{
			int hr = (int)(ntStatus | (NTSTATUS)16777216u);
			return hr.ToCryptographicException();
		}
	}
}
