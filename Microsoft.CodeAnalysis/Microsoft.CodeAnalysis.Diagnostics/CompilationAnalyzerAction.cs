using System;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class CompilationAnalyzerAction : AnalyzerAction
    {
        public Action<CompilationAnalysisContext> Action { get; }

        public CompilationAnalyzerAction(Action<CompilationAnalysisContext> action, DiagnosticAnalyzer analyzer)
            : base(analyzer)
        {
            Action = action;
        }
    }
}
