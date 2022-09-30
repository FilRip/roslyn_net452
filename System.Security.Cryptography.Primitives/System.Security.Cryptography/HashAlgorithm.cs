using System.Buffers;
using System.IO;

namespace System.Security.Cryptography.Primitives // FilRip : Added sub namespace "Primitives"
{
	public abstract class HashAlgorithm : IDisposable, ICryptoTransform
	{
		private bool _disposed;

		protected int HashSizeValue;

		protected internal byte[] HashValue;

		protected int State;

		public virtual int HashSize => HashSizeValue;

		public virtual byte[] Hash
		{
			get
			{
				if (_disposed)
				{
					throw new ObjectDisposedException(null);
				}
				if (State != 0)
				{
					throw new CryptographicUnexpectedOperationException(SR.Cryptography_HashNotYetFinalized);
				}
				return (byte[])HashValue?.Clone();
			}
		}

		public virtual int InputBlockSize => 1;

		public virtual int OutputBlockSize => 1;

		public virtual bool CanTransformMultipleBlocks => true;

		public virtual bool CanReuseTransform => true;

		public static HashAlgorithm Create()
		{
			throw new PlatformNotSupportedException(SR.Cryptography_DefaultAlgorithm_NotSupported);
		}

		public static HashAlgorithm Create(string hashName)
		{
			return (HashAlgorithm)CryptoConfigForwarder.CreateFromName(hashName);
		}

		public byte[] ComputeHash(byte[] buffer)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(null);
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			HashCore(buffer, 0, buffer.Length);
			return CaptureHashCodeAndReinitialize();
		}

		public bool TryComputeHash(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(null);
			}
			if (destination.Length < HashSizeValue / 8)
			{
				bytesWritten = 0;
				return false;
			}
			HashCore(source);
			if (!TryHashFinal(destination, out bytesWritten))
			{
				throw new InvalidOperationException(SR.InvalidOperation_IncorrectImplementation);
			}
			HashValue = null;
			Initialize();
			return true;
		}

		public byte[] ComputeHash(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", SR.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (count < 0 || count > buffer.Length)
			{
				throw new ArgumentException(SR.Argument_InvalidValue);
			}
			if (buffer.Length - count < offset)
			{
				throw new ArgumentException(SR.Argument_InvalidOffLen);
			}
			if (_disposed)
			{
				throw new ObjectDisposedException(null);
			}
			HashCore(buffer, offset, count);
			return CaptureHashCodeAndReinitialize();
		}

		public byte[] ComputeHash(Stream inputStream)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(null);
			}
			byte[] array = ArrayPool<byte>.Shared.Rent(4096);
			try
			{
				int cbSize;
				while ((cbSize = inputStream.Read(array, 0, array.Length)) > 0)
				{
					HashCore(array, 0, cbSize);
				}
				return CaptureHashCodeAndReinitialize();
			}
			finally
			{
				CryptographicOperations.ZeroMemory(array);
				ArrayPool<byte>.Shared.Return(array);
			}
		}

		private byte[] CaptureHashCodeAndReinitialize()
		{
			HashValue = HashFinal();
			byte[] result = (byte[])HashValue.Clone();
			Initialize();
			return result;
		}

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
				_disposed = true;
			}
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			ValidateTransformBlock(inputBuffer, inputOffset, inputCount);
			State = 1;
			HashCore(inputBuffer, inputOffset, inputCount);
			if (outputBuffer != null && (inputBuffer != outputBuffer || inputOffset != outputOffset))
			{
				Buffer.BlockCopy(inputBuffer, inputOffset, outputBuffer, outputOffset, inputCount);
			}
			return inputCount;
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			ValidateTransformBlock(inputBuffer, inputOffset, inputCount);
			HashCore(inputBuffer, inputOffset, inputCount);
			HashValue = CaptureHashCodeAndReinitialize();
			byte[] array;
			if (inputCount != 0)
			{
				array = new byte[inputCount];
				Buffer.BlockCopy(inputBuffer, inputOffset, array, 0, inputCount);
			}
			else
			{
				array = new byte[0];
			}
			State = 0;
			return array;
		}

		private void ValidateTransformBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			if (inputOffset < 0)
			{
				throw new ArgumentOutOfRangeException("inputOffset", SR.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (inputCount < 0 || inputCount > inputBuffer.Length)
			{
				throw new ArgumentException(SR.Argument_InvalidValue);
			}
			if (inputBuffer.Length - inputCount < inputOffset)
			{
				throw new ArgumentException(SR.Argument_InvalidOffLen);
			}
			if (_disposed)
			{
				throw new ObjectDisposedException(null);
			}
		}

		protected abstract void HashCore(byte[] array, int ibStart, int cbSize);

		protected abstract byte[] HashFinal();

		public abstract void Initialize();

		protected virtual void HashCore(ReadOnlySpan<byte> source)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(source.Length);
			try
			{
				source.CopyTo(array);
				HashCore(array, 0, source.Length);
			}
			finally
			{
				Array.Clear(array, 0, source.Length);
				ArrayPool<byte>.Shared.Return(array);
			}
		}

		protected virtual bool TryHashFinal(Span<byte> destination, out int bytesWritten)
		{
			int num = HashSizeValue / 8;
			if (destination.Length >= num)
			{
				byte[] array = HashFinal();
				if (array.Length == num)
				{
					new ReadOnlySpan<byte>(array).CopyTo(destination);
					bytesWritten = array.Length;
					return true;
				}
				throw new InvalidOperationException(SR.InvalidOperation_IncorrectImplementation);
			}
			bytesWritten = 0;
			return false;
		}
	}
}
