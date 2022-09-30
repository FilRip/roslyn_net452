using System;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class SymbolAnalyzerAction : AnalyzerAction
    {
        public Action<SymbolAnalysisContext> Action { get; }

        public ImmutableArray<SymbolKind> Kinds { get; }

        public SymbolAnalyzerAction(Action<SymbolAnalysisContext> action, ImmutableArray<SymbolKind> kinds, DiagnosticAnalyzer analyzer)
            : base(analyzer)
        {
            Action = action;
            Kinds = kinds;
        }
    }
}
