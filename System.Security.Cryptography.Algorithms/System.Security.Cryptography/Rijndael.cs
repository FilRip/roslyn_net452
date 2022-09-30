using System.ComponentModel;
using Internal.Cryptography;

namespace System.Security.Cryptography
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class Rijndael : SymmetricAlgorithm
	{
		private static readonly KeySizes[] s_legalBlockSizes = new KeySizes[1]
		{
			new KeySizes(128, 256, 64)
		};

		private static readonly KeySizes[] s_legalKeySizes = new KeySizes[1]
		{
			new KeySizes(128, 256, 64)
		};

		public new static Rijndael Create()
		{
			return new RijndaelImplementation();
		}

		public new static Rijndael Create(string algName)
		{
			return (Rijndael)CryptoConfig.CreateFromName(algName);
		}

		protected Rijndael()
		{
			LegalBlockSizesValue = s_legalBlockSizes.CloneKeySizesArray();
			LegalKeySizesValue = s_legalKeySizes.CloneKeySizesArray();
			KeySizeValue = 256;
			BlockSizeValue = 128;
			FeedbackSizeValue = BlockSizeValue;
		}
	}
}
