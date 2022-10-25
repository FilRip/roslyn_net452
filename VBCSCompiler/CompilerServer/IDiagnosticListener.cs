using System;

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal interface IDiagnosticListener
    {
        void UpdateKeepAlive(TimeSpan keepAlive);

        void ConnectionReceived();

        void ConnectionCompleted(CompletionData completionData);

        void KeepAliveReached();
    }
}
