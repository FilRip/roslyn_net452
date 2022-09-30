using Internal.Cryptography;

namespace System.Security.Cryptography
{
	public class PKCS1MaskGenerationMethod : MaskGenerationMethod
	{
		private string _hashNameValue;

		public string HashName
		{
			get
			{
				return _hashNameValue;
			}
			set
			{
				_hashNameValue = value ?? "SHA1";
			}
		}

		public PKCS1MaskGenerationMethod()
		{
			_hashNameValue = "SHA1";
		}

		public override byte[] GenerateMask(byte[] rgbSeed, int cbReturn)
		{
			using HashAlgorithm hashAlgorithm = (HashAlgorithm)CryptoConfig.CreateFromName(_hashNameValue);
			byte[] array = new byte[4];
			byte[] array2 = new byte[cbReturn];
			uint num = 0u;
			for (int i = 0; i < array2.Length; i += hashAlgorithm.Hash.Length)
			{
				Internal.Cryptography.Helpers.ConvertIntToByteArray(num++, array);
				hashAlgorithm.TransformBlock(rgbSeed, 0, rgbSeed.Length, rgbSeed, 0);
				hashAlgorithm.TransformFinalBlock(array, 0, 4);
				byte[] hash = hashAlgorithm.Hash;
				hashAlgorithm.Initialize();
				Buffer.BlockCopy(hash, 0, array2, i, Math.Min(array2.Length - i, hash.Length));
			}
			return array2;
		}
	}
}
