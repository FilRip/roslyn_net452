using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipes;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    internal static class RuntimeHostInfo
    {
        internal static bool IsCoreClrRuntime => !IsDesktopRuntime;

        internal static string ToolExtension
        {
            get
            {
                if (!IsCoreClrRuntime)
                {
                    return "exe";
                }
                return "dll";
            }
        }

        private static string NativeToolSuffix
        {
            get
            {
                if (!PlatformInformation.IsWindows)
                {
                    return "";
                }
                return ".exe";
            }
        }

        internal static bool IsDesktopRuntime => true;

        internal static (string processFilePath, string commandLineArguments, string toolFilePath) GetProcessInfo(string toolFilePathWithoutExtension, string commandLineArguments)
        {
            string text = toolFilePathWithoutExtension + NativeToolSuffix;
            if (IsCoreClrRuntime && File.Exists(text))
            {
                return (text, commandLineArguments, text);
            }
            string text2 = toolFilePathWithoutExtension + "." + ToolExtension;
            if (IsDotNetHost(out var pathToDotNet))
            {
                commandLineArguments = "exec \"" + text2 + "\" " + commandLineArguments;
                return (pathToDotNet, commandLineArguments, text2);
            }
            return (text2, commandLineArguments, text2);
        }

        internal static bool IsDotNetHost([NotNullWhen(true)] out string? pathToDotNet)
        {
            pathToDotNet = null;
            return false;
        }

        internal static NamedPipeClientStream CreateNamedPipeClient(string serverName, string pipeName, PipeDirection direction, PipeOptions options)
        {
            return new NamedPipeClientStream(serverName, pipeName, direction, options);
        }
    }
}
