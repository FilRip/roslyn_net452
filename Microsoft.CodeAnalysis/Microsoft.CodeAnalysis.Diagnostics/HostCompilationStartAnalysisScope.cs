namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class HostCompilationStartAnalysisScope : HostAnalysisScope
    {
        private readonly HostSessionStartAnalysisScope _sessionScope;

        public HostCompilationStartAnalysisScope(HostSessionStartAnalysisScope sessionScope)
        {
            _sessionScope = sessionScope;
        }

        public override AnalyzerActions GetAnalyzerActions(DiagnosticAnalyzer analyzer)
        {
            AnalyzerActions analyzerActions = base.GetAnalyzerActions(analyzer);
            AnalyzerActions otherActions = _sessionScope.GetAnalyzerActions(analyzer);
            if (otherActions.IsEmpty)
            {
                return analyzerActions;
            }
            if (analyzerActions.IsEmpty)
            {
                return otherActions;
            }
            return analyzerActions.Append(in otherActions);
        }
    }
}
