namespace System.Security.Cryptography
{
	public abstract class HMAC : KeyedHashAlgorithm
	{
		private string _hashName;

		private int _blockSizeValue = 64;

		protected int BlockSizeValue
		{
			get
			{
				return _blockSizeValue;
			}
			set
			{
				_blockSizeValue = value;
			}
		}

		public string HashName
		{
			get
			{
				return _hashName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("HashName");
				}
				if (_hashName != null && value != _hashName)
				{
					throw new PlatformNotSupportedException(SR.HashNameMultipleSetNotSupported);
				}
				_hashName = value;
			}
		}

		public override byte[] Key
		{
			get
			{
				return base.Key;
			}
			set
			{
				base.Key = value;
			}
		}

		public new static HMAC Create()
		{
			throw new PlatformNotSupportedException(SR.Cryptography_DefaultAlgorithm_NotSupported);
		}

		public new static HMAC Create(string algorithmName)
		{
			return (HMAC)CryptoConfigForwarder.CreateFromName(algorithmName);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected override void HashCore(byte[] rgb, int ib, int cb)
		{
			throw new PlatformNotSupportedException(SR.CryptoConfigNotSupported);
		}

		protected override void HashCore(ReadOnlySpan<byte> source)
		{
			throw new PlatformNotSupportedException(SR.CryptoConfigNotSupported);
		}

		protected override byte[] HashFinal()
		{
			throw new PlatformNotSupportedException(SR.CryptoConfigNotSupported);
		}

		protected override bool TryHashFinal(Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException(SR.CryptoConfigNotSupported);
		}

		public override void Initialize()
		{
			throw new PlatformNotSupportedException(SR.CryptoConfigNotSupported);
		}
	}
}
