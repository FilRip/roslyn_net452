using System;
using System.Collections.Immutable;
using System.Threading;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public struct SuppressionAnalysisContext
    {
        private readonly Action<Suppression> _addSuppression;

        private readonly Func<SuppressionDescriptor, bool> _isSupportedSuppressionDescriptor;

        private readonly Func<SyntaxTree, SemanticModel> _getSemanticModel;

        public ImmutableArray<Diagnostic> ReportedDiagnostics { get; }

        public Compilation Compilation { get; }

        public AnalyzerOptions Options { get; }

        public CancellationToken CancellationToken { get; }

        internal SuppressionAnalysisContext(Compilation compilation, AnalyzerOptions options, ImmutableArray<Diagnostic> reportedDiagnostics, Action<Suppression> suppressDiagnostic, Func<SuppressionDescriptor, bool> isSupportedSuppressionDescriptor, Func<SyntaxTree, SemanticModel> getSemanticModel, CancellationToken cancellationToken)
        {
            Compilation = compilation;
            Options = options;
            ReportedDiagnostics = reportedDiagnostics;
            _addSuppression = suppressDiagnostic;
            _isSupportedSuppressionDescriptor = isSupportedSuppressionDescriptor;
            _getSemanticModel = getSemanticModel;
            CancellationToken = cancellationToken;
        }

        public void ReportSuppression(Suppression suppression)
        {
            if (!ReportedDiagnostics.Contains(suppression.SuppressedDiagnostic))
            {
                throw new ArgumentException(string.Format(CodeAnalysisResources.NonReportedDiagnosticCannotBeSuppressed, suppression.SuppressedDiagnostic.Id));
            }
            if (!_isSupportedSuppressionDescriptor(suppression.Descriptor))
            {
                throw new ArgumentException(string.Format(CodeAnalysisResources.UnsupportedSuppressionReported, suppression.Descriptor.Id));
            }
            if (!suppression.Descriptor.IsDisabled(Compilation.Options))
            {
                _addSuppression(suppression);
            }
        }

        public SemanticModel GetSemanticModel(SyntaxTree syntaxTree)
        {
            return _getSemanticModel(syntaxTree);
        }
    }
}
