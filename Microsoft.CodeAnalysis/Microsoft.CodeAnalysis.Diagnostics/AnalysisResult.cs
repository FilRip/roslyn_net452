using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics.Telemetry;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public class AnalysisResult
    {
        public ImmutableArray<DiagnosticAnalyzer> Analyzers { get; }

        public ImmutableDictionary<SyntaxTree, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> SyntaxDiagnostics { get; }

        public ImmutableDictionary<SyntaxTree, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> SemanticDiagnostics { get; }

        public ImmutableDictionary<AdditionalText, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> AdditionalFileDiagnostics { get; }

        public ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>> CompilationDiagnostics { get; }

        public ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo> AnalyzerTelemetryInfo { get; }

        internal AnalysisResult(ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableDictionary<SyntaxTree, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> localSyntaxDiagnostics, ImmutableDictionary<SyntaxTree, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> localSemanticDiagnostics, ImmutableDictionary<AdditionalText, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> localAdditionalFileDiagnostics, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>> nonLocalDiagnostics, ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo> analyzerTelemetryInfo)
        {
            Analyzers = analyzers;
            SyntaxDiagnostics = localSyntaxDiagnostics;
            SemanticDiagnostics = localSemanticDiagnostics;
            AdditionalFileDiagnostics = localAdditionalFileDiagnostics;
            CompilationDiagnostics = nonLocalDiagnostics;
            AnalyzerTelemetryInfo = analyzerTelemetryInfo;
        }

        public ImmutableArray<Diagnostic> GetAllDiagnostics(DiagnosticAnalyzer analyzer)
        {
            if (!Analyzers.Contains(analyzer))
            {
                throw new ArgumentException(CodeAnalysisResources.UnsupportedAnalyzerInstance, "analyzer");
            }
            return GetDiagnostics(SpecializedCollections.SingletonEnumerable(analyzer));
        }

        public ImmutableArray<Diagnostic> GetAllDiagnostics()
        {
            return GetDiagnostics(Analyzers);
        }

        private ImmutableArray<Diagnostic> GetDiagnostics(IEnumerable<DiagnosticAnalyzer> analyzers)
        {
            IEnumerable<DiagnosticAnalyzer> source = Analyzers.Except<DiagnosticAnalyzer>(analyzers);
            ImmutableHashSet<DiagnosticAnalyzer> excludedAnalyzers = (source.Any() ? source.ToImmutableHashSet() : ImmutableHashSet<DiagnosticAnalyzer>.Empty);
            return GetDiagnostics(excludedAnalyzers);
        }

        private ImmutableArray<Diagnostic> GetDiagnostics(ImmutableHashSet<DiagnosticAnalyzer> excludedAnalyzers)
        {
            if (SyntaxDiagnostics.Count > 0 || SemanticDiagnostics.Count > 0 || AdditionalFileDiagnostics.Count > 0 || CompilationDiagnostics.Count > 0)
            {
                ImmutableArray<Diagnostic>.Builder builder = ImmutableArray.CreateBuilder<Diagnostic>();
                AddLocalDiagnostics(SyntaxDiagnostics, excludedAnalyzers, builder);
                AddLocalDiagnostics(SemanticDiagnostics, excludedAnalyzers, builder);
                AddLocalDiagnostics(AdditionalFileDiagnostics, excludedAnalyzers, builder);
                AddNonLocalDiagnostics(CompilationDiagnostics, excludedAnalyzers, builder);
                return builder.ToImmutable();
            }
            return ImmutableArray<Diagnostic>.Empty;
        }

        private static void AddLocalDiagnostics<T>(ImmutableDictionary<T, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> localDiagnostics, ImmutableHashSet<DiagnosticAnalyzer> excludedAnalyzers, ImmutableArray<Diagnostic>.Builder builder) where T : notnull
        {
            foreach (KeyValuePair<T, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> localDiagnostic in localDiagnostics)
            {
                foreach (KeyValuePair<DiagnosticAnalyzer, ImmutableArray<Diagnostic>> item in localDiagnostic.Value)
                {
                    if (!excludedAnalyzers.Contains(item.Key))
                    {
                        builder.AddRange(item.Value);
                    }
                }
            }
        }

        private static void AddNonLocalDiagnostics(ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>> nonLocalDiagnostics, ImmutableHashSet<DiagnosticAnalyzer> excludedAnalyzers, ImmutableArray<Diagnostic>.Builder builder)
        {
            foreach (KeyValuePair<DiagnosticAnalyzer, ImmutableArray<Diagnostic>> nonLocalDiagnostic in nonLocalDiagnostics)
            {
                if (!excludedAnalyzers.Contains(nonLocalDiagnostic.Key))
                {
                    builder.AddRange(nonLocalDiagnostic.Value);
                }
            }
        }
    }
}
