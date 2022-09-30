namespace System.Security.Cryptography.Primitives // FilRip Added sub namespace "Primitives"
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

		public static bool IsLegalSize(this int size, KeySizes[] legalSizes)
		{
			bool validatedByZeroSkipSizeKeySizes;
			return size.IsLegalSize(legalSizes, out validatedByZeroSkipSizeKeySizes);
		}

		public static bool IsLegalSize(this int size, KeySizes[] legalSizes, out bool validatedByZeroSkipSizeKeySizes)
		{
			validatedByZeroSkipSizeKeySizes = false;
			foreach (KeySizes keySizes in legalSizes)
			{
				if (keySizes.SkipSize == 0)
				{
					if (keySizes.MinSize == size)
					{
						validatedByZeroSkipSizeKeySizes = true;
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
	}
}
