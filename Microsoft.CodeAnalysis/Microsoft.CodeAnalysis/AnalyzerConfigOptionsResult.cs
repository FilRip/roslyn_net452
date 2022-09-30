using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis
{
    public readonly struct AnalyzerConfigOptionsResult
    {
        public ImmutableDictionary<string, ReportDiagnostic> TreeOptions { get; }

        public ImmutableDictionary<string, string> AnalyzerOptions { get; }

        public ImmutableArray<Diagnostic> Diagnostics { get; }

        internal AnalyzerConfigOptionsResult(ImmutableDictionary<string, ReportDiagnostic> treeOptions, ImmutableDictionary<string, string> analyzerOptions, ImmutableArray<Diagnostic> diagnostics)
        {
            TreeOptions = treeOptions;
            AnalyzerOptions = analyzerOptions;
            Diagnostics = diagnostics;
        }
    }
}
