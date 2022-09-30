using System;

namespace Internal.Cryptography
{
	internal abstract class HashProvider : IDisposable
	{
		public abstract int HashSizeInBytes { get; }

		public void AppendHashData(byte[] data, int offset, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", System.SR.ArgumentNull_Buffer);
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", System.SR.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", System.SR.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (data.Length - offset < count)
			{
				throw new ArgumentException(System.SR.Argument_InvalidOffLen);
			}
			AppendHashData(new ReadOnlySpan<byte>(data, offset, count));
		}

		public abstract void AppendHashData(ReadOnlySpan<byte> data);

		public abstract byte[] FinalizeHashAndReset();

		public abstract bool TryFinalizeHashAndReset(Span<byte> destination, out int bytesWritten);

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public abstract void Dispose(bool disposing);
	}
}
