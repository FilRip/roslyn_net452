using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Microsoft.CodeAnalysis.CommandLine
{
	internal sealed class FileMutex : IDisposable
	{
		public readonly FileStream Stream;

		public readonly string FilePath;

		public bool IsLocked { get; private set; }

		internal static string GetMutexDirectory()
		{
			string text = Path.Combine(BuildServerConnection.GetTempPath(null), ".roslyn");
			Directory.CreateDirectory(text);
			return text;
		}

		public FileMutex(string name)
		{
			FilePath = Path.Combine(GetMutexDirectory(), name);
			Stream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
		}

		public bool TryLock(int timeoutMs)
		{
			if (IsLocked)
			{
				throw new InvalidOperationException("Lock already held");
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			do
			{
				try
				{
					Stream.Lock(0L, 0L);
					IsLocked = true;
					return true;
				}
				catch (IOException)
				{
					Thread.Sleep(1);
				}
				catch (Exception)
				{
					return false;
				}
			}
			while (stopwatch.ElapsedMilliseconds < timeoutMs);
			return false;
		}

		public void Unlock()
		{
			if (IsLocked)
			{
				Stream.Unlock(0L, 0L);
				IsLocked = false;
			}
		}

		public void Dispose()
		{
			if (IsLocked)
			{
				Unlock();
			}
			Stream.Dispose();
		}
	}
}
