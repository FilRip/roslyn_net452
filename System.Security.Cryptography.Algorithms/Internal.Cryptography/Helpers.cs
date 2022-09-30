using System;
using System.Security.Cryptography;

namespace Internal.Cryptography
{
	internal static class Helpers
	{
		public static byte[] CloneByteArray(this byte[] src)
		{
			if (src == null)
			{
				return null;
			}
			return (byte[])src.Clone();
		}

		public static KeySizes[] CloneKeySizesArray(this KeySizes[] src)
		{
			return (KeySizes[])src.Clone();
		}

		public static bool UsesIv(this CipherMode cipherMode)
		{
			return cipherMode != CipherMode.ECB;
		}

		public static byte[] GetCipherIv(this CipherMode cipherMode, byte[] iv)
		{
			if (cipherMode.UsesIv())
			{
				if (iv == null)
				{
					throw new CryptographicException(System.SR.Cryptography_MissingIV);
				}
				return iv;
			}
			return null;
		}

		public static bool IsLegalSize(this int size, KeySizes[] legalSizes)
		{
			foreach (KeySizes keySizes in legalSizes)
			{
				if (keySizes.SkipSize == 0)
				{
					if (keySizes.MinSize == size)
					{
						return true;
					}
				}
				else if (size >= keySizes.MinSize && size <= keySizes.MaxSize)
				{
					int num = size - keySizes.MinSize;
					if (num % keySizes.SkipSize == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static byte[] GenerateRandom(int count)
		{
			byte[] array = new byte[count];
			using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
			randomNumberGenerator.GetBytes(array);
			return array;
		}

		public static void WriteInt(uint i, byte[] arr, int offset)
		{
			arr[offset] = (byte)(i >> 24);
			arr[offset + 1] = (byte)(i >> 16);
			arr[offset + 2] = (byte)(i >> 8);
			arr[offset + 3] = (byte)i;
		}

		public static byte[] FixupKeyParity(this byte[] key)
		{
			byte[] array = new byte[key.Length];
			for (int i = 0; i < key.Length; i++)
			{
				array[i] = (byte)(key[i] & 0xFEu);
				byte b = (byte)((array[i] & 0xFu) ^ (uint)(array[i] >> 4));
				byte b2 = (byte)((b & 3u) ^ (uint)(b >> 2));
				if ((byte)((b2 & 1) ^ (b2 >> 1)) == 0)
				{
					array[i] |= 1;
				}
			}
			return array;
		}

		internal static void ConvertIntToByteArray(uint value, byte[] dest)
		{
			dest[0] = (byte)((value & 0xFF000000u) >> 24);
			dest[1] = (byte)((value & 0xFF0000) >> 16);
			dest[2] = (byte)((value & 0xFF00) >> 8);
			dest[3] = (byte)(value & 0xFFu);
		}
	}
}
