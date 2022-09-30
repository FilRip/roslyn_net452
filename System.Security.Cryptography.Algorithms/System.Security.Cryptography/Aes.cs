using Internal.Cryptography;

namespace System.Security.Cryptography.Algorithms // FilRip : Added sub namespace "Algorithms"
{
	public abstract class Aes : SymmetricAlgorithm
	{
		private static readonly KeySizes[] s_legalBlockSizes = new KeySizes[1]
		{
			new KeySizes(128, 128, 0)
		};

		private static readonly KeySizes[] s_legalKeySizes = new KeySizes[1]
		{
			new KeySizes(128, 256, 64)
		};

		protected Aes()
		{
			LegalBlockSizesValue = s_legalBlockSizes.CloneKeySizesArray();
			LegalKeySizesValue = s_legalKeySizes.CloneKeySizesArray();
			BlockSizeValue = 128;
			FeedbackSizeValue = 8;
			KeySizeValue = 256;
			ModeValue = CipherMode.CBC;
		}

		public new static Aes Create()
		{
			return new Internal.Cryptography.Algorithms.AesImplementation();
		}

		public new static Aes Create(string algorithmName)
		{
			return (Aes)CryptoConfig.CreateFromName(algorithmName);
		}
	}
}
