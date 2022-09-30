using System;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class SyntaxTreeAnalyzerAction : AnalyzerAction
    {
        public Action<SyntaxTreeAnalysisContext> Action { get; }

        public SyntaxTreeAnalyzerAction(Action<SyntaxTreeAnalysisContext> action, DiagnosticAnalyzer analyzer)
            : base(analyzer)
        {
            Action = action;
        }
    }
}
