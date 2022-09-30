using System;
using System.Security.Cryptography;
using Internal.NativeCrypto;

namespace Internal.Cryptography
{
	internal sealed class DesImplementation : DES
	{
		private static readonly RandomNumberGenerator s_rng = RandomNumberGenerator.Create();

		public override ICryptoTransform CreateDecryptor()
		{
			return CreateTransform(Key, IV, encrypting: false);
		}

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return CreateTransform(rgbKey, rgbIV.CloneByteArray(), encrypting: false);
		}

		public override ICryptoTransform CreateEncryptor()
		{
			return CreateTransform(Key, IV, encrypting: true);
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return CreateTransform(rgbKey, rgbIV.CloneByteArray(), encrypting: true);
		}

		public override void GenerateIV()
		{
			byte[] array = new byte[BlockSize / 8];
			s_rng.GetBytes(array);
			IV = array;
		}

		public sealed override void GenerateKey()
		{
			byte[] array = new byte[KeySize / 8];
			s_rng.GetBytes(array);
			while (DES.IsWeakKey(array) || DES.IsSemiWeakKey(array))
			{
				s_rng.GetBytes(array);
			}
			KeyValue = array;
		}

		private ICryptoTransform CreateTransform(byte[] rgbKey, byte[] rgbIV, bool encrypting)
		{
			if (rgbKey == null)
			{
				throw new ArgumentNullException("rgbKey");
			}
			long num = (long)rgbKey.Length * 8L;
			if (num > int.MaxValue || !((int)num).IsLegalSize(LegalKeySizes))
			{
				throw new ArgumentException(System.SR.Cryptography_InvalidKeySize, "rgbKey");
			}
			if (DES.IsWeakKey(rgbKey))
			{
				throw new CryptographicException(System.SR.Cryptography_InvalidKey_Weak, "DES");
			}
			if (DES.IsSemiWeakKey(rgbKey))
			{
				throw new CryptographicException(System.SR.Cryptography_InvalidKey_SemiWeak, "DES");
			}
			if (rgbIV != null)
			{
				long num2 = (long)rgbIV.Length * 8L;
				if (num2 != BlockSize)
				{
					throw new ArgumentException(System.SR.Cryptography_InvalidIVSize, "rgbIV");
				}
			}
			return CreateTransformCore(Mode, Padding, rgbKey, rgbIV, BlockSize / 8, encrypting);
		}

		private static ICryptoTransform CreateTransformCore(CipherMode cipherMode, PaddingMode paddingMode, byte[] key, byte[] iv, int blockSize, bool encrypting)
		{
			SafeAlgorithmHandle sharedHandle = DesBCryptModes.GetSharedHandle(cipherMode);
			BasicSymmetricCipher cipher = new BasicSymmetricCipherBCrypt(sharedHandle, cipherMode, blockSize, key, ownsParentHandle: false, iv, encrypting);
			return UniversalCryptoTransform.Create(paddingMode, cipher, encrypting);
		}
	}
}
