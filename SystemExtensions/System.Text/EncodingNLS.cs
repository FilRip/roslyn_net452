namespace System.Text
{
    internal abstract class EncodingNLS : Encoding
    {
        private string _encodingName;

        private string _webName;

        private readonly EncoderFallback _encoder;
        private readonly DecoderFallback _decoder;

        public override string EncodingName
        {
            get
            {
                if (_encodingName == null)
                {
                    _encodingName = GetLocalizedEncodingNameResource(CodePage);
                    if (_encodingName == null)
                    {
                        throw new NotSupportedException(SR.Format(SR.MissingEncodingNameResource, WebName, CodePage));
                    }
                    if (_encodingName.StartsWith("Globalization_cp_", StringComparison.OrdinalIgnoreCase))
                    {
                        _encodingName = EncodingTable.GetEnglishNameFromCodePage(CodePage);
                        if (_encodingName == null)
                        {
                            throw new NotSupportedException(SR.Format(SR.MissingEncodingNameResource, WebName, CodePage));
                        }
                    }
                }
                return _encodingName;
            }
        }

        public override string WebName
        {
            get
            {
                if (_webName == null)
                {
                    _webName = EncodingTable.GetWebNameFromCodePage(CodePage);
                    if (_webName == null)
                    {
                        throw new NotSupportedException(SR.Format(SR.NotSupported_NoCodepageData, CodePage));
                    }
                }
                return _webName;
            }
        }

        protected EncodingNLS(int codePage)
            : base(codePage)
        {
        }

        protected EncodingNLS(int codePage, EncoderFallback enc, DecoderFallback dec)
			: base(codePage)
		{
            _encoder = enc;
            _decoder = dec;
		}

        public EncoderFallback Encoder
        {
            get { return _encoder; }
        }

        public DecoderFallback Decoder
        {
            get { return _decoder; }
        }

        public unsafe abstract int GetByteCount(char* chars, int count, EncoderNLS encoder);

        public unsafe abstract int GetBytes(char* chars, int charCount, byte* bytes, int byteCount, EncoderNLS encoder);

        public unsafe abstract int GetCharCount(byte* bytes, int count, DecoderNLS decoder);

        public unsafe abstract int GetChars(byte* bytes, int byteCount, char* chars, int charCount, DecoderNLS decoder);

        public unsafe override int GetByteCount(char[] chars, int index, int count)
        {
            if (chars == null)
            {
                throw new ArgumentNullException("chars", SR.ArgumentNull_Array);
            }
            if (index < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (chars.Length - index < count)
            {
                throw new ArgumentOutOfRangeException("chars", SR.ArgumentOutOfRange_IndexCountBuffer);
            }
            if (chars.Length == 0)
            {
                return 0;
            }
            fixed (char* ptr = &chars[0])
            {
                return GetByteCount(ptr + index, count, null);
            }
        }

        public unsafe override int GetByteCount(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            fixed (char* chars = s)
            {
                return GetByteCount(chars, s.Length, null);
            }
        }

        public unsafe override int GetByteCount(char* chars, int count)
        {
            if (chars == null)
            {
                throw new ArgumentNullException("chars", SR.ArgumentNull_Array);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            return GetByteCount(chars, count, null);
        }

        public unsafe override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            if (s == null || bytes == null)
            {
                throw new ArgumentNullException((s == null) ? "s" : "bytes", SR.ArgumentNull_Array);
            }
            if (charIndex < 0 || charCount < 0)
            {
                throw new ArgumentOutOfRangeException((charIndex < 0) ? "charIndex" : "charCount", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (s.Length - charIndex < charCount)
            {
                throw new ArgumentOutOfRangeException("s", SR.ArgumentOutOfRange_IndexCount);
            }
            if (byteIndex < 0 || byteIndex > bytes.Length)
            {
                throw new ArgumentOutOfRangeException("byteIndex", SR.ArgumentOutOfRange_Index);
            }
            int byteCount = bytes.Length - byteIndex;
            if (bytes.Length == 0)
            {
                bytes = new byte[1];
            }
            fixed (char* ptr = s)
            {
                fixed (byte* ptr2 = &bytes[0])
                {
                    return GetBytes(ptr + charIndex, charCount, ptr2 + byteIndex, byteCount, null);
                }
            }
        }

        public unsafe override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            if (chars == null || bytes == null)
            {
                throw new ArgumentNullException((chars == null) ? "chars" : "bytes", SR.ArgumentNull_Array);
            }
            if (charIndex < 0 || charCount < 0)
            {
                throw new ArgumentOutOfRangeException((charIndex < 0) ? "charIndex" : "charCount", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (chars.Length - charIndex < charCount)
            {
                throw new ArgumentOutOfRangeException("chars", SR.ArgumentOutOfRange_IndexCountBuffer);
            }
            if (byteIndex < 0 || byteIndex > bytes.Length)
            {
                throw new ArgumentOutOfRangeException("byteIndex", SR.ArgumentOutOfRange_Index);
            }
            if (chars.Length == 0)
            {
                return 0;
            }
            int byteCount = bytes.Length - byteIndex;
            if (bytes.Length == 0)
            {
                bytes = new byte[1];
            }
            fixed (char* ptr = &chars[0])
            {
                fixed (byte* ptr2 = &bytes[0])
                {
                    return GetBytes(ptr + charIndex, charCount, ptr2 + byteIndex, byteCount, null);
                }
            }
        }

        public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
        {
            if (bytes == null || chars == null)
            {
                throw new ArgumentNullException((bytes == null) ? "bytes" : "chars", SR.ArgumentNull_Array);
            }
            if (charCount < 0 || byteCount < 0)
            {
                throw new ArgumentOutOfRangeException((charCount < 0) ? "charCount" : "byteCount", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            return GetBytes(chars, charCount, bytes, byteCount, null);
        }

        public unsafe override int GetCharCount(byte[] bytes, int index, int count)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes", SR.ArgumentNull_Array);
            }
            if (index < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (bytes.Length - index < count)
            {
                throw new ArgumentOutOfRangeException("bytes", SR.ArgumentOutOfRange_IndexCountBuffer);
            }
            if (bytes.Length == 0)
            {
                return 0;
            }
            fixed (byte* ptr = &bytes[0])
            {
                return GetCharCount(ptr + index, count, null);
            }
        }

        public unsafe override int GetCharCount(byte* bytes, int count)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes", SR.ArgumentNull_Array);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            return GetCharCount(bytes, count, null);
        }

        public unsafe override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            if (bytes == null || chars == null)
            {
                throw new ArgumentNullException((bytes == null) ? "bytes" : "chars", SR.ArgumentNull_Array);
            }
            if (byteIndex < 0 || byteCount < 0)
            {
                throw new ArgumentOutOfRangeException((byteIndex < 0) ? "byteIndex" : "byteCount", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (bytes.Length - byteIndex < byteCount)
            {
                throw new ArgumentOutOfRangeException("bytes", SR.ArgumentOutOfRange_IndexCountBuffer);
            }
            if (charIndex < 0 || charIndex > chars.Length)
            {
                throw new ArgumentOutOfRangeException("charIndex", SR.ArgumentOutOfRange_Index);
            }
            if (bytes.Length == 0)
            {
                return 0;
            }
            int charCount = chars.Length - charIndex;
            if (chars.Length == 0)
            {
                chars = new char[1];
            }
            fixed (byte* ptr = &bytes[0])
            {
                fixed (char* ptr2 = &chars[0])
                {
                    return GetChars(ptr + byteIndex, byteCount, ptr2 + charIndex, charCount, null);
                }
            }
        }

        public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
        {
            if (bytes == null || chars == null)
            {
                throw new ArgumentNullException((bytes == null) ? "bytes" : "chars", SR.ArgumentNull_Array);
            }
            if (charCount < 0 || byteCount < 0)
            {
                throw new ArgumentOutOfRangeException((charCount < 0) ? "charCount" : "byteCount", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            return GetChars(bytes, byteCount, chars, charCount, null);
        }

        /*public unsafe override string GetString(byte[] bytes, int index, int count)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes", SR.ArgumentNull_Array);
			}
			if (index < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", SR.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (bytes.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("bytes", SR.ArgumentOutOfRange_IndexCountBuffer);
			}
			if (bytes.Length == 0)
			{
				return string.Empty;
			}
			fixed (byte* ptr = &bytes[0])
			{
				return GetString(ptr + index, count);
			}
		}*/ // FilRip : Commented to ignore

        public override Decoder GetDecoder()
        {
            return new DecoderNLS(this);
        }

        public override Encoder GetEncoder()
        {
            return new EncoderNLS(this);
        }

        internal void ThrowBytesOverflow(EncoderNLS encoder, bool nothingEncoded)
        {
            if ((encoder?.m_throwOnOverflow ?? true) || nothingEncoded)
            {
                if (encoder != null && encoder.InternalHasFallbackBuffer)
                {
                    encoder.FallbackBuffer.Reset();
                }
                ThrowBytesOverflow();
            }
            encoder.ClearMustFlush();
        }

        internal void ThrowCharsOverflow(DecoderNLS decoder, bool nothingDecoded)
        {
            if ((decoder?.m_throwOnOverflow ?? true) || nothingDecoded)
            {
                if (decoder != null && decoder.InternalHasFallbackBuffer)
                {
                    decoder.FallbackBuffer.Reset();
                }
                ThrowCharsOverflow();
            }
            decoder.ClearMustFlush();
        }

        internal void ThrowBytesOverflow()
        {
            throw new ArgumentException(SR.Format(SR.Argument_EncodingConversionOverflowBytes, EncodingName, base.EncoderFallback.GetType()), "bytes");
        }

        internal void ThrowCharsOverflow()
        {
            throw new ArgumentException(SR.Format(SR.Argument_EncodingConversionOverflowChars, EncodingName, base.DecoderFallback.GetType()), "chars");
        }

        private static string GetLocalizedEncodingNameResource(int codePage)
        {
            return codePage switch
            {
                37 => SR.Globalization_cp_37,
                437 => SR.Globalization_cp_437,
                500 => SR.Globalization_cp_500,
                708 => SR.Globalization_cp_708,
                720 => SR.Globalization_cp_720,
                737 => SR.Globalization_cp_737,
                775 => SR.Globalization_cp_775,
                850 => SR.Globalization_cp_850,
                852 => SR.Globalization_cp_852,
                855 => SR.Globalization_cp_855,
                857 => SR.Globalization_cp_857,
                858 => SR.Globalization_cp_858,
                860 => SR.Globalization_cp_860,
                861 => SR.Globalization_cp_861,
                862 => SR.Globalization_cp_862,
                863 => SR.Globalization_cp_863,
                864 => SR.Globalization_cp_864,
                865 => SR.Globalization_cp_865,
                866 => SR.Globalization_cp_866,
                869 => SR.Globalization_cp_869,
                870 => SR.Globalization_cp_870,
                874 => SR.Globalization_cp_874,
                875 => SR.Globalization_cp_875,
                932 => SR.Globalization_cp_932,
                936 => SR.Globalization_cp_936,
                949 => SR.Globalization_cp_949,
                950 => SR.Globalization_cp_950,
                1026 => SR.Globalization_cp_1026,
                1047 => SR.Globalization_cp_1047,
                1140 => SR.Globalization_cp_1140,
                1141 => SR.Globalization_cp_1141,
                1142 => SR.Globalization_cp_1142,
                1143 => SR.Globalization_cp_1143,
                1144 => SR.Globalization_cp_1144,
                1145 => SR.Globalization_cp_1145,
                1146 => SR.Globalization_cp_1146,
                1147 => SR.Globalization_cp_1147,
                1148 => SR.Globalization_cp_1148,
                1149 => SR.Globalization_cp_1149,
                1250 => SR.Globalization_cp_1250,
                1251 => SR.Globalization_cp_1251,
                1252 => SR.Globalization_cp_1252,
                1253 => SR.Globalization_cp_1253,
                1254 => SR.Globalization_cp_1254,
                1255 => SR.Globalization_cp_1255,
                1256 => SR.Globalization_cp_1256,
                1257 => SR.Globalization_cp_1257,
                1258 => SR.Globalization_cp_1258,
                1361 => SR.Globalization_cp_1361,
                10000 => SR.Globalization_cp_10000,
                10001 => SR.Globalization_cp_10001,
                10002 => SR.Globalization_cp_10002,
                10003 => SR.Globalization_cp_10003,
                10004 => SR.Globalization_cp_10004,
                10005 => SR.Globalization_cp_10005,
                10006 => SR.Globalization_cp_10006,
                10007 => SR.Globalization_cp_10007,
                10008 => SR.Globalization_cp_10008,
                10010 => SR.Globalization_cp_10010,
                10017 => SR.Globalization_cp_10017,
                10021 => SR.Globalization_cp_10021,
                10029 => SR.Globalization_cp_10029,
                10079 => SR.Globalization_cp_10079,
                10081 => SR.Globalization_cp_10081,
                10082 => SR.Globalization_cp_10082,
                20000 => SR.Globalization_cp_20000,
                20001 => SR.Globalization_cp_20001,
                20002 => SR.Globalization_cp_20002,
                20003 => SR.Globalization_cp_20003,
                20004 => SR.Globalization_cp_20004,
                20005 => SR.Globalization_cp_20005,
                20105 => SR.Globalization_cp_20105,
                20106 => SR.Globalization_cp_20106,
                20107 => SR.Globalization_cp_20107,
                20108 => SR.Globalization_cp_20108,
                20261 => SR.Globalization_cp_20261,
                20269 => SR.Globalization_cp_20269,
                20273 => SR.Globalization_cp_20273,
                20277 => SR.Globalization_cp_20277,
                20278 => SR.Globalization_cp_20278,
                20280 => SR.Globalization_cp_20280,
                20284 => SR.Globalization_cp_20284,
                20285 => SR.Globalization_cp_20285,
                20290 => SR.Globalization_cp_20290,
                20297 => SR.Globalization_cp_20297,
                20420 => SR.Globalization_cp_20420,
                20423 => SR.Globalization_cp_20423,
                20424 => SR.Globalization_cp_20424,
                20833 => SR.Globalization_cp_20833,
                20838 => SR.Globalization_cp_20838,
                20866 => SR.Globalization_cp_20866,
                20871 => SR.Globalization_cp_20871,
                20880 => SR.Globalization_cp_20880,
                20905 => SR.Globalization_cp_20905,
                20924 => SR.Globalization_cp_20924,
                20932 => SR.Globalization_cp_20932,
                20936 => SR.Globalization_cp_20936,
                20949 => SR.Globalization_cp_20949,
                21025 => SR.Globalization_cp_21025,
                21027 => SR.Globalization_cp_21027,
                21866 => SR.Globalization_cp_21866,
                28592 => SR.Globalization_cp_28592,
                28593 => SR.Globalization_cp_28593,
                28594 => SR.Globalization_cp_28594,
                28595 => SR.Globalization_cp_28595,
                28596 => SR.Globalization_cp_28596,
                28597 => SR.Globalization_cp_28597,
                28598 => SR.Globalization_cp_28598,
                28599 => SR.Globalization_cp_28599,
                28603 => SR.Globalization_cp_28603,
                28605 => SR.Globalization_cp_28605,
                29001 => SR.Globalization_cp_29001,
                38598 => SR.Globalization_cp_38598,
                50000 => SR.Globalization_cp_50000,
                50220 => SR.Globalization_cp_50220,
                50221 => SR.Globalization_cp_50221,
                50222 => SR.Globalization_cp_50222,
                50225 => SR.Globalization_cp_50225,
                50227 => SR.Globalization_cp_50227,
                50229 => SR.Globalization_cp_50229,
                50930 => SR.Globalization_cp_50930,
                50931 => SR.Globalization_cp_50931,
                50933 => SR.Globalization_cp_50933,
                50935 => SR.Globalization_cp_50935,
                50937 => SR.Globalization_cp_50937,
                50939 => SR.Globalization_cp_50939,
                51932 => SR.Globalization_cp_51932,
                51936 => SR.Globalization_cp_51936,
                51949 => SR.Globalization_cp_51949,
                52936 => SR.Globalization_cp_52936,
                54936 => SR.Globalization_cp_54936,
                57002 => SR.Globalization_cp_57002,
                57003 => SR.Globalization_cp_57003,
                57004 => SR.Globalization_cp_57004,
                57005 => SR.Globalization_cp_57005,
                57006 => SR.Globalization_cp_57006,
                57007 => SR.Globalization_cp_57007,
                57008 => SR.Globalization_cp_57008,
                57009 => SR.Globalization_cp_57009,
                57010 => SR.Globalization_cp_57010,
                57011 => SR.Globalization_cp_57011,
                _ => null,
            };
        }
    }
}
