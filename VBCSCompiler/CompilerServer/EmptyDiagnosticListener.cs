using System;

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal sealed class EmptyDiagnosticListener : IDiagnosticListener
    {
        public void UpdateKeepAlive(TimeSpan keepAlive)
        {
            // Nothing here
        }

        public void ConnectionReceived()
        {
            // Nothing here
        }

        public void ConnectionCompleted(CompletionData completionData)
        {
            // Nothing here
        }

        public void KeepAliveReached()
        {
            // Nothing here
        }
    }
}
