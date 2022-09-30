using System;
using System.IO;
using System.Text;

using Roslyn.Utilities;

using SystemExtensions;

#nullable enable

namespace Microsoft.CodeAnalysis.Text
{
    internal static class EncodedStringText
    {
        internal static class TestAccessor
        {
            internal static SourceText Create(Stream stream, Lazy<Encoding> getEncoding, Encoding defaultEncoding, SourceHashAlgorithm checksumAlgorithm, bool canBeEmbedded)
            {
                return EncodedStringText.Create(stream, getEncoding, defaultEncoding, checksumAlgorithm, canBeEmbedded);
            }

            internal static SourceText Decode(Stream data, Encoding encoding, SourceHashAlgorithm checksumAlgorithm, bool throwIfBinaryDetected, bool canBeEmbedded)
            {
                return EncodedStringText.Decode(data, encoding, checksumAlgorithm, throwIfBinaryDetected, canBeEmbedded);
            }
        }

        private const int LargeObjectHeapLimitInChars = 40960;

        private static readonly Encoding s_utf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private static readonly Lazy<Encoding> s_fallbackEncoding = new Lazy<Encoding>(CreateFallbackEncoding);

        internal static Encoding CreateFallbackEncoding()
        {
            try
            {
                if (CodePagesEncodingProvider.Instance != null)
                {
                    ExtensionsEncoding.Encoding_RegisterProvider(CodePagesEncodingProvider.Instance);
                }
                return Encoding.GetEncoding(0) ?? Encoding.GetEncoding(1252);
            }
            catch (NotSupportedException)
            {
                return Encoding.GetEncoding("Latin1");
            }
        }

        internal static SourceText Create(Stream stream, Encoding? defaultEncoding = null, SourceHashAlgorithm checksumAlgorithm = SourceHashAlgorithm.Sha1, bool canBeEmbedded = false)
        {
            return Create(stream, s_fallbackEncoding, defaultEncoding, checksumAlgorithm, canBeEmbedded);
        }

        internal static SourceText Create(Stream stream, Lazy<Encoding> getEncoding, Encoding? defaultEncoding = null, SourceHashAlgorithm checksumAlgorithm = SourceHashAlgorithm.Sha1, bool canBeEmbedded = false)
        {
            bool flag = defaultEncoding == null;
            if (flag)
            {
                try
                {
                    return Decode(stream, s_utf8Encoding, checksumAlgorithm, throwIfBinaryDetected: false, canBeEmbedded);
                }
                catch (DecoderFallbackException)
                {
                }
            }
            try
            {
                return Decode(stream, defaultEncoding ?? getEncoding.Value, checksumAlgorithm, flag, canBeEmbedded);
            }
            catch (DecoderFallbackException ex2)
            {
                throw new InvalidDataException(ex2.Message);
            }
        }

        /// <summary>
        /// Try to create a <see cref="SourceText"/> from the given stream using the given encoding.
        /// </summary>
        /// <param name="data">The input stream containing the encoded text. The stream will not be closed.</param>
        /// <param name="encoding">The expected encoding of the stream. The actual encoding used may be different if byte order marks are detected.</param>
        /// <param name="checksumAlgorithm">The checksum algorithm to use.</param>
        /// <param name="throwIfBinaryDetected">Throw <see cref="InvalidDataException"/> if binary (non-text) data is detected.</param>
        /// <returns>The <see cref="SourceText"/> decoded from the stream.</returns>
        /// <exception cref="DecoderFallbackException">The decoder was unable to decode the stream with the given encoding.</exception>
        /// <remarks>
        /// internal for unit testing
        /// </remarks>
        internal static SourceText Decode(Stream data, Encoding encoding, SourceHashAlgorithm checksumAlgorithm, bool throwIfBinaryDetected = false, bool canBeEmbedded = false)
        {
            data.Seek(0, SeekOrigin.Begin);

            // For small streams, see if we can read the byte buffer directly.
            if (encoding.GetMaxCharCount((int)data.Length) < LargeObjectHeapLimitInChars)
            {
                byte[] buffer = TryGetByteArrayFromStream(data);
                if (buffer != null)
                {
                    return SourceText.From(buffer, (int)data.Length, encoding, checksumAlgorithm, throwIfBinaryDetected, canBeEmbedded);
                }
            }

            return SourceText.From(data, encoding, checksumAlgorithm, throwIfBinaryDetected, canBeEmbedded);
        }

