using System;

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class ServerFileMutexPair : IServerMutex
    {
        public readonly FileMutex AliveMutex;

        public readonly FileMutex HeldMutex;

        public bool IsDisposed { get; private set; }

        public ServerFileMutexPair(string mutexName, bool initiallyOwned, out bool createdNew)
        {
            AliveMutex = new FileMutex(mutexName + "-alive");
            HeldMutex = new FileMutex(mutexName + "-held");
            createdNew = AliveMutex.TryLock(0);
            if ((initiallyOwned && createdNew) && !TryLock(0))
            {
                throw new CscException("Failed to lock mutex after creating it");
            }
        }

        public bool TryLock(int timeoutMs)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("Mutex");
            }
            return HeldMutex.TryLock(timeoutMs);
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            IsDisposed = true;
            try
            {
                HeldMutex.Unlock();
                AliveMutex.Unlock();
            }
            finally
            {
                AliveMutex.Dispose();
                HeldMutex.Dispose();
            }
        }
    }
}
