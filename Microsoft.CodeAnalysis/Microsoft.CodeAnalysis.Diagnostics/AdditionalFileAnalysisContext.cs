using System;
using System.Threading;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public readonly struct AdditionalFileAnalysisContext
    {
        private readonly Action<Diagnostic> _reportDiagnostic;

        private readonly Func<Diagnostic, bool> _isSupportedDiagnostic;

        public AdditionalText AdditionalFile { get; }

        public AnalyzerOptions Options { get; }

        public CancellationToken CancellationToken { get; }

        public Compilation Compilation { get; }

        internal AdditionalFileAnalysisContext(AdditionalText additionalFile, AnalyzerOptions options, Action<Diagnostic> reportDiagnostic, Func<Diagnostic, bool> isSupportedDiagnostic, Compilation compilation, CancellationToken cancellationToken)
        {
            AdditionalFile = additionalFile;
            Options = options;
            _reportDiagnostic = reportDiagnostic;
            _isSupportedDiagnostic = isSupportedDiagnostic;
            Compilation = compilation;
            CancellationToken = cancellationToken;
        }

        public void ReportDiagnostic(Diagnostic diagnostic)
        {
            DiagnosticAnalysisContextHelpers.VerifyArguments(diagnostic, Compilation, _isSupportedDiagnostic);
            lock (_reportDiagnostic)
            {
                _reportDiagnostic(diagnostic);
            }
        }
    }
}
