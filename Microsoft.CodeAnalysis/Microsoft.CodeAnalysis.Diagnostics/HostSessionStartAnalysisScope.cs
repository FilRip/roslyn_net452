using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class HostSessionStartAnalysisScope : HostAnalysisScope
    {
        private ImmutableHashSet<DiagnosticAnalyzer> _concurrentAnalyzers = ImmutableHashSet<DiagnosticAnalyzer>.Empty;

        private readonly ConcurrentDictionary<DiagnosticAnalyzer, GeneratedCodeAnalysisFlags> _generatedCodeConfigurationMap = new ConcurrentDictionary<DiagnosticAnalyzer, GeneratedCodeAnalysisFlags>();

        public bool IsConcurrentAnalyzer(DiagnosticAnalyzer analyzer)
        {
            return _concurrentAnalyzers.Contains(analyzer);
        }

        public GeneratedCodeAnalysisFlags GetGeneratedCodeAnalysisFlags(DiagnosticAnalyzer analyzer)
        {
            if (!_generatedCodeConfigurationMap.TryGetValue(analyzer, out var value))
            {
                return GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics;
            }
            return value;
        }

        public void RegisterCompilationStartAction(DiagnosticAnalyzer analyzer, Action<CompilationStartAnalysisContext> action)
        {
            CompilationStartAnalyzerAction action2 = new CompilationStartAnalyzerAction(action, analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.AddCompilationStartAction(action2);
        }

        public void EnableConcurrentExecution(DiagnosticAnalyzer analyzer)
        {
            _concurrentAnalyzers = _concurrentAnalyzers.Add(analyzer);
            GetOrCreateAnalyzerActions(analyzer).Value.EnableConcurrentExecution();
        }

        public void ConfigureGeneratedCodeAnalysis(DiagnosticAnalyzer analyzer, GeneratedCodeAnalysisFlags mode)
        {
            _generatedCodeConfigurationMap.AddOrUpdate(analyzer, mode, (DiagnosticAnalyzer a, GeneratedCodeAnalysisFlags c) => mode);
        }
    }
}
