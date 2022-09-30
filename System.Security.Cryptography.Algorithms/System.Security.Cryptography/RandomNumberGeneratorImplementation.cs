using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	internal sealed class RandomNumberGeneratorImplementation : RandomNumberGenerator
	{
		internal static void FillSpan(Span<byte> data)
		{
			if (data.Length > 0)
			{
				GetBytes(ref MemoryMarshal.GetReference(data), data.Length);
			}
		}

		public override void GetBytes(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			GetBytes(new Span<byte>(data));
		}

		public override void GetBytes(byte[] data, int offset, int count)
		{
			VerifyGetBytes(data, offset, count);
			GetBytes(new Span<byte>(data, offset, count));
		}

		public override void GetBytes(Span<byte> data)
		{
			if (data.Length > 0)
			{
				GetBytes(ref MemoryMarshal.GetReference(data), data.Length);
			}
		}

		public override void GetNonZeroBytes(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			GetNonZeroBytes(new Span<byte>(data));
		}

		public override void GetNonZeroBytes(Span<byte> data)
		{
			while (data.Length > 0)
			{
				GetBytes(data);
				int num = data.Length;
				for (int i = 0; i < data.Length; i++)
				{
					if (data[i] == 0)
					{
						num = i;
						break;
					}
				}
				for (int j = num + 1; j < data.Length; j++)
				{
					if (data[j] != 0)
					{
						data[num++] = data[j];
					}
				}
				data = data.Slice(num);
			}
		}

		private static void GetBytes(ref byte pbBuffer, int count)
		{
			Interop.BCrypt.NTSTATUS nTSTATUS = Interop.BCrypt.BCryptGenRandom(ref pbBuffer, count);
			if (nTSTATUS != 0)
			{
				throw Interop.BCrypt.CreateCryptographicException(nTSTATUS);
			}
		}
	}
}
