using System;
using System.Threading;


#nullable enable
namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class ServerNamedMutex : IServerMutex, IDisposable
    {
        public readonly Mutex Mutex;

        public bool IsDisposed { get; private set; }

        public bool IsLocked { get; private set; }

        public ServerNamedMutex(string mutexName, out bool createdNew)
        {
            this.Mutex = new Mutex(true, mutexName, out createdNew);
            if (!createdNew)
                return;
            this.IsLocked = true;
        }

        public static bool WasOpen(string mutexName)
        {
            Mutex? result = null;
            try
            {
                return Mutex.TryOpenExisting(mutexName, out result);
            }
            catch
            {
                return false;
            }
            finally
            {
                result?.Dispose();
            }
        }

        public bool TryLock(int timeoutMs)
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException("Mutex");
            if (this.IsLocked)
                throw new InvalidOperationException("Lock already held");
            return this.IsLocked = this.Mutex.WaitOne(timeoutMs);
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;
            this.IsDisposed = true;
            try
            {
                if (!this.IsLocked)
                    return;
                this.Mutex.ReleaseMutex();
            }
            finally
            {
                this.Mutex.Dispose();
                this.IsLocked = false;
            }
        }
    }
}
