// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.CommandLine.ServerFileMutexPair
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe

using System;


#nullable enable
namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class ServerFileMutexPair : IServerMutex, IDisposable
    {
        public readonly FileMutex AliveMutex;
        public readonly FileMutex HeldMutex;

        public bool IsDisposed { get; private set; }

        public ServerFileMutexPair(string mutexName, bool initiallyOwned, out bool createdNew)
        {
            this.AliveMutex = new FileMutex(mutexName + "-alive");
            this.HeldMutex = new FileMutex(mutexName + "-held");
            createdNew = this.AliveMutex.TryLock(0);
            if (initiallyOwned & createdNew && !this.TryLock(0))
                throw new Exception("Failed to lock mutex after creating it");
        }

        public bool TryLock(int timeoutMs)
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException("Mutex");
            return this.HeldMutex.TryLock(timeoutMs);
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;
            this.IsDisposed = true;
            try
            {
                this.HeldMutex.Unlock();
                this.AliveMutex.Unlock();
            }
            finally
            {
                this.AliveMutex.Dispose();
                this.HeldMutex.Dispose();
            }
        }
    }
}
