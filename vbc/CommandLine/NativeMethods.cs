// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.CommandLine.NativeMethods
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe

using System;
using System.Runtime.InteropServices;
using System.Text;


#nullable enable
namespace Microsoft.CodeAnalysis.CommandLine
{
    internal static class NativeMethods
    {
        internal static readonly IntPtr NullPtr = IntPtr.Zero;
        internal static readonly IntPtr InvalidIntPtr = new IntPtr(-1);
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
