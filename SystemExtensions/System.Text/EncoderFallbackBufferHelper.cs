namespace System.Text
{
    internal struct EncoderFallbackBufferHelper
    {
        internal unsafe char* charStart;

        internal unsafe char* charEnd;

        internal EncoderNls encoder;

        internal bool setEncoder;

        internal bool bUsedEncoder;

        internal bool bFallingBack;

        internal int iRecursionCount;

        private const int iMaxRecursion = 250;

        private readonly EncoderFallbackBuffer _fallbackBuffer;

        public unsafe EncoderFallbackBufferHelper(EncoderFallbackBuffer fallbackBuffer)
        {
            _fallbackBuffer = fallbackBuffer;
            bFallingBack = (bUsedEncoder = (setEncoder = false));
            iRecursionCount = 0;
            charEnd = (charStart = null);
            encoder = null;
        }

        internal unsafe void InternalReset()
        {
            charStart = null;
            bFallingBack = false;
            iRecursionCount = 0;
            _fallbackBuffer.Reset();
        }

        internal unsafe void InternalInitialize(char* _charStart, char* _charEnd, EncoderNls _encoder, bool _setEncoder)
        {
            charStart = _charStart;
            charEnd = _charEnd;
            encoder = _encoder;
            setEncoder = _setEncoder;
            bUsedEncoder = false;
            bFallingBack = false;
            iRecursionCount = 0;
        }

        internal char InternalGetNextChar()
        {
            char nextChar = _fallbackBuffer.GetNextChar();
            bFallingBack = nextChar != '\0';
            if (nextChar == '\0')
            {
                iRecursionCount = 0;
            }
            return nextChar;
        }

        internal unsafe bool InternalFallback(char ch, ref char* chars)
        {
            int index = (int)(chars - charStart) - 1;
            if (char.IsHighSurrogate(ch))
            {
                if (chars >= charEnd)
                {
                    if (encoder != null && !encoder.MustFlush)
                    {
                        if (setEncoder)
                        {
                            bUsedEncoder = true;
                            encoder.charLeftOver = ch;
                        }
                        bFallingBack = false;
                        return false;
                    }
                }
                else
                {
                    char c = *chars;
                    if (char.IsLowSurrogate(c))
                    {
                        if (bFallingBack && iRecursionCount++ > iMaxRecursion)
                        {
                            ThrowLastCharRecursive(char.ConvertToUtf32(ch, c));
                        }
                        chars++;
                        bFallingBack = _fallbackBuffer.Fallback(ch, c, index);
                        return bFallingBack;
                    }
                }
            }
            if (bFallingBack && iRecursionCount++ > iMaxRecursion)
            {
                ThrowLastCharRecursive(ch);
            }
            bFallingBack = _fallbackBuffer.Fallback(ch, index);
            return bFallingBack;
        }

        internal void ThrowLastCharRecursive(int charRecursive)
        {
            throw new ArgumentException(SR.Format(SR.Argument_RecursiveFallback, charRecursive), nameof(charRecursive));
        }
    }
}
