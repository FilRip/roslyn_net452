using System.Runtime.InteropServices;

using Microsoft.CodeAnalysis.CommandLine;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CompilerServer
{
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class MemoryHelper
    {
        public int Length;
        public int PercentPhysicalUsed;
        public ulong MaxPhysical;
        public ulong AvailablePhysical;
        public ulong MaxPageFile;
        public ulong AvailablePageFile;
        public ulong MaxVirtual;
        public ulong AvailableVirtual;
        public ulong Reserved;

        private MemoryHelper() => this.Length = Marshal.SizeOf<MemoryHelper>(this);

        public static bool IsMemoryAvailable(ICompilerServerLogger logger)
        {
            if (!PlatformInformation.IsWindows)
                return true;
            MemoryHelper buffer = new();
            MemoryHelper.GlobalMemoryStatusEx(buffer);
            ulong maxVirtual = buffer.MaxVirtual;
            ulong availableVirtual = buffer.AvailableVirtual;
            int num = 20;
            string str = "MB";
            if (availableVirtual >> num == 0UL)
            {
                num = 10;
                str = "KB";
            }
            logger.Log("Free memory: {1}{0} of {2}{0}.", str, availableVirtual >> num, maxVirtual >> num);
            return availableVirtual >= 838860800UL;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MemoryHelper buffer);
    }
}
