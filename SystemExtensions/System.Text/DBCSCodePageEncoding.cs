using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Text
{
    internal class DbCsCodePageEncoding : BaseCodePageEncoding
    {
        internal class DbCsDecoder : DecoderNls
        {
            internal byte bLeftOver;

            internal override bool HasState => bLeftOver != 0;

            public DbCsDecoder(DbCsCodePageEncoding encoding)
                : base(encoding)
            {
            }

            public override void Reset()
            {
                bLeftOver = 0;
                m_fallbackBuffer?.Reset();
            }
        }

        protected unsafe char* mapBytesToUnicode = null;

        protected unsafe ushort* mapUnicodeToBytes = null;

        protected const char UNKNOWN_CHAR_FLAG = '\0';

        protected const char UNICODE_REPLACEMENT_CHAR = '\ufffd';

        protected const char LEAD_BYTE_CHAR = '\ufffe';

        protected char charUnknown;

        private static object s_InternalSyncObject;

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object value = new();
                    Interlocked.CompareExchange<object>(ref s_InternalSyncObject, value, null);
                }
                return s_InternalSyncObject;
            }
        }

        public DbCsCodePageEncoding(int codePage)
            : base(codePage)
        {
        }

        protected unsafe override void LoadManagedCodePage()
        {
            fixed (byte* ptr = &m_codePageHeader[0])
            {
                ushort _bytesUnknown;
                int _byteCountUnknown;
                CodePageHeader* ptr2 = (CodePageHeader*)ptr;
                if (ptr2->ByteCount != 2)
                {
                    throw new NotSupportedException(SR.Format(SR.NotSupported_NoCodepageData, CodePage));
                }
                _bytesUnknown = ptr2->ByteReplace;
                charUnknown = ptr2->UnicodeReplace;
                if (base.DecoderFallback is InternalDecoderBestFitFallback fallback)
                {
                    fallback.cReplacement = charUnknown;
                }
                _byteCountUnknown = 1;
                if (_bytesUnknown > 255)
                {
                    _byteCountUnknown++;
                }
                int num = 262148 + iExtraBytes;
                byte* nativeMemory = GetNativeMemory(num);
                Unsafe.InitBlockUnaligned(nativeMemory, 0, (uint)num);
                mapBytesToUnicode = (char*)nativeMemory;
                mapUnicodeToBytes = (ushort*)(nativeMemory + 131072);
                byte[] array = new byte[m_dataSize];
                lock (s_streamLock)
                {
                    s_codePagesEncodingDataStream.Seek(m_firstDataWordOffset, SeekOrigin.Begin);
                    s_codePagesEncodingDataStream.Read(array, 0, m_dataSize);
                }
                fixed (byte* ptr3 = array)
                {
                    char* ptr4 = (char*)ptr3;
                    int num2 = 0;
                    int num3 = 0;
                    while (num2 < 65536)
                    {
                        char c = *ptr4;
                        ptr4++;
                        switch (c)
                        {
                            case '\u0001':
                                num2 = *ptr4;
                                ptr4++;
                                continue;
                            case '\u0002':
                            case '\u0003':
                            case '\u0004':
                            case '\u0005':
                            case '\u0006':
                            case '\a':
                            case '\b':
                            case '\t':
                            case '\n':
                            case '\v':
                            case '\f':
                            case '\r':
                            case '\u000e':
                            case '\u000f':
                            case '\u0010':
                            case '\u0011':
                            case '\u0012':
                            case '\u0013':
                            case '\u0014':
                            case '\u0015':
                            case '\u0016':
                            case '\u0017':
                            case '\u0018':
                            case '\u0019':
                            case '\u001a':
                            case '\u001b':
                            case '\u001c':
                            case '\u001d':
                            case '\u001e':
                            case '\u001f':
                                num2 += c;
                                continue;
                        }
                        switch (c)
                        {
                            case '\uffff':
                                num3 = num2;
                                c = (char)num2;
                                break;
                            case LEAD_BYTE_CHAR:
                                num3 = num2;
                                break;
                            case UNICODE_REPLACEMENT_CHAR:
                                num2++;
                                continue;
                            default:
                                num3 = num2;
                                break;
                        }
                        if (CleanUpBytes(ref num3))
                        {
                            if (c != LEAD_BYTE_CHAR)
                            {
                                mapUnicodeToBytes[c] = (ushort)num3;
                            }
                            mapBytesToUnicode[num3] = c;
                        }
                        num2++;
                    }
                }
                CleanUpEndBytes(mapBytesToUnicode);
            }
        }

        protected virtual bool CleanUpBytes(ref int bytes)
        {
            return true;
        }

        protected unsafe virtual void CleanUpEndBytes(char* chars)
        {
        }

        protected unsafe override void ReadBestFitTable()
        {
            lock (InternalSyncObject)
            {
                if (arrayUnicodeBestFit != null)
                {
                    return;
                }
                byte[] array = new byte[m_dataSize];
                lock (s_streamLock)
                {
                    s_codePagesEncodingDataStream.Seek(m_firstDataWordOffset, SeekOrigin.Begin);
                    s_codePagesEncodingDataStream.Read(array, 0, m_dataSize);
                }
                fixed (byte* ptr = array)
                {
                    char* ptr2 = (char*)ptr;
                    int num = 0;
                    while (num < 65536)
                    {
                        char c = *ptr2;
                        ptr2++;
                        switch (c)
                        {
                            case '\u0001':
                                num = *ptr2;
                                ptr2++;
                                break;
                            case '\u0002':
                            case '\u0003':
                            case '\u0004':
                            case '\u0005':
                            case '\u0006':
                            case '\a':
                            case '\b':
                            case '\t':
                            case '\n':
                            case '\v':
                            case '\f':
                            case '\r':
                            case '\u000e':
                            case '\u000f':
                            case '\u0010':
                            case '\u0011':
                            case '\u0012':
                            case '\u0013':
                            case '\u0014':
                            case '\u0015':
                            case '\u0016':
                            case '\u0017':
                            case '\u0018':
                            case '\u0019':
                            case '\u001a':
                            case '\u001b':
                            case '\u001c':
                            case '\u001d':
                            case '\u001e':
                            case '\u001f':
                                num += c;
                                break;
                            default:
                                num++;
                                break;
                        }
                    }
                    char* ptr3 = ptr2;
                    int num2 = 0;
                    num = *ptr2;
                    ptr2++;
                    while (num < 65536)
                    {
                        char c2 = *ptr2;
                        ptr2++;
                        switch (c2)
                        {
                            case '\u0001':
                                num = *ptr2;
                                ptr2++;
                                continue;
                            case '\u0002':
                            case '\u0003':
                            case '\u0004':
                            case '\u0005':
                            case '\u0006':
                            case '\a':
                            case '\b':
                            case '\t':
                            case '\n':
                            case '\v':
                            case '\f':
                            case '\r':
                            case '\u000e':
                            case '\u000f':
                            case '\u0010':
                            case '\u0011':
                            case '\u0012':
                            case '\u0013':
                            case '\u0014':
                            case '\u0015':
                            case '\u0016':
                            case '\u0017':
                            case '\u0018':
                            case '\u0019':
                            case '\u001a':
                            case '\u001b':
                            case '\u001c':
                            case '\u001d':
                            case '\u001e':
                            case '\u001f':
                                num += c2;
                                continue;
                        }
                        if (c2 != UNICODE_REPLACEMENT_CHAR)
                        {
                            int bytes = num;
                            if (CleanUpBytes(ref bytes) && mapBytesToUnicode[bytes] != c2)
                            {
                                num2++;
                            }
                        }
                        num++;
                    }
                    char[] array2 = new char[num2 * 2];
                    num2 = 0;
                    ptr2 = ptr3;
                    num = *ptr2;
                    ptr2++;
                    bool flag = false;
                    while (num < 65536)
                    {
                        char c3 = *ptr2;
                        ptr2++;
                        switch (c3)
                        {
                            case '\u0001':
                                num = *ptr2;
                                ptr2++;
                                continue;
                            case '\u0002':
                            case '\u0003':
                            case '\u0004':
                            case '\u0005':
                            case '\u0006':
                            case '\a':
                            case '\b':
                            case '\t':
                            case '\n':
                            case '\v':
                            case '\f':
                            case '\r':
                            case '\u000e':
                            case '\u000f':
                            case '\u0010':
                            case '\u0011':
                            case '\u0012':
                            case '\u0013':
                            case '\u0014':
                            case '\u0015':
                            case '\u0016':
                            case '\u0017':
                            case '\u0018':
                            case '\u0019':
                            case '\u001a':
                            case '\u001b':
                            case '\u001c':
                            case '\u001d':
                            case '\u001e':
                            case '\u001f':
                                num += c3;
                                continue;
                        }
                        if (c3 != UNICODE_REPLACEMENT_CHAR)
                        {
                            int bytes2 = num;
                            if (CleanUpBytes(ref bytes2) && mapBytesToUnicode[bytes2] != c3)
                            {
                                if (bytes2 != num)
                                {
                                    flag = true;
                                }
                                array2[num2++] = (char)bytes2;
                                array2[num2++] = c3;
                            }
                        }
                        num++;
                    }
                    if (flag)
                    {
                        for (int i = 0; i < array2.Length - 2; i += 2)
                        {
                            int num3 = i;
                            char c4 = array2[i];
                            for (int j = i + 2; j < array2.Length; j += 2)
                            {
                                if (c4 > array2[j])
                                {
                                    c4 = array2[j];
                                    num3 = j;
                                }
                            }
                            if (num3 != i)
                            {
                                (array2[num3], array2[i]) = (array2[i], array2[num3]);
                                (array2[num3 + 1], array2[i + 1]) = (array2[i + 1], array2[num3 + 1]);
                            }
                        }
                    }
                    arrayBytesBestFit = array2;
                    char* ptr4 = ptr2;
                    char* num4 = ptr2;
                    ptr2 = num4 + 1;
                    int num5 = *num4;
                    num2 = 0;
                    while (num5 < 65536)
                    {
                        char c6 = *ptr2;
                        ptr2++;
                        switch (c6)
                        {
                            case '\u0001':
                                num5 = *ptr2;
                                ptr2++;
                                continue;
                            case '\u0002':
                            case '\u0003':
                            case '\u0004':
                            case '\u0005':
                            case '\u0006':
                            case '\a':
                            case '\b':
                            case '\t':
                            case '\n':
                            case '\v':
                            case '\f':
                            case '\r':
                            case '\u000e':
                            case '\u000f':
                            case '\u0010':
                            case '\u0011':
                            case '\u0012':
                            case '\u0013':
                            case '\u0014':
                            case '\u0015':
                            case '\u0016':
                            case '\u0017':
                            case '\u0018':
                            case '\u0019':
                            case '\u001a':
                            case '\u001b':
                            case '\u001c':
                            case '\u001d':
                            case '\u001e':
                            case '\u001f':
                                num5 += c6;
                                continue;
                        }
                        if (c6 > '\0')
                        {
                            num2++;
                        }
                        num5++;
                    }
                    array2 = new char[num2 * 2];
                    ptr2 = ptr4;
                    char* num6 = ptr2;
                    ptr2 = num6 + 1;
                    num5 = *num6;
                    num2 = 0;
                    while (num5 < 65536)
                    {
                        char c7 = *ptr2;
                        ptr2++;
                        switch (c7)
                        {
                            case '\u0001':
                                num5 = *ptr2;
                                ptr2++;
                                continue;
                            case '\u0002':
                            case '\u0003':
                            case '\u0004':
                            case '\u0005':
                            case '\u0006':
                            case '\a':
                            case '\b':
                            case '\t':
                            case '\n':
                            case '\v':
                            case '\f':
                            case '\r':
                            case '\u000e':
                            case '\u000f':
                            case '\u0010':
                            case '\u0011':
                            case '\u0012':
                            case '\u0013':
                            case '\u0014':
                            case '\u0015':
                            case '\u0016':
                            case '\u0017':
                            case '\u0018':
                            case '\u0019':
                            case '\u001a':
                            case '\u001b':
                            case '\u001c':
                            case '\u001d':
                            case '\u001e':
                            case '\u001f':
                                num5 += c7;
                                continue;
                        }
                        if (c7 > '\0')
                        {
                            int bytes3 = c7;
                            if (CleanUpBytes(ref bytes3))
                            {
                                array2[num2++] = (char)num5;
                                array2[num2++] = mapBytesToUnicode[bytes3];
                            }
                        }
                        num5++;
                    }
                    arrayUnicodeBestFit = array2;
                }
            }
        }

        public unsafe override int GetByteCount(char* chars, int count, EncoderNls encoder)
        {
            CheckMemorySection();
            char c = UNKNOWN_CHAR_FLAG;
            if (encoder != null)
            {
                c = encoder.charLeftOver;
                if (encoder.InternalHasFallbackBuffer && encoder.FallbackBuffer.Remaining > 0)
                {
                    throw new ArgumentException(SR.Format(SR.Argument_EncoderFallbackNotEmpty, EncodingName, encoder.Fallback.GetType()));
                }
            }
            int num = 0;
            char* ptr = chars + count;
            EncoderFallbackBuffer encoderFallbackBuffer = null;
            EncoderFallbackBufferHelper encoderFallbackBufferHelper = new(encoderFallbackBuffer);
            if (c > UNKNOWN_CHAR_FLAG)
            {
                encoderFallbackBuffer = encoder?.FallbackBuffer;
                encoderFallbackBufferHelper = new(encoderFallbackBuffer);
                encoderFallbackBufferHelper.InternalInitialize(chars, ptr, encoder, _setEncoder: false);
                encoderFallbackBufferHelper.InternalFallback(c, ref chars);
            }
            char c2;
            while ((c2 = ((encoderFallbackBuffer != null) ? encoderFallbackBufferHelper.InternalGetNextChar() : '\0')) != 0 || chars < ptr)
            {
                if (c2 == UNKNOWN_CHAR_FLAG)
                {
                    c2 = *chars;
                    chars++;
                }
                ushort num2 = mapUnicodeToBytes[c2];
                if (num2 == 0 && c2 != 0)
                {
                    if (encoderFallbackBuffer == null)
                    {
                        encoderFallbackBuffer = ((encoder != null) ? encoder.FallbackBuffer : base.EncoderFallback.CreateFallbackBuffer());
                        encoderFallbackBufferHelper = new EncoderFallbackBufferHelper(encoderFallbackBuffer);
                        encoderFallbackBufferHelper.InternalInitialize(ptr - count, ptr, encoder, _setEncoder: false);
                    }
                    encoderFallbackBufferHelper.InternalFallback(c2, ref chars);
                }
                else
                {
                    num++;
                    if (num2 >= 256)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount, EncoderNls encoder)
        {
            CheckMemorySection();
            EncoderFallbackBuffer encoderFallbackBuffer = null;
            char* ptr = chars + charCount;
            char* ptr2 = chars;
            byte* ptr3 = bytes;
            byte* ptr4 = bytes + byteCount;
            EncoderFallbackBufferHelper encoderFallbackBufferHelper = new(encoderFallbackBuffer);
            char c;
            if (encoder != null)
            {
                c = encoder.charLeftOver;
                encoderFallbackBuffer = encoder.FallbackBuffer;
                encoderFallbackBufferHelper = new(encoderFallbackBuffer);
                encoderFallbackBufferHelper.InternalInitialize(chars, ptr, encoder, _setEncoder: true);
                if (encoder.m_throwOnOverflow && encoderFallbackBuffer.Remaining > 0)
                {
                    throw new ArgumentException(SR.Format(SR.Argument_EncoderFallbackNotEmpty, EncodingName, encoder.Fallback.GetType()));
                }
                if (c > UNKNOWN_CHAR_FLAG)
                {
                    encoderFallbackBufferHelper.InternalFallback(c, ref chars);
                }
            }
            char c2;
            while ((c2 = ((encoderFallbackBuffer != null) ? encoderFallbackBufferHelper.InternalGetNextChar() : '\0')) != 0 || chars < ptr)
            {
                if (c2 == UNKNOWN_CHAR_FLAG)
                {
                    c2 = *chars;
                    chars++;
                }
                ushort num = mapUnicodeToBytes[c2];
                if (num == 0 && c2 != 0)
                {
                    if (encoderFallbackBuffer == null)
                    {
                        encoderFallbackBuffer = base.EncoderFallback.CreateFallbackBuffer();
                        encoderFallbackBufferHelper = new EncoderFallbackBufferHelper(encoderFallbackBuffer);
                        encoderFallbackBufferHelper.InternalInitialize(ptr - charCount, ptr, encoder, _setEncoder: true);
                    }
                    encoderFallbackBufferHelper.InternalFallback(c2, ref chars);
                    continue;
                }
                if (num >= 256)
                {
                    if (bytes + 1 >= ptr4)
                    {
                        if (encoderFallbackBuffer == null || !encoderFallbackBufferHelper.bFallingBack)
                        {
                            chars--;
                        }
                        else
                        {
                            encoderFallbackBuffer.MovePrevious();
                        }
                        ThrowBytesOverflow(encoder, chars == ptr2);
                        break;
                    }
                    *bytes = (byte)(num >> 8);
                    bytes++;
                }
                else if (bytes >= ptr4)
                {
                    if (encoderFallbackBuffer == null || !encoderFallbackBufferHelper.bFallingBack)
                    {
                        chars--;
                    }
                    else
                    {
                        encoderFallbackBuffer.MovePrevious();
                    }
                    ThrowBytesOverflow(encoder, chars == ptr2);
                    break;
                }
                *bytes = (byte)(num & 0xFFu);
                bytes++;
            }
            if (encoder != null)
            {
                if (encoderFallbackBuffer != null && !encoderFallbackBufferHelper.bUsedEncoder)
                {
                    encoder.charLeftOver = UNKNOWN_CHAR_FLAG;
                }
                encoder.m_charsUsed = (int)(chars - ptr2);
            }
            return (int)(bytes - ptr3);
        }

        public unsafe override int GetCharCount(byte* bytes, int count, DecoderNls decoder)
        {
            CheckMemorySection();
            DbCsDecoder dBCSDecoder = (DbCsDecoder)decoder;
            DecoderFallbackBuffer decoderFallbackBuffer = null;
            byte* ptr = bytes + count;
            int num = count;
            DecoderFallbackBufferHelper decoderFallbackBufferHelper = new(decoderFallbackBuffer);
            if (dBCSDecoder != null && dBCSDecoder.bLeftOver > 0)
            {
                if (count == 0)
                {
                    if (!dBCSDecoder.MustFlush)
                    {
                        return 0;
                    }
                    decoderFallbackBuffer = dBCSDecoder.FallbackBuffer;
                    decoderFallbackBufferHelper = new DecoderFallbackBufferHelper(decoderFallbackBuffer);
                    decoderFallbackBufferHelper.InternalInitialize(bytes, null);
                    byte[] bytes2 = new byte[1] { dBCSDecoder.bLeftOver };
                    return decoderFallbackBufferHelper.InternalFallback(bytes2, bytes);
                }
                int num2 = dBCSDecoder.bLeftOver << 8;
                num2 |= *bytes;
                bytes++;
                if (mapBytesToUnicode[num2] == UNKNOWN_CHAR_FLAG && num2 != 0)
                {
                    num--;
                    decoderFallbackBuffer = dBCSDecoder.FallbackBuffer;
                    decoderFallbackBufferHelper = new DecoderFallbackBufferHelper(decoderFallbackBuffer);
                    decoderFallbackBufferHelper.InternalInitialize(ptr - count, null);
                    byte[] bytes3 = new byte[2]
                    {
                        (byte)(num2 >> 8),
                        (byte)num2
                    };
                    num += decoderFallbackBufferHelper.InternalFallback(bytes3, bytes);
                }
            }
            while (bytes < ptr)
            {
                int num3 = *bytes;
                bytes++;
                char c = mapBytesToUnicode[num3];
                if (c == LEAD_BYTE_CHAR)
                {
                    num--;
                    if (bytes < ptr)
                    {
                        num3 <<= 8;
                        num3 |= *bytes;
                        bytes++;
                        c = mapBytesToUnicode[num3];
                    }
                    else
                    {
                        if (dBCSDecoder != null && !dBCSDecoder.MustFlush)
                        {
                            break;
                        }
                        num++;
                        c = UNKNOWN_CHAR_FLAG;
                    }
                }
                if (c == '\0' && num3 != 0)
                {
                    if (decoderFallbackBuffer == null)
                    {
                        decoderFallbackBuffer = ((dBCSDecoder != null) ? dBCSDecoder.FallbackBuffer : base.DecoderFallback.CreateFallbackBuffer());
                        decoderFallbackBufferHelper = new DecoderFallbackBufferHelper(decoderFallbackBuffer);
                        decoderFallbackBufferHelper.InternalInitialize(ptr - count, null);
                    }
                    num--;
                    byte[] array = null;
                    array = ((num3 >= 256) ? new byte[2]
                    {
                        (byte)(num3 >> 8),
                        (byte)num3
                    } : new byte[1] { (byte)num3 });
                    num += decoderFallbackBufferHelper.InternalFallback(array, bytes);
                }
            }
            return num;
        }

        public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount, DecoderNls decoder)
        {
            CheckMemorySection();
            DbCsDecoder dBCSDecoder = (DbCsDecoder)decoder;
            byte* ptr = bytes;
            byte* ptr2 = bytes + byteCount;
            char* ptr3 = chars;
            char* ptr4 = chars + charCount;
            bool flag = false;
            DecoderFallbackBuffer decoderFallbackBuffer = null;
            DecoderFallbackBufferHelper decoderFallbackBufferHelper = new(decoderFallbackBuffer);
            if (dBCSDecoder != null && dBCSDecoder.bLeftOver > 0)
            {
                if (byteCount == 0)
                {
                    if (!dBCSDecoder.MustFlush)
                    {
                        return 0;
                    }
                    decoderFallbackBuffer = dBCSDecoder.FallbackBuffer;
                    decoderFallbackBufferHelper = new DecoderFallbackBufferHelper(decoderFallbackBuffer);
                    decoderFallbackBufferHelper.InternalInitialize(bytes, ptr4);
                    byte[] bytes2 = new byte[1] { dBCSDecoder.bLeftOver };
                    if (!decoderFallbackBufferHelper.InternalFallback(bytes2, bytes, ref chars))
                    {
                        ThrowCharsOverflow(dBCSDecoder, nothingDecoded: true);
                    }
                    dBCSDecoder.bLeftOver = 0;
                    return (int)(chars - ptr3);
                }
                int num = dBCSDecoder.bLeftOver << 8;
                num |= *bytes;
                bytes++;
                char c = mapBytesToUnicode[num];
                if (c == UNKNOWN_CHAR_FLAG && num != 0)
                {
                    decoderFallbackBuffer = dBCSDecoder.FallbackBuffer;
                    decoderFallbackBufferHelper = new DecoderFallbackBufferHelper(decoderFallbackBuffer);
                    decoderFallbackBufferHelper.InternalInitialize(ptr2 - byteCount, ptr4);
                    byte[] bytes3 = new byte[2]
                    {
                        (byte)(num >> 8),
                        (byte)num
                    };
                    if (!decoderFallbackBufferHelper.InternalFallback(bytes3, bytes, ref chars))
                    {
                        ThrowCharsOverflow(dBCSDecoder, nothingDecoded: true);
                    }
                }
                else
                {
                    if (chars >= ptr4)
                    {
                        ThrowCharsOverflow(dBCSDecoder, nothingDecoded: true);
                    }
                    char* num2 = chars;
                    chars = num2 + 1;
                    *num2 = c;
                }
            }
            while (bytes < ptr2)
            {
                int num3 = *bytes;
                bytes++;
                char c2 = mapBytesToUnicode[num3];
                if (c2 == LEAD_BYTE_CHAR)
                {
                    if (bytes < ptr2)
                    {
                        num3 <<= 8;
                        num3 |= *bytes;
                        bytes++;
                        c2 = mapBytesToUnicode[num3];
                    }
                    else
                    {
                        if (dBCSDecoder != null && !dBCSDecoder.MustFlush)
                        {
                            flag = true;
                            dBCSDecoder.bLeftOver = (byte)num3;
                            break;
                        }
                        c2 = UNKNOWN_CHAR_FLAG;
                    }
                }
                if (c2 == UNKNOWN_CHAR_FLAG && num3 != 0)
                {
                    if (decoderFallbackBuffer == null)
                    {
                        decoderFallbackBuffer = ((dBCSDecoder != null) ? dBCSDecoder.FallbackBuffer : base.DecoderFallback.CreateFallbackBuffer());
                        decoderFallbackBufferHelper = new DecoderFallbackBufferHelper(decoderFallbackBuffer);
                        decoderFallbackBufferHelper.InternalInitialize(ptr2 - byteCount, ptr4);
                    }
                    byte[] array = null;
                    array = ((num3 >= 256) ? new byte[2]
                    {
                        (byte)(num3 >> 8),
                        (byte)num3
                    } : new byte[1] { (byte)num3 });
                    if (!decoderFallbackBufferHelper.InternalFallback(array, bytes, ref chars))
                    {
                        bytes -= array.Length;
                        decoderFallbackBufferHelper.InternalReset();
                        ThrowCharsOverflow(dBCSDecoder, bytes == ptr);
                        break;
                    }
                    continue;
                }
                if (chars >= ptr4)
                {
                    bytes--;
                    if (num3 >= 256)
                    {
                        bytes--;
                    }
                    ThrowCharsOverflow(dBCSDecoder, bytes == ptr);
                    break;
                }
                char* num4 = chars;
                chars = num4 + 1;
                *num4 = c2;
            }
            if (dBCSDecoder != null)
            {
                if (!flag)
                {
                    dBCSDecoder.bLeftOver = 0;
                }
                dBCSDecoder.m_bytesUsed = (int)(bytes - ptr);
            }
            return (int)(chars - ptr3);
        }

        public override int GetMaxByteCount(int charCount)
        {
            if (charCount < 0)
            {
                throw new ArgumentOutOfRangeException("charCount", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            long num = charCount + 1L;
            if (base.EncoderFallback.MaxCharCount > 1)
            {
                num *= base.EncoderFallback.MaxCharCount;
            }
            num *= 2;
            if (num > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("charCount", SR.ArgumentOutOfRange_GetByteCountOverflow);
            }
            return (int)num;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            if (byteCount < 0)
            {
                throw new ArgumentOutOfRangeException("byteCount", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            long num = byteCount + 1L;
            if (base.DecoderFallback.MaxCharCount > 1)
            {
                num *= base.DecoderFallback.MaxCharCount;
            }
            if (num > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("byteCount", SR.ArgumentOutOfRange_GetCharCountOverflow);
            }
            return (int)num;
        }

        public override Decoder GetDecoder()
        {
            return new DbCsDecoder(this);
        }
    }
}
