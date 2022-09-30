using System.ComponentModel;
using Internal.Cryptography;

namespace System.Security.Cryptography.Algorithms // FilRip : Added sub namespace "Algorithms"
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class SHA1Managed : SHA1
	{
		private readonly HashProvider _hashProvider;

		public SHA1Managed()
		{
			_hashProvider = HashProviderDispenser.CreateHashProvider("SHA1");
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
}
