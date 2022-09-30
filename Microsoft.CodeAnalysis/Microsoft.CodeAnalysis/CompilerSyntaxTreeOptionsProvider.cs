using System.Collections.Immutable;
using System.Threading;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class CompilerSyntaxTreeOptionsProvider : SyntaxTreeOptionsProvider
    {
        private readonly struct Options
        {
            public readonly GeneratedKind IsGenerated;

            public readonly ImmutableDictionary<string, ReportDiagnostic> DiagnosticOptions;

            public Options(AnalyzerConfigOptionsResult? result)
            {
                if (result.HasValue)
                {
                    AnalyzerConfigOptionsResult valueOrDefault = result.GetValueOrDefault();
                    DiagnosticOptions = valueOrDefault.TreeOptions;
                    IsGenerated = GeneratedCodeUtilities.GetIsGeneratedCodeFromOptions(valueOrDefault.AnalyzerOptions);
                }
                else
                {
                    DiagnosticOptions = SyntaxTree.EmptyDiagnosticOptions;
                    IsGenerated = GeneratedKind.Unknown;
                }
            }
        }

        private readonly ImmutableDictionary<SyntaxTree, Options> _options;

        private readonly AnalyzerConfigOptionsResult _globalOptions;

        public CompilerSyntaxTreeOptionsProvider(SyntaxTree?[] trees, ImmutableArray<AnalyzerConfigOptionsResult> results, AnalyzerConfigOptionsResult globalResults)
        {
            ImmutableDictionary<SyntaxTree, Options>.Builder builder = ImmutableDictionary.CreateBuilder<SyntaxTree, Options>();
            for (int i = 0; i < trees.Length; i++)
            {
                if (trees[i] != null)
                {
                    builder.Add(trees[i], new Options(results.IsDefault ? null : new AnalyzerConfigOptionsResult?(results[i])));
                }
            }
            _options = builder.ToImmutableDictionary();
            _globalOptions = globalResults;
        }

        public override GeneratedKind IsGenerated(SyntaxTree tree, CancellationToken _)
        {
            if (!_options.TryGetValue(tree, out var value))
            {
                return GeneratedKind.Unknown;
            }
            return value.IsGenerated;
        }

        public override bool TryGetDiagnosticValue(SyntaxTree tree, string diagnosticId, CancellationToken _, out ReportDiagnostic severity)
        {
            if (_options.TryGetValue(tree, out var value))
            {
                return value.DiagnosticOptions.TryGetValue(diagnosticId, out severity);
            }
            severity = ReportDiagnostic.Default;
            return false;
        }

        public override bool TryGetGlobalDiagnosticValue(string diagnosticId, CancellationToken _, out ReportDiagnostic severity)
        {
            if (_globalOptions.TreeOptions != null)
            {
                return _globalOptions.TreeOptions.TryGetValue(diagnosticId, out severity);
            }
            severity = ReportDiagnostic.Default;
            return false;
        }
    }
}
