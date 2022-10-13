// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.CommandLine.FileMutex
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe

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
                catch (IOException ex)
                {
                    Thread.Sleep(1);
                }
                catch (Exception ex)
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
