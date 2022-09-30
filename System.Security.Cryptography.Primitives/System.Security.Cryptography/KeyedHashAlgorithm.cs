namespace System.Security.Cryptography
{
	public abstract class KeyedHashAlgorithm : HashAlgorithm
	{
		protected byte[] KeyValue;

		public virtual byte[] Key
		{
			get
			{
				return KeyValue.CloneByteArray();
			}
			set
			{
				KeyValue = value.CloneByteArray();
			}
		}

		public new static KeyedHashAlgorithm Create()
		{
			throw new PlatformNotSupportedException(SR.Cryptography_DefaultAlgorithm_NotSupported);
		}

		public new static KeyedHashAlgorithm Create(string algName)
		{
			return (KeyedHashAlgorithm)CryptoConfigForwarder.CreateFromName(algName);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (KeyValue != null)
				{
					Array.Clear(KeyValue, 0, KeyValue.Length);
				}
				KeyValue = null;
			}
			base.Dispose(disposing);
		}
	}
}
