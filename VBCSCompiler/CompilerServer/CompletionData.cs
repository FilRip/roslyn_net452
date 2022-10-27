using System;

#nullable enable

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal readonly struct CompletionData
    {
        internal CompletionReason Reason { get; }

        internal TimeSpan? NewKeepAlive { get; }

        internal bool ShutdownRequest { get; }

        internal CompletionData(
            CompletionReason reason,
            TimeSpan? newKeepAlive = null,
            bool shutdownRequested = false)
        {
            this.Reason = reason;
            this.NewKeepAlive = newKeepAlive;
            this.ShutdownRequest = shutdownRequested;
        }

        internal static CompletionData RequestCompleted { get; } = new CompletionData(CompletionReason.RequestCompleted);

        internal static CompletionData RequestError { get; } = new CompletionData(CompletionReason.RequestError);

        public override string ToString() => string.Format("{0} KeepAlive:{1} ShutdownRequest:{2}", Reason, NewKeepAlive, ShutdownRequest);
    }
}
