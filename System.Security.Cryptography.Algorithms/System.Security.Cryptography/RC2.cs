using System.ComponentModel;
using Internal.Cryptography;

namespace System.Security.Cryptography
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class RC2 : SymmetricAlgorithm
	{
		protected int EffectiveKeySizeValue;

		private static readonly KeySizes[] s_legalBlockSizes = new KeySizes[1]
		{
			new KeySizes(64, 64, 0)
		};

		private static readonly KeySizes[] s_legalKeySizes = new KeySizes[1]
		{
			new KeySizes(40, 1024, 8)
		};

		public override int KeySize
		{
			get
			{
				return KeySizeValue;
			}
			set
			{
				if (value < EffectiveKeySizeValue)
				{
					throw new CryptographicException(SR.Cryptography_RC2_EKSKS);
				}
				base.KeySize = value;
			}
		}

		public virtual int EffectiveKeySize
		{
			get
			{
				if (EffectiveKeySizeValue == 0)
				{
					return KeySizeValue;
				}
				return EffectiveKeySizeValue;
			}
			set
			{
				if (value > KeySizeValue)
				{
					throw new CryptographicException(SR.Cryptography_RC2_EKSKS);
				}
				if (value == 0)
				{
					EffectiveKeySizeValue = value;
					return;
				}
				if (value < 40)
				{
					throw new CryptographicException(SR.Cryptography_RC2_EKS40);
				}
				if (value.IsLegalSize(s_legalKeySizes))
				{
					EffectiveKeySizeValue = value;
					return;
				}
				throw new CryptographicException(SR.Cryptography_InvalidKeySize);
			}
		}

		protected RC2()
		{
			LegalBlockSizesValue = s_legalBlockSizes.CloneKeySizesArray();
			LegalKeySizesValue = s_legalKeySizes.CloneKeySizesArray();
			KeySizeValue = 128;
			BlockSizeValue = 64;
			FeedbackSizeValue = BlockSizeValue;
		}

		public new static RC2 Create()
		{
			return new RC2Implementation();
		}

		public new static RC2 Create(string AlgName)
		{
			return (RC2)CryptoConfig.CreateFromName(AlgName);
		}
	}
}
