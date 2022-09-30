namespace System.Security.Cryptography.Primitives // FilRip : Added sub namespace "Primitives"
{
	public abstract class SymmetricAlgorithm : IDisposable
	{
		protected CipherMode ModeValue;

		protected PaddingMode PaddingValue;

		protected byte[] KeyValue;

		protected byte[] IVValue;

		protected int BlockSizeValue;

		protected int FeedbackSizeValue;

		protected int KeySizeValue;

		protected KeySizes[] LegalBlockSizesValue;

		protected KeySizes[] LegalKeySizesValue;

		public virtual int FeedbackSize
		{
			get
			{
				return FeedbackSizeValue;
			}
			set
			{
				if (value <= 0 || value > BlockSizeValue || value % 8 != 0)
				{
					throw new CryptographicException(SR.Cryptography_InvalidFeedbackSize);
				}
				FeedbackSizeValue = value;
			}
		}

		public virtual int BlockSize
		{
			get
			{
				return BlockSizeValue;
			}
			set
			{
				if (!value.IsLegalSize(LegalBlockSizes, out var validatedByZeroSkipSizeKeySizes))
				{
					throw new CryptographicException(SR.Cryptography_InvalidBlockSize);
				}
				if (BlockSizeValue != value || validatedByZeroSkipSizeKeySizes)
				{
					BlockSizeValue = value;
					IVValue = null;
				}
			}
		}

		public virtual byte[] IV
		{
			get
			{
				if (IVValue == null)
				{
					GenerateIV();
				}
				return IVValue.CloneByteArray();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length != BlockSize / 8)
				{
					throw new CryptographicException(SR.Cryptography_InvalidIVSize);
				}
				IVValue = value.CloneByteArray();
			}
		}

		public virtual byte[] Key
		{
			get
			{
				if (KeyValue == null)
				{
					GenerateKey();
				}
				return KeyValue.CloneByteArray();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				long num = (long)value.Length * 8L;
				if (num > int.MaxValue || !ValidKeySize((int)num))
				{
					throw new CryptographicException(SR.Cryptography_InvalidKeySize);
				}
				KeySize = (int)num;
				KeyValue = value.CloneByteArray();
			}
		}

		public virtual int KeySize
		{
			get
			{
				return KeySizeValue;
			}
			set
			{
				if (!ValidKeySize(value))
				{
					throw new CryptographicException(SR.Cryptography_InvalidKeySize);
				}
				KeySizeValue = value;
				KeyValue = null;
			}
		}

		public virtual KeySizes[] LegalBlockSizes => (KeySizes[])LegalBlockSizesValue.Clone();

		public virtual KeySizes[] LegalKeySizes => (KeySizes[])LegalKeySizesValue.Clone();

		public virtual CipherMode Mode
		{
			get
			{
				return ModeValue;
			}
			set
			{
				if (value != CipherMode.CBC && value != CipherMode.ECB)
				{
					throw new CryptographicException(SR.Cryptography_InvalidCipherMode);
				}
				ModeValue = value;
			}
		}

		public virtual PaddingMode Padding
		{
			get
			{
				return PaddingValue;
			}
			set
			{
				if (value < PaddingMode.None || value > PaddingMode.ISO10126)
				{
					throw new CryptographicException(SR.Cryptography_InvalidPaddingMode);
				}
				PaddingValue = value;
			}
		}

		protected SymmetricAlgorithm()
		{
			ModeValue = CipherMode.CBC;
			PaddingValue = PaddingMode.PKCS7;
		}

		public static SymmetricAlgorithm Create()
		{
			throw new PlatformNotSupportedException(SR.Cryptography_DefaultAlgorithm_NotSupported);
		}

		public static SymmetricAlgorithm Create(string algName)
		{
			return (SymmetricAlgorithm)CryptoConfigForwarder.CreateFromName(algName);
		}

		public virtual ICryptoTransform CreateDecryptor()
		{
			return CreateDecryptor(Key, IV);
		}

		public abstract ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV);

		public virtual ICryptoTransform CreateEncryptor()
		{
			return CreateEncryptor(Key, IV);
		}

		public abstract ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV);

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void Clear()
		{
			((IDisposable)this).Dispose();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (KeyValue != null)
				{
					Array.Clear(KeyValue, 0, KeyValue.Length);
					KeyValue = null;
				}
				if (IVValue != null)
				{
					Array.Clear(IVValue, 0, IVValue.Length);
					IVValue = null;
				}
			}
		}

		public abstract void GenerateIV();

		public abstract void GenerateKey();

		public bool ValidKeySize(int bitLength)
		{
			KeySizes[] legalKeySizes = LegalKeySizes;
			if (legalKeySizes == null)
			{
				return false;
			}
			return bitLength.IsLegalSize(legalKeySizes);
		}
	}
}
