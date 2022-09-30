namespace Microsoft.CodeAnalysis.Diagnostics
{
    internal static class SeverityFilterExtensions
    {
        internal static bool Contains(this SeverityFilter severityFilter, ReportDiagnostic severity)
        {
            return severity switch
            {
                ReportDiagnostic.Hidden => (severityFilter & SeverityFilter.Hidden) != 0,
                ReportDiagnostic.Info => (severityFilter & SeverityFilter.Info) != 0,
                _ => false,
            };
        }
    }
}
