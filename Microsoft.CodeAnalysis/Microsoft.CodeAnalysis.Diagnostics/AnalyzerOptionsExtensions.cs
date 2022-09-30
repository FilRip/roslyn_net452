using System.Threading;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    internal static class AnalyzerOptionsExtensions
    {
        private const string DotnetAnalyzerDiagnosticPrefix = "dotnet_analyzer_diagnostic";

        private const string CategoryPrefix = "category";

        private const string SeveritySuffix = "severity";

        private const string DotnetAnalyzerDiagnosticSeverityKey = "dotnet_analyzer_diagnostic.severity";

        private static string GetCategoryBasedDotnetAnalyzerDiagnosticSeverityKey(string category)
        {
            return "dotnet_analyzer_diagnostic.category-" + category + ".severity";
        }

        public static bool TryGetSeverityFromBulkConfiguration(this AnalyzerOptions? analyzerOptions, SyntaxTree tree, Compilation compilation, DiagnosticDescriptor descriptor, CancellationToken cancellationToken, out ReportDiagnostic severity)
        {
            if (analyzerOptions == null || !descriptor.IsEnabledByDefault || descriptor.CustomTags.Contains((string tag) => tag == "Compiler" || tag == "NotConfigurable"))
            {
                severity = ReportDiagnostic.Default;
                return false;
            }
            if (!compilation.Options.SpecificDiagnosticOptions.ContainsKey(descriptor.Id))
            {
                SyntaxTreeOptionsProvider? syntaxTreeOptionsProvider = compilation.Options.SyntaxTreeOptionsProvider;
                if (syntaxTreeOptionsProvider == null || !syntaxTreeOptionsProvider!.TryGetDiagnosticValue(tree, descriptor.Id, cancellationToken, out var severity2))
                {
                    SyntaxTreeOptionsProvider? syntaxTreeOptionsProvider2 = compilation.Options.SyntaxTreeOptionsProvider;
                    if (syntaxTreeOptionsProvider2 == null || !syntaxTreeOptionsProvider2!.TryGetGlobalDiagnosticValue(descriptor.Id, cancellationToken, out severity2))
                    {
                        AnalyzerConfigOptions options = analyzerOptions!.AnalyzerConfigOptionsProvider.GetOptions(tree);
                        string categoryBasedDotnetAnalyzerDiagnosticSeverityKey = GetCategoryBasedDotnetAnalyzerDiagnosticSeverityKey(descriptor.Category);
                        if (options.TryGetValue(categoryBasedDotnetAnalyzerDiagnosticSeverityKey, out var value) && AnalyzerConfigSet.TryParseSeverity(value, out severity))
                        {
                            return true;
                        }
                        if (options.TryGetValue("dotnet_analyzer_diagnostic.severity", out value) && AnalyzerConfigSet.TryParseSeverity(value, out severity))
                        {
                            return true;
                        }
                        severity = ReportDiagnostic.Default;
                        return false;
                    }
                }
            }
            severity = ReportDiagnostic.Default;
            return false;
        }
    }
}
