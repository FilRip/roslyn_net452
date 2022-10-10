namespace System.Text
{
	internal class ISO2022Encoding : DBCSCodePageEncoding
	{
		internal enum ISO2022Modes
		{
			ModeHalfwidthKatakana = 0,
			ModeJIS0208 = 1,
			ModeKR = 5,
			ModeHZ = 6,
			ModeGB2312 = 7,
			ModeCNS11643_1 = 9,
			ModeCNS11643_2 = 10,
			ModeASCII = 11,
			ModeIncompleteEscape = -1,
			ModeInvalidEscape = -2,
			ModeNOOP = -3
		}

		internal class ISO2022Encoder : EncoderNLS
		{
			internal ISO2022Modes currentMode;

			internal ISO2022Modes shiftInOutMode;

			internal override bool HasState
			{
				get
				{
					if (charLeftOver == '\0')
					{
						return currentMode != ISO2022Modes.ModeASCII;
					}
					return true;
				}
			}

			internal ISO2022Encoder(EncodingNLS encoding)
				: base(encoding)
			{
			}

			public override void Reset()
			{
				currentMode = ISO2022Modes.ModeASCII;
				shiftInOutMode = ISO2022Modes.ModeASCII;
				charLeftOver = '\0';
				if (m_fallbackBuffer != null)
				{
					m_fallbackBuffer.Reset();
				}
			}
		}

		internal class ISO2022Decoder : DecoderNLS
		{
			internal byte[] bytesLeftOver;

			internal int bytesLeftOverCount;

			internal ISO2022Modes currentMode;

			internal ISO2022Modes shiftInOutMode;

			internal override bool HasState
			{
				get
				{
					if (bytesLeftOverCount == 0)
					{
						return currentMode != ISO2022Modes.ModeASCII;
					}
					return true;
				}
			}

			internal ISO2022Decoder(EncodingNLS encoding)
				: base(encoding)
			{
			}

			public override void Reset()
			{
				bytesLeftOverCount = 0;
				bytesLeftOver = new byte[4];
				currentMode = ISO2022Modes.ModeASCII;
				shiftInOutMode = ISO2022Modes.ModeASCII;
				if (m_fallbackBuffer != null)
				{
					m_fallbackBuffer.Reset();
				}
			}
		}

		private const byte SHIFT_OUT = 14;

		private const byte SHIFT_IN = 15;

		private const byte ESCAPE = 27;

		private const byte LEADBYTE_HALFWIDTH = 16;

		private static int[] s_tableBaseCodePages = new int[12]
		{
			932, 932, 932, 0, 0, 949, 936, 0, 0, 0,
			0, 0
		};

		private static ushort[] s_HalfToFullWidthKanaTable = new ushort[63]
		{
			41379, 41430, 41431, 41378, 41382, 42482, 42401, 42403, 42405, 42407,
			42409, 42467, 42469, 42471, 42435, 41404, 42402, 42404, 42406, 42408,
			42410, 42411, 42413, 42415, 42417, 42419, 42421, 42423, 42425, 42427,
			42429, 42431, 42433, 42436, 42438, 42440, 42442, 42443, 42444, 42445,
			42446, 42447, 42450, 42453, 42456, 42459, 42462, 42463, 42464, 42465,
			42466, 42468, 42470, 42472, 42473, 42474, 42475, 42476, 42477, 42479,
			42483, 41387, 41388
		};

		internal ISO2022Encoding(int codePage)
			: base(codePage)
		{
		}

		protected override bool CleanUpBytes(ref int bytes)
		{
			switch (CodePage)
			{
			case 50220:
			case 50221:
			case 50222:
				if (bytes >= 256)
				{
					if (bytes >= 64064 && bytes <= 64587)
					{
						if (bytes >= 64064 && bytes <= 64091)
						{
							if (bytes <= 64073)
							{
								bytes -= 2897;
							}
							else if (bytes >= 64074 && bytes <= 64083)
							{
								bytes -= 29430;
							}
							else if (bytes >= 64084 && bytes <= 64087)
							{
								bytes -= 2907;
							}
							else if (bytes == 64088)
							{
								bytes = 34698;
							}
							else if (bytes == 64089)
							{
								bytes = 34690;
							}
							else if (bytes == 64090)
							{
								bytes = 34692;
							}
							else if (bytes == 64091)
							{
								bytes = 34714;
							}
						}
						else if (bytes >= 64092 && bytes <= 64587)
						{
							byte b = (byte)bytes;
							if (b < 92)
							{
								bytes -= 3423;
							}
							else if (b >= 128 && b <= 155)
							{
								bytes -= 3357;
							}
							else
							{
								bytes -= 3356;
							}
						}
					}
					byte b2 = (byte)(bytes >> 8);
					byte b3 = (byte)bytes;
					b2 = (byte)(b2 - ((b2 > 159) ? 177 : 113));
					b2 = (byte)((b2 << 1) + 1);
					if (b3 > 158)
					{
						b3 = (byte)(b3 - 126);
						b2 = (byte)(b2 + 1);
					}
					else
					{
						if (b3 > 126)
						{
							b3 = (byte)(b3 - 1);
						}
						b3 = (byte)(b3 - 31);
					}
					bytes = (b2 << 8) | b3;
				}
				else
				{
					if (bytes >= 161 && bytes <= 223)
					{
						bytes += 3968;
					}
					if (bytes >= 129 && (bytes <= 159 || (bytes >= 224 && bytes <= 252)))
					{
						return false;
					}
				}
				break;
			case 50225:
				if (bytes >= 128 && bytes <= 255)
				{
					return false;
				}
				if (bytes >= 256 && ((bytes & 0xFF) < 161 || (bytes & 0xFF) == 255 || (bytes & 0xFF00) < 41216 || (bytes & 0xFF00) == 65280))
				{
					return false;
				}
				bytes &= 32639;
				break;
			case 52936:
				if (bytes >= 129 && bytes <= 254)
				{
					return false;
				}
				break;
			}
			return true;
		}

		public unsafe override int GetByteCount(char* chars, int count, EncoderNLS baseEncoder)
		{
			return GetBytes(chars, count, null, 0, baseEncoder);
		}

		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount, EncoderNLS baseEncoder)
		{
			ISO2022Encoder encoder = (ISO2022Encoder)baseEncoder;
			int result = 0;
			switch (CodePage)
			{
			case 50220:
			case 50221:
			case 50222:
				result = GetBytesCP5022xJP(chars, charCount, bytes, byteCount, encoder);
				break;
			case 50225:
				result = GetBytesCP50225KR(chars, charCount, bytes, byteCount, encoder);
				break;
			case 52936:
				result = GetBytesCP52936(chars, charCount, bytes, byteCount, encoder);
				break;
			}
			return result;
		}

		public unsafe override int GetCharCount(byte* bytes, int count, DecoderNLS baseDecoder)
		{
			return GetChars(bytes, count, null, 0, baseDecoder);
		}

		public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount, DecoderNLS baseDecoder)
		{
			ISO2022Decoder decoder = (ISO2022Decoder)baseDecoder;
			int result = 0;
			switch (CodePage)
			{
			case 50220:
			case 50221:
			case 50222:
				result = GetCharsCP5022xJP(bytes, byteCount, chars, charCount, decoder);
				break;
			case 50225:
				result = GetCharsCP50225KR(bytes, byteCount, chars, charCount, decoder);
				break;
			case 52936:
				result = GetCharsCP52936(bytes, byteCount, chars, charCount, decoder);
				break;
			}
			return result;
		}

		// ISO 2022 Code pages for JP.
		//  50220 - No halfwidth Katakana, convert to full width
		//  50221 - Use escape sequence for half width Katakana
		//  50222 - Use shift-in/shift-out for half width Katakana
		//
		// These are the JIS code pages, superset of ISO-2022 / ISO-2022-JP-1
		//  0E          Shift Out (following bytes are Katakana)
		//  0F          Shift In  (back to "normal" behavior)
		//  21-7E       Byte ranges (1 or 2 bytes)
		//  <ESC> $ @   To Double Byte 0208 Mode (actually older code page, but subset of 0208)
		//  <ESC> $ B   To Double Byte 0208 Mode (duplicate)
		//  <ESC> $ ( D To Double Byte 0212 Mode (previously we misinterpreted this)
		//  <ESC> $ I   To half width Katakana
		//  <ESC> ( J   To JIS-Roman
		//  <ESC> ( H   To JIS-Roman (swedish character set)
		//  <ESC> ( B   To ASCII
		//  <ESC> & @   Alternate lead in to <ESC> $ B so just ignore it.
		//
		// So in Katakana mode we add 0x8e as a lead byte and use CP 20932 to convert it
		// In ASCII mode we just spit out the single byte.
		// In Roman mode we should change 0x5c (\) -> Yen sign and 0x7e (~) to Overline, however
		//      we didn't in mLang, otherwise roman is like ASCII.
		// In 0208 double byte mode we have to |= with 0x8080 and use CP 20932 to convert it.
		// In 0212 double byte mode we have to |= with 0x8000 and use CP 20932 to convert it.
		//
		// Note that JIS Shift In/Shift Out is different than the other ISO2022 encodings.  For JIS
		// Shift out always shifts to half-width Katakana.  Chinese encodings use designator sequences
		// instead of escape sequences and shift out to the designated sequence or back in to ASCII.
		//
		// When decoding JIS 0208, MLang used a '*' (0x2a) character in JIS 0208 mode to map the trailing byte
		// to halfwidth katakana.  I found no description of that behavior, however that block of 0208 is
		// undefined, so we maintain that behavior when decoding.  We will never generate characters using
		// that technique, but the decoder will process them.
		//
		private unsafe int GetBytesCP5022xJP(char* chars, int charCount,
												  byte* bytes, int byteCount, ISO2022Encoder? encoder)
		{
			// prepare our helpers
			EncodingByteBuffer buffer = new EncodingByteBuffer(this, encoder, bytes, byteCount, chars, charCount);

			// Get our mode
			ISO2022Modes currentMode = ISO2022Modes.ModeASCII;      // Mode
			ISO2022Modes shiftInMode = ISO2022Modes.ModeASCII;      // Mode that shift in will go back to (only used by CP 50222)

			// Check our encoder
			if (encoder != null)
			{
				char charLeftOver = encoder.charLeftOver;

				currentMode = encoder.currentMode;
				shiftInMode = encoder.shiftInOutMode;

				// We may have a left over character from last time, try and process it.
				if (charLeftOver > 0)
				{
					// It has to be a high surrogate, which we don't support, so it has to be a fallback
					buffer.Fallback(charLeftOver);
				}
			}

			while (buffer.MoreData)
			{
				// Get our char
				char ch = buffer.GetNextChar();

				// Get our bytes
				ushort iBytes = mapUnicodeToBytes[ch];

			StartConvert:
				// Check for halfwidth bytes
				byte bLeadByte = (byte)(iBytes >> 8);
				byte bTrailByte = (byte)(iBytes & 0xff);

				if (bLeadByte == LEADBYTE_HALFWIDTH)
				{
					// Its Halfwidth Katakana
					if (CodePage == 50220)
					{
						// CodePage 50220 doesn't use halfwidth Katakana, convert to fullwidth
						// See if its out of range, fallback if so, throws if recursive fallback
						if (bTrailByte < 0x21 || bTrailByte >= 0x21 + s_HalfToFullWidthKanaTable.Length)
						{
							buffer.Fallback(ch);
							continue;
						}

						// Get the full width katakana char to use.
						iBytes = unchecked((ushort)(s_HalfToFullWidthKanaTable[bTrailByte - 0x21] & 0x7F7F));

						// May have to do all sorts of fun stuff for mode, go back to start convert
						goto StartConvert;
					}

					// Can use halfwidth Katakana, make sure we're in right mode

					// Make sure we're in right mode
					if (currentMode != ISO2022Modes.ModeHalfwidthKatakana)
					{
						// 50222 or 50221, either shift in/out or escape to get to Katakana mode
						if (CodePage == 50222)
						{
							// Shift Out
							if (!buffer.AddByte(SHIFT_OUT))
								break;  // convert out of space, stop

							// Don't change modes until after AddByte in case it fails for convert
							// We get to shift out to Katakana, make sure we'll go back to the right mode
							// (This ends up always being ASCII)
							shiftInMode = currentMode;
							currentMode = ISO2022Modes.ModeHalfwidthKatakana;
						}
						else
						{
							// Add our escape sequence
							if (!buffer.AddByte(ESCAPE, unchecked((byte)'('), unchecked((byte)'I')))
								break;  // convert out of space, stop

							currentMode = ISO2022Modes.ModeHalfwidthKatakana;
						}
					}

					// We know we're in Katakana mode now, so add it.
					// Go ahead and add the Katakana byte.  Our table tail bytes are 0x80 too big.
					if (!buffer.AddByte(unchecked((byte)(bTrailByte & 0x7F))))
						break;  // convert out of space, stop

					// Done with this one
					continue;
				}
				else if (bLeadByte != 0)
				{
					//
					//  It's a double byte character.
					//

					// If we're CP 50222 we may have to shift in from Katakana mode first
					if (CodePage == 50222 && currentMode == ISO2022Modes.ModeHalfwidthKatakana)
					{
						// Shift In
						if (!buffer.AddByte(SHIFT_IN))
							break;    // convert out of space, stop

						// Need to shift in from katakana.  (Still might not be right, but won't be shifted out anyway)
						currentMode = shiftInMode;
					}

					// Make sure we're in the right mode (JIS 0208 or JIS 0212)
					// Note: Right now we don't use JIS 0212.  Also this table would be wrong

					// Its JIS extension 0208
					if (currentMode != ISO2022Modes.ModeJIS0208)
					{
						// Escape sequence, we can fail after this, mode will be correct for convert
						if (!buffer.AddByte(ESCAPE, unchecked((byte)'$'), unchecked((byte)'B')))
							break;  // Convert out of space, stop

						currentMode = ISO2022Modes.ModeJIS0208;
					}

					// Add our double bytes
					if (!buffer.AddByte(unchecked((byte)(bLeadByte)), unchecked((byte)(bTrailByte))))
						break; // Convert out of space, stop
					continue;
				}
				else if (iBytes != 0 || ch == 0)
				{
					// Single byte Char
					// If we're CP 50222 we may have to shift in from Katakana mode first
					if (CodePage == 50222 && currentMode == ISO2022Modes.ModeHalfwidthKatakana)
					{
						// Shift IN
						if (!buffer.AddByte(SHIFT_IN))
							break; // convert ran out of room

						// Need to shift in from katakana.  (Still might not be right, but won't be shifted out anyway)
						currentMode = shiftInMode;
					}

					// Its a single byte character, switch to ASCII if we have to
					if (currentMode != ISO2022Modes.ModeASCII)
					{
						if (!buffer.AddByte(ESCAPE, unchecked((byte)'('), unchecked((byte)'B')))
							break; // convert ran out of room

						currentMode = ISO2022Modes.ModeASCII;
					}

					// Add the ASCII char
					if (!buffer.AddByte(bTrailByte))
						break; // convert had no room left
					continue;
				}

				// Its unknown, do fallback, throws if recursive (knows because we called InternalGetNextChar)
				buffer.Fallback(ch);
			}

			// Switch back to ASCII if MustFlush or no encoder
			if (currentMode != ISO2022Modes.ModeASCII &&
				(encoder == null || encoder.MustFlush))
			{
				// If we're CP 50222 we may have to shift in from Katakana mode first
				if (CodePage == 50222 && currentMode == ISO2022Modes.ModeHalfwidthKatakana)
				{
					// Shift IN, only shift mode if necessary.
					if (buffer.AddByte(SHIFT_IN))
						// Need to shift in from katakana.  (Still might not be right, but won't be shifted out anyway)
						currentMode = shiftInMode;
					else
						// If not successful, convert will maintain state for next time, also
						// AddByte will have decremented our char count, however we need it to remain the same
						buffer.GetNextChar();
				}

				// switch back to ASCII to finish neatly
				if (currentMode != ISO2022Modes.ModeASCII &&
					(CodePage != 50222 || currentMode != ISO2022Modes.ModeHalfwidthKatakana))
				{
					// only shift if it was successful
					if (buffer.AddByte(ESCAPE, unchecked((byte)'('), unchecked((byte)'B')))
						currentMode = ISO2022Modes.ModeASCII;
					else
						// If not successful, convert will maintain state for next time, also
						// AddByte will have decremented our char count, however we need it to remain the same
						buffer.GetNextChar();
				}
			}

			// Remember our encoder state
			if (bytes != null && encoder != null)
			{
				// This is ASCII if we had to flush
				encoder.currentMode = currentMode;
				encoder.shiftInOutMode = shiftInMode;

				if (!buffer.fallbackBufferHelper.bUsedEncoder)
				{
					encoder.charLeftOver = (char)0;
				}

				encoder.m_charsUsed = buffer.CharsUsed;
			}

			// Return our length
			return buffer.Count;
		}

		private unsafe int GetBytesCP50225KR(char* chars, int charCount, byte* bytes, int byteCount, ISO2022Encoder encoder)
		{
			EncodingByteBuffer encodingByteBuffer = new EncodingByteBuffer(this, encoder, bytes, byteCount, chars, charCount);
			ISO2022Modes iSO2022Modes = ISO2022Modes.ModeASCII;
			ISO2022Modes iSO2022Modes2 = ISO2022Modes.ModeASCII;
			if (encoder != null)
			{
				char charLeftOver = encoder.charLeftOver;
				iSO2022Modes = encoder.currentMode;
				iSO2022Modes2 = encoder.shiftInOutMode;
				if (charLeftOver > '\0')
				{
					encodingByteBuffer.Fallback(charLeftOver);
				}
			}
			while (encodingByteBuffer.MoreData)
			{
				char nextChar = encodingByteBuffer.GetNextChar();
				ushort num = mapUnicodeToBytes[(int)nextChar];
				byte b = (byte)(num >> 8);
				byte b2 = (byte)(num & 0xFFu);
				if (b != 0)
				{
					if (iSO2022Modes2 != ISO2022Modes.ModeKR)
					{
						if (!encodingByteBuffer.AddByte((byte)27, (byte)36, (byte)41, (byte)67))
						{
							break;
						}
						iSO2022Modes2 = ISO2022Modes.ModeKR;
					}
					if (iSO2022Modes != ISO2022Modes.ModeKR)
					{
						if (!encodingByteBuffer.AddByte(14))
						{
							break;
						}
						iSO2022Modes = ISO2022Modes.ModeKR;
					}
					if (!encodingByteBuffer.AddByte(b, b2))
					{
						break;
					}
				}
				else if (num != 0 || nextChar == '\0')
				{
					if (iSO2022Modes != ISO2022Modes.ModeASCII)
					{
						if (!encodingByteBuffer.AddByte(15))
						{
							break;
						}
						iSO2022Modes = ISO2022Modes.ModeASCII;
					}
					if (!encodingByteBuffer.AddByte(b2))
					{
						break;
					}
				}
				else
				{
					encodingByteBuffer.Fallback(nextChar);
				}
			}
			if (iSO2022Modes != ISO2022Modes.ModeASCII && (encoder == null || encoder.MustFlush))
			{
				if (encodingByteBuffer.AddByte(15))
				{
					iSO2022Modes = ISO2022Modes.ModeASCII;
				}
				else
				{
					encodingByteBuffer.GetNextChar();
				}
			}
			if (bytes != null && encoder != null)
			{
				if (!encodingByteBuffer.fallbackBufferHelper.bUsedEncoder)
				{
					encoder.charLeftOver = '\0';
				}
				encoder.currentMode = iSO2022Modes;
				if (!encoder.MustFlush || encoder.charLeftOver != 0)
				{
					encoder.shiftInOutMode = iSO2022Modes2;
				}
				else
				{
					encoder.shiftInOutMode = ISO2022Modes.ModeASCII;
				}
				encoder.m_charsUsed = encodingByteBuffer.CharsUsed;
			}
			return encodingByteBuffer.Count;
		}

		private unsafe int GetBytesCP52936(char* chars, int charCount, byte* bytes, int byteCount, ISO2022Encoder encoder)
		{
			EncodingByteBuffer encodingByteBuffer = new EncodingByteBuffer(this, encoder, bytes, byteCount, chars, charCount);
			ISO2022Modes iSO2022Modes = ISO2022Modes.ModeASCII;
			if (encoder != null)
			{
				char charLeftOver = encoder.charLeftOver;
				iSO2022Modes = encoder.currentMode;
				if (charLeftOver > '\0')
				{
					encodingByteBuffer.Fallback(charLeftOver);
				}
			}
			while (encodingByteBuffer.MoreData)
			{
				char nextChar = encodingByteBuffer.GetNextChar();
				ushort num = mapUnicodeToBytes[(int)nextChar];
				if (num == 0 && nextChar != 0)
				{
					encodingByteBuffer.Fallback(nextChar);
					continue;
				}
				byte b = (byte)(num >> 8);
				byte b2 = (byte)(num & 0xFFu);
				if ((b != 0 && (b < 161 || b > 247 || b2 < 161 || b2 > 254)) || (b == 0 && b2 > 128 && b2 != byte.MaxValue))
				{
					encodingByteBuffer.Fallback(nextChar);
					continue;
				}
				if (b != 0)
				{
					if (iSO2022Modes != ISO2022Modes.ModeHZ)
					{
						if (!encodingByteBuffer.AddByte(126, 123, 2))
						{
							break;
						}
						iSO2022Modes = ISO2022Modes.ModeHZ;
					}
					if (encodingByteBuffer.AddByte((byte)(b & 0x7Fu), (byte)(b2 & 0x7Fu)))
					{
						continue;
					}
					break;
				}
				if (iSO2022Modes != ISO2022Modes.ModeASCII)
				{
					if (!encodingByteBuffer.AddByte(126, 125, (b2 != 126) ? 1 : 2))
					{
						break;
					}
					iSO2022Modes = ISO2022Modes.ModeASCII;
				}
				if ((b2 == 126 && !encodingByteBuffer.AddByte(126, 1)) || !encodingByteBuffer.AddByte(b2))
				{
					break;
				}
			}
			if (iSO2022Modes != ISO2022Modes.ModeASCII && (encoder == null || encoder.MustFlush))
			{
				if (encodingByteBuffer.AddByte((byte)126, (byte)125))
				{
					iSO2022Modes = ISO2022Modes.ModeASCII;
				}
				else
				{
					encodingByteBuffer.GetNextChar();
				}
			}
			if (encoder != null && bytes != null)
			{
				encoder.currentMode = iSO2022Modes;
				if (!encodingByteBuffer.fallbackBufferHelper.bUsedEncoder)
				{
					encoder.charLeftOver = '\0';
				}
				encoder.m_charsUsed = encodingByteBuffer.CharsUsed;
			}
			return encodingByteBuffer.Count;
		}

		private unsafe int GetCharsCP5022xJP(byte* bytes, int byteCount, char* chars, int charCount, ISO2022Decoder decoder)
		{
			EncodingCharBuffer encodingCharBuffer = new EncodingCharBuffer(this, decoder, chars, charCount, bytes, byteCount);
			ISO2022Modes iSO2022Modes = ISO2022Modes.ModeASCII;
			ISO2022Modes iSO2022Modes2 = ISO2022Modes.ModeASCII;
			byte[] bytes2 = new byte[4];
			int count = 0;
			if (decoder != null)
			{
				iSO2022Modes = decoder.currentMode;
				iSO2022Modes2 = decoder.shiftInOutMode;
				count = decoder.bytesLeftOverCount;
				for (int i = 0; i < count; i++)
				{
					bytes2[i] = decoder.bytesLeftOver[i];
				}
			}
			while (encodingCharBuffer.MoreData || count > 0)
			{
				byte b;
				if (count > 0)
				{
					if (bytes2[0] == 27)
					{
						if (!encodingCharBuffer.MoreData)
						{
							if (decoder != null && !decoder.MustFlush)
							{
								break;
							}
						}
						else
						{
							bytes2[count++] = encodingCharBuffer.GetNextByte();
							ISO2022Modes iSO2022Modes3 = CheckEscapeSequenceJP(bytes2, count);
							switch (iSO2022Modes3)
							{
							default:
								count = 0;
								iSO2022Modes = (iSO2022Modes2 = iSO2022Modes3);
								continue;
							case ISO2022Modes.ModeInvalidEscape:
								break;
							case ISO2022Modes.ModeIncompleteEscape:
								continue;
							}
						}
					}
					b = DecrementEscapeBytes(ref bytes2, ref count);
				}
				else
				{
					b = encodingCharBuffer.GetNextByte();
					if (b == 27)
					{
						if (count == 0)
						{
							bytes2[0] = b;
							count = 1;
							continue;
						}
						encodingCharBuffer.AdjustBytes(-1);
					}
				}
				switch (b)
				{
				case 14:
					iSO2022Modes2 = iSO2022Modes;
					iSO2022Modes = ISO2022Modes.ModeHalfwidthKatakana;
					continue;
				case 15:
					iSO2022Modes = iSO2022Modes2;
					continue;
				}
				ushort num = b;
				bool flag = false;
				if (iSO2022Modes == ISO2022Modes.ModeJIS0208)
				{
					if (count > 0)
					{
						if (bytes2[0] != 27)
						{
							num = (ushort)(num << 8);
							num = (ushort)(num | DecrementEscapeBytes(ref bytes2, ref count));
							flag = true;
						}
					}
					else
					{
						if (!encodingCharBuffer.MoreData)
						{
							if (decoder == null || decoder.MustFlush)
							{
								encodingCharBuffer.Fallback(b);
							}
							else if (chars != null)
							{
								bytes2[0] = b;
								count = 1;
							}
							break;
						}
						num = (ushort)(num << 8);
						num = (ushort)(num | encodingCharBuffer.GetNextByte());
						flag = true;
					}
					if (flag && (num & 0xFF00) == 10752)
					{
						num = (ushort)(num & 0xFFu);
						num = (ushort)(num | 0x1000u);
					}
				}
				else if (num >= 161 && num <= 223)
				{
					num = (ushort)(num | 0x1000u);
					num = (ushort)(num & 0xFF7Fu);
				}
				else if (iSO2022Modes == ISO2022Modes.ModeHalfwidthKatakana)
				{
					num = (ushort)(num | 0x1000u);
				}
				char c = mapBytesToUnicode[(int)num];
				if (c == '\0' && num != 0)
				{
					if (flag)
					{
						if (!encodingCharBuffer.Fallback((byte)(num >> 8), (byte)num))
						{
							break;
						}
					}
					else if (!encodingCharBuffer.Fallback(b))
					{
						break;
					}
				}
				else if (!encodingCharBuffer.AddChar(c, (!flag) ? 1 : 2))
				{
					break;
				}
			}
			if (chars != null && decoder != null)
			{
				if (!decoder.MustFlush || count != 0)
				{
					decoder.currentMode = iSO2022Modes;
					decoder.shiftInOutMode = iSO2022Modes2;
					decoder.bytesLeftOverCount = count;
					decoder.bytesLeftOver = bytes2;
				}
				else
				{
					decoder.currentMode = ISO2022Modes.ModeASCII;
					decoder.shiftInOutMode = ISO2022Modes.ModeASCII;
					decoder.bytesLeftOverCount = 0;
				}
				decoder.m_bytesUsed = encodingCharBuffer.BytesUsed;
			}
			return encodingCharBuffer.Count;
		}

		private ISO2022Modes CheckEscapeSequenceJP(byte[] bytes, int escapeCount)
		{
			if (bytes[0] != 27)
			{
				return ISO2022Modes.ModeInvalidEscape;
			}
			if (escapeCount < 3)
			{
				return ISO2022Modes.ModeIncompleteEscape;
			}
			if (bytes[1] == 40)
			{
				if (bytes[2] == 66)
				{
					return ISO2022Modes.ModeASCII;
				}
				if (bytes[2] == 72)
				{
					return ISO2022Modes.ModeASCII;
				}
				if (bytes[2] == 74)
				{
					return ISO2022Modes.ModeASCII;
				}
				if (bytes[2] == 73)
				{
					return ISO2022Modes.ModeHalfwidthKatakana;
				}
			}
			else if (bytes[1] == 36)
			{
				if (bytes[2] == 64 || bytes[2] == 66)
				{
					return ISO2022Modes.ModeJIS0208;
				}
				if (escapeCount < 4)
				{
					return ISO2022Modes.ModeIncompleteEscape;
				}
				if (bytes[2] == 40 && bytes[3] == 68)
				{
					return ISO2022Modes.ModeJIS0208;
				}
			}
			else if (bytes[1] == 38 && bytes[2] == 64)
			{
				return ISO2022Modes.ModeNOOP;
			}
			return ISO2022Modes.ModeInvalidEscape;
		}

		private byte DecrementEscapeBytes(ref byte[] bytes, ref int count)
		{
			count--;
			byte result = bytes[0];
			for (int i = 0; i < count; i++)
			{
				bytes[i] = bytes[i + 1];
			}
			bytes[count] = 0;
			return result;
		}

		private unsafe int GetCharsCP50225KR(byte* bytes, int byteCount, char* chars, int charCount, ISO2022Decoder decoder)
		{
			EncodingCharBuffer encodingCharBuffer = new EncodingCharBuffer(this, decoder, chars, charCount, bytes, byteCount);
			ISO2022Modes iSO2022Modes = ISO2022Modes.ModeASCII;
			byte[] bytes2 = new byte[4];
			int count = 0;
			if (decoder != null)
			{
				iSO2022Modes = decoder.currentMode;
				count = decoder.bytesLeftOverCount;
				for (int i = 0; i < count; i++)
				{
					bytes2[i] = decoder.bytesLeftOver[i];
				}
			}
			while (encodingCharBuffer.MoreData || count > 0)
			{
				byte b;
				if (count > 0)
				{
					if (bytes2[0] == 27)
					{
						if (!encodingCharBuffer.MoreData)
						{
							if (decoder != null && !decoder.MustFlush)
							{
								break;
							}
						}
						else
						{
							bytes2[count++] = encodingCharBuffer.GetNextByte();
							switch (CheckEscapeSequenceKR(bytes2, count))
							{
							default:
								count = 0;
								continue;
							case ISO2022Modes.ModeInvalidEscape:
								break;
							case ISO2022Modes.ModeIncompleteEscape:
								continue;
							}
						}
					}
					b = DecrementEscapeBytes(ref bytes2, ref count);
				}
				else
				{
					b = encodingCharBuffer.GetNextByte();
					if (b == 27)
					{
						if (count == 0)
						{
							bytes2[0] = b;
							count = 1;
							continue;
						}
						encodingCharBuffer.AdjustBytes(-1);
					}
				}
				switch (b)
				{
				case 14:
					iSO2022Modes = ISO2022Modes.ModeKR;
					continue;
				case 15:
					iSO2022Modes = ISO2022Modes.ModeASCII;
					continue;
				}
				ushort num = b;
				bool flag = false;
				if (iSO2022Modes == ISO2022Modes.ModeKR && b != 32 && b != 9 && b != 10)
				{
					if (count > 0)
					{
						if (bytes2[0] != 27)
						{
							num = (ushort)(num << 8);
							num = (ushort)(num | DecrementEscapeBytes(ref bytes2, ref count));
							flag = true;
						}
					}
					else
					{
						if (!encodingCharBuffer.MoreData)
						{
							if (decoder == null || decoder.MustFlush)
							{
								encodingCharBuffer.Fallback(b);
							}
							else if (chars != null)
							{
								bytes2[0] = b;
								count = 1;
							}
							break;
						}
						num = (ushort)(num << 8);
						num = (ushort)(num | encodingCharBuffer.GetNextByte());
						flag = true;
					}
				}
				char c = mapBytesToUnicode[(int)num];
				if (c == '\0' && num != 0)
				{
					if (flag)
					{
						if (!encodingCharBuffer.Fallback((byte)(num >> 8), (byte)num))
						{
							break;
						}
					}
					else if (!encodingCharBuffer.Fallback(b))
					{
						break;
					}
				}
				else if (!encodingCharBuffer.AddChar(c, (!flag) ? 1 : 2))
				{
					break;
				}
			}
			if (chars != null && decoder != null)
			{
				if (!decoder.MustFlush || count != 0)
				{
					decoder.currentMode = iSO2022Modes;
					decoder.bytesLeftOverCount = count;
					decoder.bytesLeftOver = bytes2;
				}
				else
				{
					decoder.currentMode = ISO2022Modes.ModeASCII;
					decoder.shiftInOutMode = ISO2022Modes.ModeASCII;
					decoder.bytesLeftOverCount = 0;
				}
				decoder.m_bytesUsed = encodingCharBuffer.BytesUsed;
			}
			return encodingCharBuffer.Count;
		}

		private ISO2022Modes CheckEscapeSequenceKR(byte[] bytes, int escapeCount)
		{
			if (bytes[0] != 27)
			{
				return ISO2022Modes.ModeInvalidEscape;
			}
			if (escapeCount < 4)
			{
				return ISO2022Modes.ModeIncompleteEscape;
			}
			if (bytes[1] == 36 && bytes[2] == 41 && bytes[3] == 67)
			{
				return ISO2022Modes.ModeKR;
			}
			return ISO2022Modes.ModeInvalidEscape;
		}

		private unsafe int GetCharsCP52936(byte* bytes, int byteCount, char* chars, int charCount, ISO2022Decoder decoder)
		{
			EncodingCharBuffer encodingCharBuffer = new EncodingCharBuffer(this, decoder, chars, charCount, bytes, byteCount);
			ISO2022Modes iSO2022Modes = ISO2022Modes.ModeASCII;
			int num = -1;
			bool flag = false;
			if (decoder != null)
			{
				iSO2022Modes = decoder.currentMode;
				if (decoder.bytesLeftOverCount != 0)
				{
					num = decoder.bytesLeftOver[0];
				}
			}
			while (encodingCharBuffer.MoreData || num >= 0)
			{
				byte b;
				if (num >= 0)
				{
					b = (byte)num;
					num = -1;
				}
				else
				{
					b = encodingCharBuffer.GetNextByte();
				}
				if (b == 126)
				{
					if (!encodingCharBuffer.MoreData)
					{
						if (decoder == null || decoder.MustFlush)
						{
							encodingCharBuffer.Fallback(b);
							break;
						}
						decoder?.ClearMustFlush();
						if (chars != null)
						{
							decoder.bytesLeftOverCount = 1;
							decoder.bytesLeftOver[0] = 126;
							flag = true;
						}
						break;
					}
					b = encodingCharBuffer.GetNextByte();
					if (b == 126 && iSO2022Modes == ISO2022Modes.ModeASCII)
					{
						if (!encodingCharBuffer.AddChar((char)b, 2))
						{
							break;
						}
						continue;
					}
					if (b == 123)
					{
						iSO2022Modes = ISO2022Modes.ModeHZ;
						continue;
					}
					if (b == 125)
					{
						iSO2022Modes = ISO2022Modes.ModeASCII;
						continue;
					}
					if (b == 10)
					{
						continue;
					}
					encodingCharBuffer.AdjustBytes(-1);
					b = 126;
				}
				if (iSO2022Modes != ISO2022Modes.ModeASCII && b >= 32)
				{
					if (!encodingCharBuffer.MoreData)
					{
						if (decoder == null || decoder.MustFlush)
						{
							encodingCharBuffer.Fallback(b);
							break;
						}
						decoder?.ClearMustFlush();
						if (chars != null)
						{
							decoder.bytesLeftOverCount = 1;
							decoder.bytesLeftOver[0] = b;
							flag = true;
						}
						break;
					}
					byte nextByte = encodingCharBuffer.GetNextByte();
					ushort num2 = (ushort)((b << 8) | nextByte);
					char c;
					if (b == 32 && nextByte != 0)
					{
						c = (char)nextByte;
					}
					else
					{
						if ((b < 33 || b > 119 || nextByte < 33 || nextByte > 126) && (b < 161 || b > 247 || nextByte < 161 || nextByte > 254))
						{
							if (nextByte != 32 || 33 > b || b > 125)
							{
								if (!encodingCharBuffer.Fallback((byte)(num2 >> 8), (byte)num2))
								{
									break;
								}
								continue;
							}
							num2 = 8481;
						}
						num2 = (ushort)(num2 | 0x8080u);
						c = mapBytesToUnicode[(int)num2];
					}
					if (c == '\0' && num2 != 0)
					{
						if (!encodingCharBuffer.Fallback((byte)(num2 >> 8), (byte)num2))
						{
							break;
						}
					}
					else if (!encodingCharBuffer.AddChar(c, 2))
					{
						break;
					}
					continue;
				}
				char c2 = mapBytesToUnicode[(int)b];
				if ((c2 == '\0' || c2 == '\0') && b != 0)
				{
					if (!encodingCharBuffer.Fallback(b))
					{
						break;
					}
				}
				else if (!encodingCharBuffer.AddChar(c2))
				{
					break;
				}
			}
			if (chars != null && decoder != null)
			{
				if (!flag)
				{
					decoder.bytesLeftOverCount = 0;
				}
				if (decoder.MustFlush && decoder.bytesLeftOverCount == 0)
				{
					decoder.currentMode = ISO2022Modes.ModeASCII;
				}
				else
				{
					decoder.currentMode = iSO2022Modes;
				}
				decoder.m_bytesUsed = encodingCharBuffer.BytesUsed;
			}
			return encodingCharBuffer.Count;
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", SR.ArgumentOutOfRange_NeedNonNegNum);
			}
			long num = (long)charCount + 1L;
			if (base.EncoderFallback.MaxCharCount > 1)
			{
				num *= base.EncoderFallback.MaxCharCount;
			}
			int num2 = 2;
			int num3 = 0;
			int num4 = 0;
			switch (CodePage)
			{
			case 50220:
			case 50221:
				num2 = 5;
				num4 = 3;
				break;
			case 50222:
				num2 = 5;
				num4 = 4;
				break;
			case 50225:
				num2 = 3;
				num3 = 4;
				num4 = 1;
				break;
			case 52936:
				num2 = 4;
				num4 = 2;
				break;
			}
			num *= num2;
			num += num3 + num4;
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
			int num = 1;
			int num2 = 1;
			switch (CodePage)
			{
			case 50220:
			case 50221:
			case 50222:
			case 50225:
				num = 1;
				num2 = 3;
				break;
			case 52936:
				num = 1;
				num2 = 1;
				break;
			}
			long num3 = (long)byteCount * (long)num + num2;
			if (base.DecoderFallback.MaxCharCount > 1)
			{
				num3 *= base.DecoderFallback.MaxCharCount;
			}
			if (num3 > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("byteCount", SR.ArgumentOutOfRange_GetCharCountOverflow);
			}
			return (int)num3;
		}

		public override Encoder GetEncoder()
		{
			return new ISO2022Encoder(this);
		}

		public override Decoder GetDecoder()
		{
			return new ISO2022Decoder(this);
		}
	}
}
