using System;

namespace Microsoft.CodeAnalysis.CommandLine
{
	internal interface IServerMutex : IDisposable
	{
		bool IsDisposed { get; }

		bool TryLock(int timeoutMs);
	}
}