        // FilRip : Remove this method by the one above
        /*private static SourceText Decode(Stream data, Encoding encoding, SourceHashAlgorithm checksumAlgorithm, bool throwIfBinaryDetected = false, bool canBeEmbedded = false)
		{
			if (data.CanSeek)
			{
				data.Seek(0L, SeekOrigin.Begin);
				if (encoding.GetMaxCharCountOrThrowIfHuge(data) < 40960 && TryGetBytesFromStream(data, out var bytes) && bytes.Offset == 0 && bytes.Array != null)
				{
					return SourceText.From(bytes.Array, (int)data.Length, encoding, checksumAlgorithm, throwIfBinaryDetected, canBeEmbedded);
				}
			}
			return SourceText.From(data, encoding, checksumAlgorithm, throwIfBinaryDetected, canBeEmbedded);
		}*/

        /// <summary>
        /// Some streams are easily represented as byte arrays.
        /// </summary>
        /// <param name="data">The stream</param>
        /// <returns>
        /// The contents of <paramref name="data"/> as a byte array or null if the stream can't easily
        /// be read into a byte array.
        /// </returns>
        public static byte[] TryGetByteArrayFromStream(Stream data)
        {

            // PERF: If the input is a MemoryStream, we may be able to get at the buffer directly
            var memoryStream = data as MemoryStream;
            if (memoryStream != null && TryGetByteArrayFromMemoryStream(memoryStream, out byte[] buffer))
            {
                return buffer;
            }

            // PERF: If the input is a FileStream, we may be able to minimize allocations
            if (data.GetType() == PortableShim.FileStream.Type &&
                TryGetByteArrayFromFileStream(data, out buffer))
            {
                return buffer;
            }

            return null;
        }

        /// <summary>
        /// Read the contents of a FileStream into a byte array.
        /// </summary>
        /// <param name="stream">The FileStream with encoded text.</param>
        /// <param name="buffer">A byte array filled with the contents of the file.</param>
        /// <returns>True if a byte array could be created.</returns>
        public static bool TryGetByteArrayFromFileStream(Stream stream, out byte[] buffer)
        {
            int length = (int)stream.Length;
            if (length == 0)
            {
                //buffer = SpecializedCollections.EmptyBytes;
                // FilRip : Change EmptyBytes to empty array
                buffer = new byte[0];
                return true;
            }

            // PERF: While this is an obvious byte array allocation, it is still cheaper than
            // using StreamReader.ReadToEnd. The alternative allocates:
            // 1. A 1KB byte array in the StreamReader for buffered reads
            // 2. A 4KB byte array in the FileStream for buffered reads
            // 3. A StringBuilder and its associated char arrays (enough to represent the final decoded string)

            // TODO: Can this allocation be pooled?
            buffer = new byte[length];

            // Note: FileStream.Read may still allocate its internal buffer if length is less
            // than the buffer size. The default buffer size is 4KB, so this will incur a 4KB
            // allocation for any files less than 4KB. That's why, for example, the command
            // line compiler actually specifies a very small buffer size.
            return stream.TryReadAll(buffer, 0, length) == length;
        }

        /// <summary>
        /// If the MemoryStream was created with publiclyVisible=true, then we can access its buffer
        /// directly and save allocations in StreamReader. The input MemoryStream is not closed on exit.
        /// </summary>
        /// <returns>True if a byte array could be created.</returns>
        public static bool TryGetByteArrayFromMemoryStream(MemoryStream data, out byte[] buffer)
        {
            try
            {
                if (PortableShim.MemoryStream.GetBuffer != null)
                {
                    buffer = (byte[])PortableShim.MemoryStream.GetBuffer.Invoke(data, null);
                    return true;
                }

                buffer = null;
                return false;
            }
            catch (Exception)
            {
                buffer = null;
                return false;
            }
        }

        // FilRip : Remove this method by above
        /*internal static bool TryGetBytesFromStream(Stream data, out ArraySegment<byte> bytes)
		{
			if (data is MemoryStream memoryStream)
			{
				return memoryStream.TryGetBuffer(out bytes);
			}
			if (data is FileStream stream)
			{
				return TryGetBytesFromFileStream(stream, out bytes);
			}
			bytes = new ArraySegment<byte>(new byte[0]);
			return false;
		}

		private static bool TryGetBytesFromFileStream(FileStream stream, out ArraySegment<byte> bytes)
		{
			int num = (int)stream.Length;
			if (num == 0)
			{
				bytes = new ArraySegment<byte>(new byte[0]);
				return true;
			}
			byte[] array = new byte[num];
			bool flag = stream.TryReadAll(array, 0, num) == num;
			bytes = (flag ? new ArraySegment<byte>(array) : new ArraySegment<byte>(new byte[0]));
			return flag;
		}*/
    }
}
