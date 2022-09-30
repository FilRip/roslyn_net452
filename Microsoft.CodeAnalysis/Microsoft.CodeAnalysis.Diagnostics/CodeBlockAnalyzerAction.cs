using System;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class CodeBlockAnalyzerAction : AnalyzerAction
    {
        public Action<CodeBlockAnalysisContext> Action { get; }

        public CodeBlockAnalyzerAction(Action<CodeBlockAnalysisContext> action, DiagnosticAnalyzer analyzer)
            : base(analyzer)
        {
            Action = action;
        }
    }
}
