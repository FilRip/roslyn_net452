using System;
using System.Security.Cryptography;
using Internal.NativeCrypto;

namespace Internal.Cryptography
{
	internal static class AesBCryptModes
	{
		private static readonly SafeAlgorithmHandle s_hAlgCbc = OpenAesAlgorithm("ChainingModeCBC");

		private static readonly SafeAlgorithmHandle s_hAlgEcb = OpenAesAlgorithm("ChainingModeECB");

		internal static SafeAlgorithmHandle GetSharedHandle(CipherMode cipherMode)
		{
			return cipherMode switch
			{
				CipherMode.CBC => s_hAlgCbc, 
				CipherMode.ECB => s_hAlgEcb, 
				_ => throw new NotSupportedException(), 
			};
		}

		private static SafeAlgorithmHandle OpenAesAlgorithm(string cipherMode)
		{
			SafeAlgorithmHandle safeAlgorithmHandle = Internal.NativeCrypto.Cng.BCryptOpenAlgorithmProvider("AES", null, Internal.NativeCrypto.Cng.OpenAlgorithmProviderFlags.NONE);
			safeAlgorithmHandle.SetCipherMode(cipherMode);
			return safeAlgorithmHandle;
		}
	}
}
