using System;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class OperationBlockStartAnalyzerAction : AnalyzerAction
    {
        public Action<OperationBlockStartAnalysisContext> Action { get; }

        public OperationBlockStartAnalyzerAction(Action<OperationBlockStartAnalysisContext> action, DiagnosticAnalyzer analyzer)
            : base(analyzer)
        {
            Action = action;
        }
    }
}
