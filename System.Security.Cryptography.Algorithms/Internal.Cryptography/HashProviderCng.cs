using System;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography
{
	internal sealed class HashProviderCng : HashProvider
	{
		private readonly SafeBCryptAlgorithmHandle _hAlgorithm;

		private SafeBCryptHashHandle _hHash;

		private byte[] _key;

		private readonly bool _reusable;

		private readonly int _hashSize;

		public sealed override int HashSizeInBytes => _hashSize;

		public unsafe HashProviderCng(string hashAlgId, byte[] key)
		{
			Interop.BCrypt.BCryptOpenAlgorithmProviderFlags bCryptOpenAlgorithmProviderFlags = Interop.BCrypt.BCryptOpenAlgorithmProviderFlags.None;
			if (key != null)
			{
				_key = key.CloneByteArray();
				bCryptOpenAlgorithmProviderFlags |= Interop.BCrypt.BCryptOpenAlgorithmProviderFlags.BCRYPT_ALG_HANDLE_HMAC_FLAG;
			}
			_hAlgorithm = Interop.BCrypt.BCryptAlgorithmCache.GetCachedBCryptAlgorithmHandle(hashAlgId, bCryptOpenAlgorithmProviderFlags);
			SafeBCryptHashHandle phHash = null;
			Interop.BCrypt.NTSTATUS nTSTATUS = Interop.BCrypt.BCryptCreateHash(_hAlgorithm, out phHash, IntPtr.Zero, 0, key, (key != null) ? key.Length : 0, Interop.BCrypt.BCryptCreateHashFlags.BCRYPT_HASH_REUSABLE_FLAG);
			switch (nTSTATUS)
			{
			case Interop.BCrypt.NTSTATUS.STATUS_INVALID_PARAMETER:
				ResetHashObject();
				break;
			default:
				throw Interop.BCrypt.CreateCryptographicException(nTSTATUS);
			case Interop.BCrypt.NTSTATUS.STATUS_SUCCESS:
				_hHash = phHash;
				_reusable = true;
				break;
			}
			int hashSize = default(int);
			int pcbResult;
			Interop.BCrypt.NTSTATUS nTSTATUS2 = Interop.BCrypt.BCryptGetProperty(_hHash, "HashDigestLength", &hashSize, 4, out pcbResult, 0);
			if (nTSTATUS2 != 0)
			{
				throw Interop.BCrypt.CreateCryptographicException(nTSTATUS2);
			}
			_hashSize = hashSize;
		}

		public sealed override void AppendHashData(ReadOnlySpan<byte> source)
		{
			Interop.BCrypt.NTSTATUS nTSTATUS = Interop.BCrypt.BCryptHashData(_hHash, source, source.Length, 0);
			if (nTSTATUS != 0)
			{
				throw Interop.BCrypt.CreateCryptographicException(nTSTATUS);
			}
		}

		public sealed override byte[] FinalizeHashAndReset()
		{
			byte[] array = new byte[_hashSize];
			int bytesWritten;
			bool flag = TryFinalizeHashAndReset(array, out bytesWritten);
			return array;
		}

		public override bool TryFinalizeHashAndReset(Span<byte> destination, out int bytesWritten)
		{
			if (destination.Length < _hashSize)
			{
				bytesWritten = 0;
				return false;
			}
			Interop.BCrypt.NTSTATUS nTSTATUS = Interop.BCrypt.BCryptFinishHash(_hHash, destination, _hashSize, 0);
			if (nTSTATUS != 0)
			{
				throw Interop.BCrypt.CreateCryptographicException(nTSTATUS);
			}
			bytesWritten = _hashSize;
			ResetHashObject();
			return true;
		}

		public sealed override void Dispose(bool disposing)
		{
			if (disposing)
			{
				DestroyHash();
				if (_key != null)
				{
					byte[] key = _key;
					_key = null;
					Array.Clear(key, 0, key.Length);
				}
			}
		}

		private void ResetHashObject()
		{
			if (!_reusable)
			{
				DestroyHash();
				SafeBCryptHashHandle phHash;
				Interop.BCrypt.NTSTATUS nTSTATUS = Interop.BCrypt.BCryptCreateHash(_hAlgorithm, out phHash, IntPtr.Zero, 0, _key, (_key != null) ? _key.Length : 0, Interop.BCrypt.BCryptCreateHashFlags.None);
				if (nTSTATUS != 0)
				{
					throw Interop.BCrypt.CreateCryptographicException(nTSTATUS);
				}
				_hHash = phHash;
			}
		}

		private void DestroyHash()
		{
			SafeBCryptHashHandle hHash = _hHash;
			_hHash = null;
			hHash?.Dispose();
		}
	}
}
