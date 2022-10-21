﻿using System;
using System.Runtime.InteropServices;
using System.Text;

#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal static class NativeMethods
    {
        internal static readonly IntPtr NullPtr = IntPtr.Zero;
        internal static readonly IntPtr InvalidIntPtr = new(-1);
        internal const uint NORMAL_PRIORITY_CLASS = 32;
        internal const uint CREATE_NO_WINDOW = 134217728;
        internal const int STARTF_USESTDHANDLES = 256;
        internal const int ERROR_SUCCESS = 0;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CreateProcess(
          string? lpApplicationName,
          [In, Out] StringBuilder lpCommandLine,
          IntPtr lpProcessAttributes,
          IntPtr lpThreadAttributes,
          [MarshalAs(UnmanagedType.Bool), In] bool bInheritHandles,
          uint dwCreationFlags,
          IntPtr lpEnvironment,
          string lpCurrentDirectory,
          [In] ref STARTUPINFO lpStartupInfo,
          out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr GetCommandLine();
    }
}
