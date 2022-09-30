using System;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class CompilationStartAnalyzerAction : AnalyzerAction
    {
        public Action<CompilationStartAnalysisContext> Action { get; }

        public CompilationStartAnalyzerAction(Action<CompilationStartAnalysisContext> action, DiagnosticAnalyzer analyzer)
            : base(analyzer)
        {
            Action = action;
        }
    }
}
