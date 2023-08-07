using System;

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal struct ProcessInformation
    {
        public IntPtr hProcess;

        public IntPtr hThread;

        public int dwProcessId;

        public int dwThreadId;
    }
}
