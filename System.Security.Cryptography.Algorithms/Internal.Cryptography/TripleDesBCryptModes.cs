using System;
using System.Security.Cryptography;
using Internal.NativeCrypto;

namespace Internal.Cryptography
{
	internal static class TripleDesBCryptModes
	{
		private static readonly SafeAlgorithmHandle s_hAlgCbc = Open3DesAlgorithm("ChainingModeCBC");

		private static readonly SafeAlgorithmHandle s_hAlgEcb = Open3DesAlgorithm("ChainingModeECB");

		internal static SafeAlgorithmHandle GetSharedHandle(CipherMode cipherMode)
		{
			return cipherMode switch
			{
				CipherMode.CBC => s_hAlgCbc, 
				CipherMode.ECB => s_hAlgEcb, 
				_ => throw new NotSupportedException(), 
			};
		}

		private static SafeAlgorithmHandle Open3DesAlgorithm(string cipherMode)
		{
			SafeAlgorithmHandle safeAlgorithmHandle = Internal.NativeCrypto.Cng.BCryptOpenAlgorithmProvider("3DES", null, Internal.NativeCrypto.Cng.OpenAlgorithmProviderFlags.NONE);
			safeAlgorithmHandle.SetCipherMode(cipherMode);
			return safeAlgorithmHandle;
		}
	}
}
