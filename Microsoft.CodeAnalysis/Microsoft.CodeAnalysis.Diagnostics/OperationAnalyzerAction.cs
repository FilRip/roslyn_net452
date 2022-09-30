using System;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class OperationAnalyzerAction : AnalyzerAction
    {
        public Action<OperationAnalysisContext> Action { get; }

        public ImmutableArray<OperationKind> Kinds { get; }

        public OperationAnalyzerAction(Action<OperationAnalysisContext> action, ImmutableArray<OperationKind> kinds, DiagnosticAnalyzer analyzer)
            : base(analyzer)
        {
            Action = action;
            Kinds = kinds;
        }
    }
}
