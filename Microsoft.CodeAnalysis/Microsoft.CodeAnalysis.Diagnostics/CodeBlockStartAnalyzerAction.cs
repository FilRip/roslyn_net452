using System;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class CodeBlockStartAnalyzerAction<TLanguageKindEnum> : AnalyzerAction where TLanguageKindEnum : struct
    {
        public Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> Action { get; }

        public CodeBlockStartAnalyzerAction(Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> action, DiagnosticAnalyzer analyzer)
            : base(analyzer)
        {
            Action = action;
        }
    }
}
