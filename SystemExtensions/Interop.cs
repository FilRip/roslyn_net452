using System.Runtime.InteropServices;

internal static class Interop
{
	internal static class Kernel32
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct CPINFOEXW
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
		private unsafe static extern int GetCPInfoExW(uint CodePage, uint dwFlags, CPINFOEXW* lpCPInfoEx);

		internal unsafe static int GetLeadByteRanges(int codePage, byte[] leadByteRanges)
		{
			int num = 0;
			CPINFOEXW cPINFOEXW = default(CPINFOEXW);
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
			CPINFOEXW cPINFOEXW = default(CPINFOEXW);
			if (GetCPInfoExW(0u, 0u, &cPINFOEXW) != 0)
			{
				codePage = (int)cPINFOEXW.CodePage;
				return true;
			}
			codePage = 0;
			return false;
		}
	}

	internal static class Libraries
	{
		internal const string Advapi32 = "advapi32.dll";

		internal const string BCrypt = "BCrypt.dll";

		internal const string CoreComm_L1_1_1 = "api-ms-win-core-comm-l1-1-1.dll";

		internal const string Crypt32 = "crypt32.dll";

		internal const string Error_L1 = "api-ms-win-core-winrt-error-l1-1-0.dll";

		internal const string HttpApi = "httpapi.dll";

		internal const string IpHlpApi = "iphlpapi.dll";

		internal const string Kernel32 = "kernel32.dll";

		internal const string Memory_L1_3 = "api-ms-win-core-memory-l1-1-3.dll";

		internal const string Mswsock = "mswsock.dll";

		internal const string NCrypt = "ncrypt.dll";

		internal const string NtDll = "ntdll.dll";

		internal const string Odbc32 = "odbc32.dll";

		internal const string OleAut32 = "oleaut32.dll";

		internal const string PerfCounter = "perfcounter.dll";

		internal const string RoBuffer = "api-ms-win-core-winrt-robuffer-l1-1-0.dll";

		internal const string Secur32 = "secur32.dll";

		internal const string Shell32 = "shell32.dll";

		internal const string SspiCli = "sspicli.dll";

		internal const string User32 = "user32.dll";

		internal const string Version = "version.dll";

		internal const string WebSocket = "websocket.dll";

		internal const string WinHttp = "winhttp.dll";

		internal const string Ws2_32 = "ws2_32.dll";

		internal const string Wtsapi32 = "wtsapi32.dll";

		internal const string CompressionNative = "clrcompression.dll";
	}
}
