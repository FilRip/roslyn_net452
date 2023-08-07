using System.Runtime.InteropServices;

namespace System
{
    internal static class Interop
    {
        internal static class Kernel32
        {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            private struct CpInfoExW
            {
                internal uint MaxCharSize;

                internal unsafe fixed byte DefaultChar[2];

                internal unsafe fixed byte LeadByte[12];

                internal char UnicodeDefaultChar;

                internal uint CodePage;

                internal unsafe fixed byte CodePageName[260];
            }

            internal const uint CP_ACP = 0u;

            internal const uint WC_NO_BEST_FIT_CHARS = 1024u;

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            private unsafe static extern int GetCPInfoExW(uint CodePage, uint dwFlags, CpInfoExW* lpCPInfoEx);

            internal unsafe static int GetLeadByteRanges(int codePage, byte[] leadByteRanges)
            {
                int num = 0;
                CpInfoExW cPINFOEXW = default;
                if (GetCPInfoExW((uint)codePage, 0u, &cPINFOEXW) != 0)
                {
                    for (int i = 0; i < 10 && leadByteRanges[i] != 0; i += 2)
                    {
                        leadByteRanges[i] = cPINFOEXW.LeadByte[i];
                        leadByteRanges[i + 1] = cPINFOEXW.LeadByte[i + 1];
                        num++;
                    }
                }
                return num;
            }

            internal unsafe static bool TryGetACPCodePage(out int codePage)
            {
                CpInfoExW cPINFOEXW = default;
                if (GetCPInfoExW(0u, 0u, &cPINFOEXW) != 0)
                {
                    codePage = (int)cPINFOEXW.CodePage;
                    return true;
                }
                codePage = 0;
                return false;
            }
        }
    }
}
