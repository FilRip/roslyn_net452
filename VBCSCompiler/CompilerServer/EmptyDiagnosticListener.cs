using System;

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal sealed class EmptyDiagnosticListener : IDiagnosticListener
    {
        public void UpdateKeepAlive(TimeSpan keepAlive)
        {
        }

        public void ConnectionReceived()
        {
        }

        public void ConnectionCompleted(CompletionData completionData)
        {
        }

        public void KeepAliveReached()
        {
        }
    }
}
