using System;
using System.Security.Cryptography;
using Internal.NativeCrypto;

namespace Internal.Cryptography
{
	internal sealed class BasicSymmetricCipherBCrypt : BasicSymmetricCipher
	{
		private readonly bool _encrypting;

		private SafeKeyHandle _hKey;

		private byte[] _currentIv;

		public BasicSymmetricCipherBCrypt(SafeAlgorithmHandle algorithm, CipherMode cipherMode, int blockSizeInBytes, byte[] key, bool ownsParentHandle, byte[] iv, bool encrypting)
			: base(cipherMode.GetCipherIv(iv), blockSizeInBytes)
		{
			_encrypting = encrypting;
			if (base.IV != null)
			{
				_currentIv = new byte[base.IV.Length];
			}
			_hKey = algorithm.BCryptImportKey(key);
			if (ownsParentHandle)
			{
				_hKey.SetParentHandle(algorithm);
			}
			Reset();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				SafeKeyHandle hKey = _hKey;
				_hKey = null;
				hKey?.Dispose();
				byte[] currentIv = _currentIv;
				_currentIv = null;
				if (currentIv != null)
				{
					Array.Clear(currentIv, 0, currentIv.Length);
				}
			}
			base.Dispose(disposing);
		}

		public override int Transform(byte[] input, int inputOffset, int count, byte[] output, int outputOffset)
		{
			int num = ((!_encrypting) ? _hKey.BCryptDecrypt(input, inputOffset, count, _currentIv, output, outputOffset, output.Length - outputOffset) : _hKey.BCryptEncrypt(input, inputOffset, count, _currentIv, output, outputOffset, output.Length - outputOffset));
			if (num != count)
			{
				throw new CryptographicException(System.SR.Cryptography_UnexpectedTransformTruncation);
			}
			return num;
		}

		public override byte[] TransformFinal(byte[] input, int inputOffset, int count)
		{
			byte[] array = new byte[count];
			if (count != 0)
			{
				int num = Transform(input, inputOffset, count, array, 0);
			}
			Reset();
			return array;
		}

		private void Reset()
		{
			if (base.IV != null)
			{
				Buffer.BlockCopy(base.IV, 0, _currentIv, 0, base.IV.Length);
			}
		}
	}
}
