using System;
using System.Threading;

namespace Microsoft.CodeAnalysis.CommandLine
{
	internal sealed class ServerNamedMutex : IServerMutex, IDisposable
	{
		public readonly Mutex Mutex;

		public bool IsDisposed { get; private set; }

		public bool IsLocked { get; private set; }

		public ServerNamedMutex(string mutexName, out bool createdNew)
		{
			Mutex = new Mutex(initiallyOwned: true, mutexName, out createdNew);
			if (createdNew)
			{
				IsLocked = true;
			}
		}

		public static bool WasOpen(string mutexName)
		{
			Mutex result = null;
			try
			{
				return System.Threading.Mutex.TryOpenExisting(mutexName, out result);
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
			if (IsDisposed)
			{
				throw new ObjectDisposedException("Mutex");
			}
			if (IsLocked)
			{
				throw new InvalidOperationException("Lock already held");
			}
			return IsLocked = Mutex.WaitOne(timeoutMs);
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
				if (IsLocked)
				{
					Mutex.ReleaseMutex();
				}
			}
			finally
			{
				Mutex.Dispose();
				IsLocked = false;
			}
		}
	}
}
