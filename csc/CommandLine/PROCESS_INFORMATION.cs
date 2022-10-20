using System;

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;

        public IntPtr hThread;

        public int dwProcessId;

        public int dwThreadId;
    }
}
