using System.Collections.Immutable;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public readonly struct GeneratorExecutionContext
    {
        private readonly DiagnosticBag _diagnostics;

        private readonly AdditionalSourcesCollection _additionalSources;

        public Compilation Compilation { get; }

        public ParseOptions ParseOptions { get; }

        public ImmutableArray<AdditionalText> AdditionalFiles { get; }

        public AnalyzerConfigOptionsProvider AnalyzerConfigOptions { get; }

        public ISyntaxReceiver? SyntaxReceiver { get; }

        public ISyntaxContextReceiver? SyntaxContextReceiver { get; }

        public CancellationToken CancellationToken { get; }

        internal GeneratorExecutionContext(Compilation compilation, ParseOptions parseOptions, ImmutableArray<AdditionalText> additionalTexts, AnalyzerConfigOptionsProvider optionsProvider, ISyntaxContextReceiver? syntaxReceiver, AdditionalSourcesCollection additionalSources, CancellationToken cancellationToken = default(CancellationToken))
        {
            Compilation = compilation;
            ParseOptions = parseOptions;
            AdditionalFiles = additionalTexts;
            AnalyzerConfigOptions = optionsProvider;
            SyntaxReceiver = (syntaxReceiver as SyntaxContextReceiverAdaptor)?.Receiver;
            SyntaxContextReceiver = ((syntaxReceiver is SyntaxContextReceiverAdaptor) ? null : syntaxReceiver);
            CancellationToken = cancellationToken;
            _additionalSources = additionalSources;
            _diagnostics = new DiagnosticBag();
        }

        public void AddSource(string hintName, string source)
        {
            AddSource(hintName, SourceText.From(source, Encoding.UTF8));
        }

        public void AddSource(string hintName, SourceText sourceText)
        {
            _additionalSources.Add(hintName, sourceText);
        }

        public void ReportDiagnostic(Diagnostic diagnostic)
        {
            _diagnostics.Add(diagnostic);
        }

        internal (ImmutableArray<GeneratedSourceText> sources, ImmutableArray<Diagnostic> diagnostics) ToImmutableAndFree()
        {
            return (_additionalSources.ToImmutableAndFree(), _diagnostics.ToReadOnlyAndFree());
        }
    }
}
