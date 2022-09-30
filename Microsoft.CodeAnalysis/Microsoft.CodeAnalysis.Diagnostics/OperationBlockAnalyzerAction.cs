using System;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class OperationBlockAnalyzerAction : AnalyzerAction
    {
        public Action<OperationBlockAnalysisContext> Action { get; }

        public OperationBlockAnalyzerAction(Action<OperationBlockAnalysisContext> action, DiagnosticAnalyzer analyzer)
            : base(analyzer)
        {
            Action = action;
        }
    }
}
