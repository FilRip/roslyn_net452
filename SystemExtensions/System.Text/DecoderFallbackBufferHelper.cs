using System.Globalization;

namespace System.Text
{
	internal struct DecoderFallbackBufferHelper
	{
		internal unsafe byte* byteStart;

		internal unsafe char* charEnd;

		private DecoderFallbackBuffer _fallbackBuffer;

		public unsafe DecoderFallbackBufferHelper(DecoderFallbackBuffer fallbackBuffer)
		{
			_fallbackBuffer = fallbackBuffer;
			byteStart = null;
			charEnd = null;
		}

		internal unsafe void InternalReset()
		{
			byteStart = null;
			_fallbackBuffer.Reset();
		}

		internal unsafe void InternalInitialize(byte* _byteStart, char* _charEnd)
		{
			byteStart = _byteStart;
			charEnd = _charEnd;
		}

		internal unsafe bool InternalFallback(byte[] bytes, byte* pBytes, ref char* chars)
		{
			if (_fallbackBuffer.Fallback(bytes, (int)(pBytes - byteStart - bytes.Length)))
			{
				char* ptr = chars;
				bool flag = false;
				char nextChar;
				while ((nextChar = _fallbackBuffer.GetNextChar()) != 0)
				{
					if (char.IsSurrogate(nextChar))
					{
						if (char.IsHighSurrogate(nextChar))
						{
							if (flag)
							{
								throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
							}
							flag = true;
						}
						else
						{
							if (!flag)
							{
								throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
							}
							flag = false;
						}
					}
					if (ptr >= charEnd)
					{
						return false;
					}
					char* num = ptr;
					ptr = num + 1;
					*num = nextChar;
				}
				if (flag)
				{
					throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
				}
				chars = ptr;
			}
			return true;
		}

		internal unsafe int InternalFallback(byte[] bytes, byte* pBytes)
		{
			if (_fallbackBuffer.Fallback(bytes, (int)(pBytes - byteStart - bytes.Length)))
			{
				int num = 0;
				bool flag = false;
				char nextChar;
				while ((nextChar = _fallbackBuffer.GetNextChar()) != 0)
				{
					if (char.IsSurrogate(nextChar))
					{
						if (char.IsHighSurrogate(nextChar))
						{
							if (flag)
							{
								throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
							}
							flag = true;
						}
						else
						{
							if (!flag)
							{
								throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
							}
							flag = false;
						}
					}
					num++;
				}
				if (flag)
				{
					throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
				}
				return num;
			}
			return 0;
		}

		internal void ThrowLastBytesRecursive(byte[] bytesUnknown)
		{
			StringBuilder stringBuilder = new StringBuilder(bytesUnknown.Length * 3);
			int i;
			for (i = 0; i < bytesUnknown.Length && i < 20; i++)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\\x{0:X2}", bytesUnknown[i]);
			}
			if (i == 20)
			{
				stringBuilder.Append(" ...");
			}
			throw new ArgumentException(SR.Format(SR.Argument_RecursiveFallbackBytes, stringBuilder.ToString()), "bytesUnknown");
		}
	}
}
