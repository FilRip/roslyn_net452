namespace System.Security.Cryptography
{
	public abstract class ECDiffieHellmanPublicKey : IDisposable
	{
		private readonly byte[] _keyBlob;

		protected ECDiffieHellmanPublicKey()
		{
			_keyBlob = Array.Empty<byte>();
		}

		protected ECDiffieHellmanPublicKey(byte[] keyBlob)
		{
			if (keyBlob == null)
			{
				throw new ArgumentNullException("keyBlob");
			}
			_keyBlob = keyBlob.Clone() as byte[];
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public virtual byte[] ToByteArray()
		{
			return _keyBlob.Clone() as byte[];
		}

		public virtual string ToXmlString()
		{
			throw new NotImplementedException(SR.NotSupported_SubclassOverride);
		}

		public virtual ECParameters ExportParameters()
		{
			throw new NotSupportedException(SR.NotSupported_SubclassOverride);
		}

		public virtual ECParameters ExportExplicitParameters()
		{
			throw new NotSupportedException(SR.NotSupported_SubclassOverride);
		}
	}
}
