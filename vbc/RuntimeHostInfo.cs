﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.RuntimeHostInfo
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipes;

using Roslyn.Utilities;


#nullable enable
namespace Microsoft.CodeAnalysis
{
    internal static class RuntimeHostInfo
    {
        internal static bool IsCoreClrRuntime => !RuntimeHostInfo.IsDesktopRuntime;

        internal static string ToolExtension => !RuntimeHostInfo.IsCoreClrRuntime ? "exe" : "dll";

        private static string NativeToolSuffix => !PlatformInformation.IsWindows ? "" : ".exe";

        internal static (string processFilePath, string commandLineArguments, string toolFilePath) GetProcessInfo(
          string toolFilePathWithoutExtension,
          string commandLineArguments)
        {
            string path = toolFilePathWithoutExtension + RuntimeHostInfo.NativeToolSuffix;
            if (RuntimeHostInfo.IsCoreClrRuntime && File.Exists(path))
                return (path, commandLineArguments, path);
            string str = toolFilePathWithoutExtension + "." + RuntimeHostInfo.ToolExtension;
            if (!RuntimeHostInfo.IsDotNetHost(out string pathToDotNet))
                return (str, commandLineArguments, str);
            commandLineArguments = "exec \"" + str + "\" " + commandLineArguments;
            return (pathToDotNet, commandLineArguments, str);
        }

        internal static bool IsDesktopRuntime => true;

        internal static bool IsDotNetHost([NotNullWhen(true)] out string? pathToDotNet)
        {
            pathToDotNet = null;
            return false;
        }

        internal static NamedPipeClientStream CreateNamedPipeClient(
          string serverName,
          string pipeName,
          PipeDirection direction,
          PipeOptions options)
        {
            return new NamedPipeClientStream(serverName, pipeName, direction, options);
        }
    }
}
