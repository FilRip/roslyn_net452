using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class CRC32
	{
		private static readonly uint[] s_CRC32_LOOKUP_TABLE = InitCrc32Table();

		private const uint s_CRC32_poly = 3988292384u;

		private static readonly UnicodeEncoding s_encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: false);

		public static uint ComputeCRC32(string[] names)
		{
			uint num = uint.MaxValue;
			foreach (string value in names)
			{
				num = Crc32Update(num, s_encoding.GetBytes(CaseInsensitiveComparison.ToLower(value)));
			}
			return num;
		}

		private static uint Crc32Update(uint crc32, byte[] bytes)
		{
			foreach (byte b in bytes)
			{
				crc32 = s_CRC32_LOOKUP_TABLE[(byte)crc32 ^ b] ^ (crc32 >> 8);
			}
			return crc32;
		}

		private static uint CalcEntry(uint crc)
		{
			int num = 0;
			do
			{
				crc = ((((ulong)crc & 1uL) == 0L) ? (crc >> 1) : ((crc >> 1) ^ 0xEDB88320u));
				num++;
			}
			while (num <= 7);
			return crc;
		}

		private static uint[] InitCrc32Table()
		{
			uint[] array = new uint[256];
			uint num = 0u;
			do
			{
				uint num2 = (array[num] = CalcEntry(num));
				num++;
			}
			while (num <= 255);
			return array;
		}
	}
}
