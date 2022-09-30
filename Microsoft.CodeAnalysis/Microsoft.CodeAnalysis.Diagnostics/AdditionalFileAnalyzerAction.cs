using System;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class AdditionalFileAnalyzerAction : AnalyzerAction
    {
        public Action<AdditionalFileAnalysisContext> Action { get; }

        public AdditionalFileAnalyzerAction(Action<AdditionalFileAnalysisContext> action, DiagnosticAnalyzer analyzer)
            : base(analyzer)
        {
            Action = action;
        }
    }
}
