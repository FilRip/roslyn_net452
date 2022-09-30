using System;

namespace Internal.Cryptography
{
	internal sealed class HMACCommon
	{
		private readonly string _hashAlgorithmId;

		private HashProvider _hMacProvider;

		private volatile HashProvider _lazyHashProvider;

		private readonly int _blockSize;

		public int HashSizeInBits => _hMacProvider.HashSizeInBytes * 8;

		public byte[] ActualKey { get; private set; }

		public HMACCommon(string hashAlgorithmId, byte[] key, int blockSize)
		{
			_hashAlgorithmId = hashAlgorithmId;
			_blockSize = blockSize;
			ChangeKey(key);
		}

		public void ChangeKey(byte[] key)
		{
			if (key.Length > _blockSize && _blockSize > 0)
			{
				if (_lazyHashProvider == null)
				{
					_lazyHashProvider = HashProviderDispenser.CreateHashProvider(_hashAlgorithmId);
				}
				_lazyHashProvider.AppendHashData(key, 0, key.Length);
				key = _lazyHashProvider.FinalizeHashAndReset();
			}
			HashProvider hMacProvider = _hMacProvider;
			_hMacProvider = null;
			hMacProvider?.Dispose(disposing: true);
			_hMacProvider = HashProviderDispenser.CreateMacProvider(_hashAlgorithmId, key);
			ActualKey = key;
		}

		public void AppendHashData(byte[] data, int offset, int count)
		{
			_hMacProvider.AppendHashData(data, offset, count);
		}

		public void AppendHashData(ReadOnlySpan<byte> source)
		{
			_hMacProvider.AppendHashData(source);
		}

		public byte[] FinalizeHashAndReset()
		{
			return _hMacProvider.FinalizeHashAndReset();
		}

		public bool TryFinalizeHashAndReset(Span<byte> destination, out int bytesWritten)
		{
			return _hMacProvider.TryFinalizeHashAndReset(destination, out bytesWritten);
		}

		public void Dispose(bool disposing)
		{
			if (disposing && _hMacProvider != null)
			{
				_hMacProvider.Dispose(disposing: true);
				_hMacProvider = null;
			}
		}
	}
}
