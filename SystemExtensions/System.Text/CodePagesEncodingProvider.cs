using System.Collections.Generic;
using System.Threading;

using SystemExtensions;

namespace System.Text
{
    public sealed class CodePagesEncodingProvider : EncodingProvider
    {
        private static readonly EncodingProvider s_singleton = new CodePagesEncodingProvider();

        private readonly Dictionary<int, Encoding> _encodings = new();

        private readonly ReaderWriterLockSlim _cacheLock = new();

        private const int ISCIIAssemese = 57006;

        private const int ISCIIBengali = 57003;

        private const int ISCIIDevanagari = 57002;

        private const int ISCIIGujarathi = 57010;

        private const int ISCIIKannada = 57008;

        private const int ISCIIMalayalam = 57009;

        private const int ISCIIOriya = 57007;

        private const int ISCIIPanjabi = 57011;

        private const int ISCIITamil = 57004;

        private const int ISCIITelugu = 57005;

        private const int ISOKorean = 50225;

        private const int ChineseHZ = 52936;

        private const int ISO2022JP = 50220;

        private const int ISO2022JPESC = 50221;

        private const int ISO2022JPSISO = 50222;

        private const int ISOSimplifiedCN = 50227;

        private const int EUCJP = 51932;

        private const int CodePageMacGB2312 = 10008;

        private const int CodePageMacKorean = 10003;

        private const int GB18030 = 54936;

        private const int DuplicateEUCCN = 51936;

        private const int EUCKR = 51949;

        private const int ISO_8859_8I = 38598;

        public static EncodingProvider Instance => s_singleton;

        private static int SystemDefaultCodePage
        {
            get
            {
                if (!Interop.Kernel32.TryGetACPCodePage(out var codePage))
                {
                    return 0;
                }
                return codePage;
            }
        }

        internal CodePagesEncodingProvider()
        {
        }

        public override Encoding GetEncoding(int codepage)
        {
            if (codepage < 0 || codepage > 65535)
            {
                return null;
            }
            if (codepage == 0)
            {
                int systemDefaultCodePage = SystemDefaultCodePage;
                if (systemDefaultCodePage == 0)
                {
                    return null;
                }
                return GetEncoding(systemDefaultCodePage);
            }
            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                if (_encodings.TryGetValue(codepage, out Encoding value))
                {
                    return value;
                }
                switch (BaseCodePageEncoding.GetCodePageByteSize(codepage))
                {
                    case 1:
                        value = new SbCsCodePageEncoding(codepage);
                        break;
                    case 2:
                        value = new DbCsCodePageEncoding(codepage);
                        break;
                    default:
                        value = GetEncodingRare(codepage);
                        if (value == null)
                        {
                            return null;
                        }
                        break;
                }
                _cacheLock.EnterWriteLock();
                try
                {
                    if (_encodings.TryGetValue(codepage, out var value2))
                    {
                        return value2;
                    }
                    _encodings.Add(codepage, value);
                    return value;
                }
                finally
                {
                    _cacheLock.ExitWriteLock();
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        public override Encoding GetEncoding(string name)
        {
            int codePageFromName = EncodingTable.GetCodePageFromName(name);
            if (codePageFromName == 0)
            {
                return null;
            }
            return GetEncoding(codePageFromName);
        }

        private static Encoding GetEncodingRare(int codepage)
        {
            Encoding result = null;
            switch (codepage)
            {
                case ISCIIDevanagari:
                case ISCIIBengali:
                case ISCIITamil:
                case ISCIITelugu:
                case ISCIIAssemese:
                case ISCIIOriya:
                case ISCIIKannada:
                case ISCIIMalayalam:
                case ISCIIGujarathi:
                case ISCIIPanjabi:
                    result = new IsciiEncoding(codepage);
                    break;
                case CodePageMacGB2312:
                    result = new DbCsCodePageEncoding(10008);
                    break;
                case CodePageMacKorean:
                    result = new DbCsCodePageEncoding(10003);
                    break;
                case GB18030:
                    result = new GB18030Encoding();
                    break;
                case ISO2022JP:
                case ISO2022JPESC:
                case ISO2022JPSISO:
                case ISOKorean:
                case ChineseHZ:
                    result = new Iso2022Encoding(codepage);
                    break;
                case ISOSimplifiedCN:
                case DuplicateEUCCN:
                    result = new DbCsCodePageEncoding(codepage);
                    break;
                case EUCJP:
                    result = new EUCJPEncoding();
                    break;
                case EUCKR:
                    result = new DbCsCodePageEncoding(codepage);
                    break;
                case ISO_8859_8I:
                    result = new SbCsCodePageEncoding(codepage);
                    break;
            }
            return result;
        }
    }
}
