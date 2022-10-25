using System;
using System.Diagnostics;
using System.IO;
using System.Threading;


#nullable enable
namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class FileMutex : IDisposable
    {
        public readonly FileStream Stream;
        public readonly string FilePath;

        public bool IsLocked { get; private set; }

        internal static string GetMutexDirectory()
        {
            string path = Path.Combine(BuildServerConnection.GetTempPath(null), ".roslyn");
            Directory.CreateDirectory(path);
            return path;
        }

        public FileMutex(string name)
        {
            this.FilePath = Path.Combine(FileMutex.GetMutexDirectory(), name);
            this.Stream = new FileStream(this.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        public bool TryLock(int timeoutMs)
        {
            if (this.IsLocked)
                throw new InvalidOperationException("Lock already held");
            Stopwatch stopwatch = Stopwatch.StartNew();
            do
            {
                try
                {
                    this.Stream.Lock(0L, 0L);
                    this.IsLocked = true;
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
            if (!this.IsLocked)
                return;
            this.Stream.Unlock(0L, 0L);
            this.IsLocked = false;
        }

        public void Dispose()
        {
            if (this.IsLocked)
                this.Unlock();
            this.Stream.Dispose();
        }
    }
}
