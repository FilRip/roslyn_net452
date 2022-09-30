namespace Internal.Cryptography
{
	internal static class HashProviderDispenser
	{
		public static HashProvider CreateHashProvider(string hashAlgorithmId)
		{
			return new HashProviderCng(hashAlgorithmId, null);
		}

		public static HashProvider CreateMacProvider(string hashAlgorithmId, byte[] key)
		{
			return new HashProviderCng(hashAlgorithmId, key);
		}
	}
}
