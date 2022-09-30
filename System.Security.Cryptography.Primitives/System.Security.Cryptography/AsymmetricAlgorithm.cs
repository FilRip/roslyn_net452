namespace System.Security.Cryptography.Primitives // FilRip Added sub namespace "Primitives"
{
	public abstract class AsymmetricAlgorithm : IDisposable
	{
		protected int KeySizeValue;

		protected KeySizes[] LegalKeySizesValue;

		public virtual int KeySize
		{
			get
			{
				return KeySizeValue;
			}
			set
			{
				if (!value.IsLegalSize(LegalKeySizes))
				{
					throw new CryptographicException(SR.Cryptography_InvalidKeySize);
				}
				KeySizeValue = value;
			}
		}

		public virtual KeySizes[] LegalKeySizes => (KeySizes[])LegalKeySizesValue.Clone();

		public virtual string SignatureAlgorithm
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string KeyExchangeAlgorithm
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public static AsymmetricAlgorithm Create()
		{
			throw new PlatformNotSupportedException(SR.Cryptography_DefaultAlgorithm_NotSupported);
		}

		public static AsymmetricAlgorithm Create(string algName)
		{
			return (AsymmetricAlgorithm)CryptoConfigForwarder.CreateFromName(algName);
		}

		public virtual void FromXmlString(string xmlString)
		{
			throw new NotImplementedException();
		}

		public virtual string ToXmlString(bool includePrivateParameters)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void Dispose()
		{
			Clear();
		}

		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
