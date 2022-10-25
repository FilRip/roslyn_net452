using System;

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal interface IServerMutex : IDisposable
    {
        bool TryLock(int timeoutMs);

        bool IsDisposed { get; }
    }
}
