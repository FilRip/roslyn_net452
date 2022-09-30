using System.Threading;

namespace Microsoft.CodeAnalysis
{
    public abstract class SyntaxTreeOptionsProvider
    {
        public abstract GeneratedKind IsGenerated(SyntaxTree tree, CancellationToken cancellationToken);

        public abstract bool TryGetDiagnosticValue(SyntaxTree tree, string diagnosticId, CancellationToken cancellationToken, out ReportDiagnostic severity);

        public abstract bool TryGetGlobalDiagnosticValue(string diagnosticId, CancellationToken cancellationToken, out ReportDiagnostic severity);
    }
}
