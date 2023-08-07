using System;

#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class ServerFileMutexPair : IServerMutex
    {
        public readonly FileMutex AliveMutex;
        public readonly FileMutex HeldMutex;

        public bool IsDisposed { get; private set; }

        public ServerFileMutexPair(string mutexName, bool initiallyOwned, out bool createdNew)
        {
            this.AliveMutex = new FileMutex(mutexName + "-alive");
            this.HeldMutex = new FileMutex(mutexName + "-held");
            createdNew = this.AliveMutex.TryLock(0);
            if (initiallyOwned && createdNew && !this.TryLock(0))
                throw new VbCsCompilerException("Failed to lock mutex after creating it");
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
