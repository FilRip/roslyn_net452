using Internal.Cryptography;

namespace System.Security.Cryptography
{
	public sealed class IncrementalHash : IDisposable
	{
		private readonly HashAlgorithmName _algorithmName;

		private HashProvider _hash;

		private HMACCommon _hmac;

		private bool _disposed;

		public HashAlgorithmName AlgorithmName => _algorithmName;

		private IncrementalHash(HashAlgorithmName name, HashProvider hash)
		{
			_algorithmName = name;
			_hash = hash;
		}

		private IncrementalHash(HashAlgorithmName name, HMACCommon hmac)
		{
			_algorithmName = new HashAlgorithmName("HMAC" + name.Name);
			_hmac = hmac;
		}

		public void AppendData(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			AppendData(data, 0, data.Length);
		}

		public void AppendData(byte[] data, int offset, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", SR.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (count < 0 || count > data.Length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (data.Length - count < offset)
			{
				throw new ArgumentException(SR.Argument_InvalidOffLen);
			}
			if (_disposed)
			{
				throw new ObjectDisposedException(typeof(IncrementalHash).Name);
			}
			AppendData(new ReadOnlySpan<byte>(data, offset, count));
		}

		public void AppendData(ReadOnlySpan<byte> data)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(typeof(IncrementalHash).Name);
			}
			if (_hash != null)
			{
				_hash.AppendHashData(data);
			}
			else
			{
				_hmac.AppendHashData(data);
			}
		}

		public byte[] GetHashAndReset()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(typeof(IncrementalHash).Name);
			}
			if (_hash == null)
			{
				return _hmac.FinalizeHashAndReset();
			}
			return _hash.FinalizeHashAndReset();
		}

		public bool TryGetHashAndReset(Span<byte> destination, out int bytesWritten)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(typeof(IncrementalHash).Name);
			}
			if (_hash == null)
			{
				return _hmac.TryFinalizeHashAndReset(destination, out bytesWritten);
			}
			return _hash.TryFinalizeHashAndReset(destination, out bytesWritten);
		}

		public void Dispose()
		{
			_disposed = true;
			if (_hash != null)
			{
				_hash.Dispose();
				_hash = null;
			}
			if (_hmac != null)
			{
				_hmac.Dispose(disposing: true);
				_hmac = null;
			}
		}

		public static IncrementalHash CreateHash(HashAlgorithmName hashAlgorithm)
		{
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
			}
			return new IncrementalHash(hashAlgorithm, HashProviderDispenser.CreateHashProvider(hashAlgorithm.Name));
		}

		public static IncrementalHash CreateHMAC(HashAlgorithmName hashAlgorithm, byte[] key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, "hashAlgorithm");
			}
			return new IncrementalHash(hashAlgorithm, new HMACCommon(hashAlgorithm.Name, key, -1));
		}
	}
}
