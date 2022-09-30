using System.Buffers;

namespace System.Security.Cryptography
{
	public abstract class RandomNumberGenerator : IDisposable
	{
		public static RandomNumberGenerator Create()
		{
			return new RandomNumberGeneratorImplementation();
		}

		public static RandomNumberGenerator Create(string rngName)
		{
			return (RandomNumberGenerator)CryptoConfig.CreateFromName(rngName);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public abstract void GetBytes(byte[] data);

		public virtual void GetBytes(byte[] data, int offset, int count)
		{
			VerifyGetBytes(data, offset, count);
			if (count > 0)
			{
				if (offset == 0 && count == data.Length)
				{
					GetBytes(data);
					return;
				}
				byte[] array = new byte[count];
				GetBytes(array);
				Buffer.BlockCopy(array, 0, data, offset, count);
			}
		}

		public virtual void GetBytes(Span<byte> data)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(data.Length);
			try
			{
				GetBytes(array, 0, data.Length);
				new ReadOnlySpan<byte>(array, 0, data.Length).CopyTo(data);
			}
			finally
			{
				Array.Clear(array, 0, data.Length);
				ArrayPool<byte>.Shared.Return(array);
			}
		}

		public virtual void GetNonZeroBytes(byte[] data)
		{
			throw new NotImplementedException();
		}

		public virtual void GetNonZeroBytes(Span<byte> data)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(data.Length);
			try
			{
				GetNonZeroBytes(array);
				new ReadOnlySpan<byte>(array, 0, data.Length).CopyTo(data);
			}
			finally
			{
				Array.Clear(array, 0, data.Length);
				ArrayPool<byte>.Shared.Return(array);
			}
		}

		public static void Fill(Span<byte> data)
		{
			RandomNumberGeneratorImplementation.FillSpan(data);
		}

		internal void VerifyGetBytes(byte[] data, int offset, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", SR.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", SR.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (count > data.Length - offset)
			{
				throw new ArgumentException(SR.Argument_InvalidOffLen);
			}
		}
	}
}
