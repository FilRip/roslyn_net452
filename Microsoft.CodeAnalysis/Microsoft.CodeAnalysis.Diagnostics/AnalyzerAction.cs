namespace Microsoft.CodeAnalysis.Diagnostics
{
    public abstract class AnalyzerAction
    {
        public DiagnosticAnalyzer Analyzer { get; }

        public AnalyzerAction(DiagnosticAnalyzer analyzer)
        {
            Analyzer = analyzer;
        }
    }
}
