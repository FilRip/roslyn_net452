using System.Diagnostics;

namespace Microsoft.CodeAnalysis
{
    [DebuggerDisplay("{Path,nq}")]
    public struct CommandLineSourceFile
    {
        public string Path { get; }

        public bool IsInputRedirected { get; }

        public bool IsScript { get; }

        public CommandLineSourceFile(string path, bool isScript)
            : this(path, isScript, isInputRedirected: false)
        {
        }

        public CommandLineSourceFile(string path, bool isScript, bool isInputRedirected)
        {
            Path = path;
            IsScript = isScript;
            IsInputRedirected = isInputRedirected;
        }
    }
}
