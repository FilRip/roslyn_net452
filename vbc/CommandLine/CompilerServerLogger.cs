using System;
using System.Diagnostics;
using System.IO;
using System.Text;

#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class CompilerServerLogger : ICompilerServerLogger, IDisposable
    {
        internal const string EnvironmentVariableName = "RoslynCommandLineLogFile";
        internal const string LoggingPrefix = "---";
        private Stream? _loggingStream;
        private readonly string _identifier;

        public bool IsLogging => this._loggingStream != null;

        public CompilerServerLogger(string identifier)
        {
            this._identifier = identifier;
            try
            {
                string str = Environment.GetEnvironmentVariable("RoslynCommandLineLogFile");
                if (str == null)
                    return;
                if (Directory.Exists(str))
                {
                    int id = Process.GetCurrentProcess().Id;
                    str = Path.Combine(str, string.Format("server.{0}.log", id));
                }
                this._loggingStream = new FileStream(str, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            }
            catch (Exception)
            {
                // Nothing to do
            }
        }

        public void Dispose()
        {
            this._loggingStream?.Dispose();
            this._loggingStream = null;
        }

        public void Log(string message)
        {
            if (this._loggingStream == null)
                return;
            byte[] bytes = Encoding.UTF8.GetBytes(string.Format("ID={0} TID={1}: ", _identifier, Environment.CurrentManagedThreadId) + message + Environment.NewLine);
            this._loggingStream.Seek(0L, SeekOrigin.End);
            this._loggingStream.Write(bytes, 0, bytes.Length);
            this._loggingStream.Flush();
        }
    }
}
