using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using Microsoft.Win32.SafeHandles;

namespace System.Text
{
    internal abstract class BaseCodePageEncoding : EncodingNLS, ISerializable
    {
        [StructLayout(LayoutKind.Explicit)]
        internal struct CodePageDataFileHeader
        {
            [FieldOffset(0)]
            internal char TableName;

            [FieldOffset(32)]
            internal ushort Version;

            [FieldOffset(40)]
            internal short CodePageCount;

            [FieldOffset(42)]
            internal short unused1;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 2)]
        internal struct CodePageIndex
        {
            [FieldOffset(0)]
            internal char CodePageName;

            [FieldOffset(32)]
            internal short CodePage;

            [FieldOffset(34)]
            internal short ByteCount;

            [FieldOffset(36)]
            internal int Offset;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct CodePageHeader
        {
            [FieldOffset(0)]
            internal char CodePageName;

            [FieldOffset(32)]
            internal ushort VersionMajor;

            [FieldOffset(34)]
            internal ushort VersionMinor;

            [FieldOffset(36)]
            internal ushort VersionRevision;

            [FieldOffset(38)]
            internal ushort VersionBuild;

            [FieldOffset(40)]
            internal short CodePage;

            [FieldOffset(42)]
            internal short ByteCount;

            [FieldOffset(44)]
            internal char UnicodeReplace;

            [FieldOffset(46)]
            internal ushort ByteReplace;
        }

        internal const string CODE_PAGE_DATA_FILE_NAME = "codepages.nlp";

        protected int dataTableCodePage;

        protected int iExtraBytes;

        protected char[] arrayUnicodeBestFit;

        protected char[] arrayBytesBestFit;

        private const int CODEPAGE_DATA_FILE_HEADER_SIZE = 44;

        private const int CODEPAGE_HEADER_SIZE = 48;

        private static readonly byte[] s_codePagesDataHeader = new byte[44];

        protected static Stream s_codePagesEncodingDataStream = GetEncodingDataStream("codepages.nlp");

        protected static readonly object s_streamLock = new();

        protected byte[] m_codePageHeader = new byte[48];

        protected int m_firstDataWordOffset;

        protected int m_dataSize;

        protected SafeAllocHHandle safeNativeMemoryHandle;

        internal BaseCodePageEncoding(int codepage)
            : base(codepage)
        {
        }

        internal BaseCodePageEncoding(int codepage, int dataCodePage)
			: base(codepage, new InternalEncoderBestFitFallback(null), new InternalDecoderBestFitFallback(null))
		{
			SetFallbackEncoding();
			dataTableCodePage = dataCodePage;
			LoadCodePageTables();
		}

		internal BaseCodePageEncoding(int codepage, int dataCodePage, EncoderFallback enc, DecoderFallback dec)
			: base(codepage, enc, dec)
		{
			dataTableCodePage = dataCodePage;
			LoadCodePageTables();
		}

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new PlatformNotSupportedException();
        }

        private void SetFallbackEncoding()
        {
            (base.EncoderFallback as InternalEncoderBestFitFallback).encoding = this;
            (base.DecoderFallback as InternalDecoderBestFitFallback).encoding = this;
        }

        internal static Stream GetEncodingDataStream(string tableName)
        {
            Stream manifestResourceStream = typeof(CodePagesEncodingProvider).GetTypeInfo().Assembly.GetManifestResourceStream(tableName);
            if (manifestResourceStream == null)
            {
                throw new InvalidOperationException();
            }
            manifestResourceStream.Read(s_codePagesDataHeader, 0, s_codePagesDataHeader.Length);
            return manifestResourceStream;
        }

        private void LoadCodePageTables()
        {
            if (!FindCodePage(dataTableCodePage))
            {
                throw new NotSupportedException(SR.Format(SR.NotSupported_NoCodepageData, CodePage));
            }
            LoadManagedCodePage();
        }

        private unsafe bool FindCodePage(int codePage)
        {
            byte[] array = new byte[sizeof(CodePageIndex)];
            lock (s_streamLock)
            {
                s_codePagesEncodingDataStream.Seek(44L, SeekOrigin.Begin);
                int codePageCount;
                fixed (byte* ptr = &s_codePagesDataHeader[0])
                {
                    CodePageDataFileHeader* ptr2 = (CodePageDataFileHeader*)ptr;
                    codePageCount = ptr2->CodePageCount;
                }
                fixed (byte* ptr3 = &array[0])
                {
                    CodePageIndex* ptr4 = (CodePageIndex*)ptr3;
                    for (int i = 0; i < codePageCount; i++)
                    {
                        s_codePagesEncodingDataStream.Read(array, 0, array.Length);
                        if (ptr4->CodePage == codePage)
                        {
                            long position = s_codePagesEncodingDataStream.Position;
                            s_codePagesEncodingDataStream.Seek(ptr4->Offset, SeekOrigin.Begin);
                            s_codePagesEncodingDataStream.Read(m_codePageHeader, 0, m_codePageHeader.Length);
                            m_firstDataWordOffset = (int)s_codePagesEncodingDataStream.Position;
                            if (i == codePageCount - 1)
                            {
                                m_dataSize = (int)(s_codePagesEncodingDataStream.Length - ptr4->Offset - m_codePageHeader.Length);
                            }
                            else
                            {
                                s_codePagesEncodingDataStream.Seek(position, SeekOrigin.Begin);
                                int offset = ptr4->Offset;
                                s_codePagesEncodingDataStream.Read(array, 0, array.Length);
                                m_dataSize = ptr4->Offset - offset - m_codePageHeader.Length;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal unsafe static int GetCodePageByteSize(int codePage)
        {
            byte[] array = new byte[sizeof(CodePageIndex)];
            lock (s_streamLock)
            {
                s_codePagesEncodingDataStream.Seek(44L, SeekOrigin.Begin);
                int codePageCount;
                fixed (byte* ptr = &s_codePagesDataHeader[0])
                {
                    CodePageDataFileHeader* ptr2 = (CodePageDataFileHeader*)ptr;
                    codePageCount = ptr2->CodePageCount;
                }
                fixed (byte* ptr3 = &array[0])
                {
                    CodePageIndex* ptr4 = (CodePageIndex*)ptr3;
                    for (int i = 0; i < codePageCount; i++)
                    {
                        s_codePagesEncodingDataStream.Read(array, 0, array.Length);
                        if (ptr4->CodePage == codePage)
                        {
                            return ptr4->ByteCount;
                        }
                    }
                }
            }
            return 0;
        }

        protected abstract void LoadManagedCodePage();

        protected unsafe byte* GetNativeMemory(int iSize)
        {
            if (safeNativeMemoryHandle == null)
            {
                byte* ptr = (byte*)(void*)Marshal.AllocHGlobal(iSize);
                safeNativeMemoryHandle = new SafeAllocHHandle((IntPtr)ptr);
            }
            return (byte*)(void*)safeNativeMemoryHandle.DangerousGetHandle();
        }

        protected abstract void ReadBestFitTable();

        internal char[] GetBestFitUnicodeToBytesData()
        {
            if (arrayUnicodeBestFit == null)
            {
                ReadBestFitTable();
            }
            return arrayUnicodeBestFit;
        }

        internal char[] GetBestFitBytesToUnicodeData()
        {
            if (arrayBytesBestFit == null)
            {
                ReadBestFitTable();
            }
            return arrayBytesBestFit;
        }

        internal void CheckMemorySection()
        {
            if (safeNativeMemoryHandle != null && safeNativeMemoryHandle.DangerousGetHandle() == IntPtr.Zero)
            {
                LoadManagedCodePage();
            }
        }
    }
}
