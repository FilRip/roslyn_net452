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

		public bool IsLogging => _loggingStream != null;

		public CompilerServerLogger(string identifier)
		{
			_identifier = identifier;
			try
			{
				string text = Environment.GetEnvironmentVariable("RoslynCommandLineLogFile");
				if (text != null)
				{
					if (Directory.Exists(text))
					{
						int id = Process.GetCurrentProcess().Id;
						text = Path.Combine(text, $"server.{id}.log");
					}
					_loggingStream = new FileStream(text, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
				}
			}
			catch (Exception)
			{
			}
		}

		public void Dispose()
		{
			_loggingStream?.Dispose();
			_loggingStream = null;
		}

		public void Log(string message)
		{
			if (_loggingStream != null)
			{
				int currentManagedThreadId = Environment.CurrentManagedThreadId;
				string s = $"ID={_identifier} TID={currentManagedThreadId}: " + message + Environment.NewLine;
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				_loggingStream!.Seek(0L, SeekOrigin.End);
				_loggingStream!.Write(bytes, 0, bytes.Length);
				_loggingStream!.Flush();
			}
		}
	}
}
