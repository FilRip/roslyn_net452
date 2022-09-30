using System;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class SymbolStartAnalyzerAction : AnalyzerAction
    {
        public Action<SymbolStartAnalysisContext> Action { get; }

        public SymbolKind Kind { get; }

        public SymbolStartAnalyzerAction(Action<SymbolStartAnalysisContext> action, SymbolKind kind, DiagnosticAnalyzer analyzer)
            : base(analyzer)
        {
            Action = action;
            Kind = kind;
        }
    }
}
