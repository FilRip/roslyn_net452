#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class EmptyCompilerServerLogger : ICompilerServerLogger
    {
        public static EmptyCompilerServerLogger Instance { get; } = new EmptyCompilerServerLogger();

        public bool IsLogging => false;

        private EmptyCompilerServerLogger()
        {
        }

        public void Log(string message)
        {
            // Nothing to do
        }
    }
}
