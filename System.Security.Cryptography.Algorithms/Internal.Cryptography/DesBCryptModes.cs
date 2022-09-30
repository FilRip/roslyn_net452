using System;
using System.Security.Cryptography;
using Internal.NativeCrypto;

namespace Internal.Cryptography
{
	internal static class DesBCryptModes
	{
		private static readonly SafeAlgorithmHandle s_hAlgCbc = OpenDesAlgorithm("ChainingModeCBC");

		private static readonly SafeAlgorithmHandle s_hAlgEcb = OpenDesAlgorithm("ChainingModeECB");

		internal static SafeAlgorithmHandle GetSharedHandle(CipherMode cipherMode)
		{
			return cipherMode switch
			{
				CipherMode.CBC => s_hAlgCbc, 
				CipherMode.ECB => s_hAlgEcb, 
				_ => throw new NotSupportedException(), 
			};
		}

		private static SafeAlgorithmHandle OpenDesAlgorithm(string cipherMode)
		{
			SafeAlgorithmHandle safeAlgorithmHandle = Internal.NativeCrypto.Cng.BCryptOpenAlgorithmProvider("DES", null, Internal.NativeCrypto.Cng.OpenAlgorithmProviderFlags.NONE);
			safeAlgorithmHandle.SetCipherMode(cipherMode);
			return safeAlgorithmHandle;
		}
	}
}
