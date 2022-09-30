using Internal.Cryptography;

namespace System.Security.Cryptography.Algorithms // FilRip : Added sub namespace "Algorithms"
{
	public abstract class MD5 : HashAlgorithm
	{
		private sealed class Implementation : MD5
		{
			private readonly HashProvider _hashProvider;

			public Implementation()
			{
				_hashProvider = HashProviderDispenser.CreateHashProvider("MD5");
				HashSizeValue = _hashProvider.HashSizeInBytes * 8;
			}

			protected sealed override void HashCore(byte[] array, int ibStart, int cbSize)
			{
				_hashProvider.AppendHashData(array, ibStart, cbSize);
			}

			protected void HashCore(ReadOnlySpan<byte> source)
			{
				_hashProvider.AppendHashData(source);
			}

			protected sealed override byte[] HashFinal()
			{
				return _hashProvider.FinalizeHashAndReset();
			}

			protected bool TryHashFinal(Span<byte> destination, out int bytesWritten)
			{
				return _hashProvider.TryFinalizeHashAndReset(destination, out bytesWritten);
			}

			public sealed override void Initialize()
			{
			}

			protected sealed override void Dispose(bool disposing)
			{
				_hashProvider.Dispose(disposing);
				base.Dispose(disposing);
			}
		}

		public new static MD5 Create()
		{
			return new Implementation();
		}

		public new static MD5 Create(string algName)
		{
			return (MD5)CryptoConfig.CreateFromName(algName);
		}
	}
}
