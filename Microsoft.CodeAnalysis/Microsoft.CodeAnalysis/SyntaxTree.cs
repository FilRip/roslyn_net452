using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class SyntaxTree
    {
        protected internal static readonly ImmutableDictionary<string, ReportDiagnostic> EmptyDiagnosticOptions = ImmutableDictionary.Create<string, ReportDiagnostic>(CaseInsensitiveComparison.Comparer);

        private ImmutableArray<byte> _lazyChecksum;

        private SourceHashAlgorithm _lazyHashAlgorithm;

        public abstract string FilePath { get; }

        public abstract bool HasCompilationUnitRoot { get; }

        public ParseOptions Options => OptionsCore;

        protected abstract ParseOptions OptionsCore { get; }

        [Obsolete("Obsolete due to performance problems, use CompilationOptions.SyntaxTreeOptionsProvider instead", false)]
        public virtual ImmutableDictionary<string, ReportDiagnostic> DiagnosticOptions => EmptyDiagnosticOptions;

        public abstract int Length { get; }

        public abstract Encoding? Encoding { get; }

        public virtual bool SupportsLocations => HasCompilationUnitRoot;

        public abstract bool TryGetText([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out SourceText? text);

        public abstract SourceText GetText(CancellationToken cancellationToken = default(CancellationToken));

        public virtual Task<SourceText> GetTextAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(TryGetText(out SourceText text) ? text : GetText(cancellationToken));
        }

        public bool TryGetRoot([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out SyntaxNode? root)
        {
            return TryGetRootCore(out root);
        }

        protected abstract bool TryGetRootCore([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out SyntaxNode? root);

        public SyntaxNode GetRoot(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetRootCore(cancellationToken);
        }

        protected abstract SyntaxNode GetRootCore(CancellationToken cancellationToken);

        public Task<SyntaxNode> GetRootAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetRootAsyncCore(cancellationToken);
        }

        protected abstract Task<SyntaxNode> GetRootAsyncCore(CancellationToken cancellationToken);

        public abstract SyntaxTree WithChangedText(SourceText newText);

        public abstract IEnumerable<Diagnostic> GetDiagnostics(CancellationToken cancellationToken = default(CancellationToken));

        public abstract IEnumerable<Diagnostic> GetDiagnostics(SyntaxNode node);

        public abstract IEnumerable<Diagnostic> GetDiagnostics(SyntaxToken token);

        public abstract IEnumerable<Diagnostic> GetDiagnostics(SyntaxTrivia trivia);

        public abstract IEnumerable<Diagnostic> GetDiagnostics(SyntaxNodeOrToken nodeOrToken);

        public abstract FileLinePositionSpan GetLineSpan(TextSpan span, CancellationToken cancellationToken = default(CancellationToken));

        public abstract FileLinePositionSpan GetMappedLineSpan(TextSpan span, CancellationToken cancellationToken = default(CancellationToken));

        public virtual LineVisibility GetLineVisibility(int position, CancellationToken cancellationToken = default(CancellationToken))
        {
            return LineVisibility.Visible;
        }

        public virtual FileLinePositionSpan GetMappedLineSpanAndVisibility(TextSpan span, out bool isHiddenPosition)
        {
            isHiddenPosition = GetLineVisibility(span.Start) == LineVisibility.Hidden;
            return GetMappedLineSpan(span);
        }

        public string GetDisplayPath(TextSpan span, SourceReferenceResolver? resolver)
        {
            FileLinePositionSpan mappedLineSpan = GetMappedLineSpan(span);
            if (resolver == null || mappedLineSpan.Path.IsEmpty())
            {
                return mappedLineSpan.Path;
            }
            return resolver!.NormalizePath(mappedLineSpan.Path, mappedLineSpan.HasMappedPath ? FilePath : null) ?? mappedLineSpan.Path;
        }

        public int GetDisplayLineNumber(TextSpan span)
        {
            return GetMappedLineSpan(span).StartLinePosition.Line + 1;
        }

        public abstract bool HasHiddenRegions();

        public abstract IList<TextSpan> GetChangedSpans(SyntaxTree syntaxTree);

        public abstract Location GetLocation(TextSpan span);

        public abstract bool IsEquivalentTo(SyntaxTree tree, bool topLevel = false);

        public abstract SyntaxReference GetReference(SyntaxNode node);

        public abstract IList<TextChange> GetChanges(SyntaxTree oldTree);

        internal DebugSourceInfo GetDebugSourceInfo()
        {
            if (_lazyChecksum.IsDefault)
            {
                SourceText text = GetText();
                _lazyChecksum = text.GetChecksum();
                _lazyHashAlgorithm = text.ChecksumAlgorithm;
            }
            return new DebugSourceInfo(_lazyChecksum, _lazyHashAlgorithm);
        }

        public abstract SyntaxTree WithRootAndOptions(SyntaxNode root, ParseOptions options);

        public abstract SyntaxTree WithFilePath(string path);

        [Obsolete("Obsolete due to performance problems, use CompilationOptions.SyntaxTreeOptionsProvider instead", false)]
        public virtual SyntaxTree WithDiagnosticOptions(ImmutableDictionary<string, ReportDiagnostic> options)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return GetText(CancellationToken.None).ToString();
        }
    }
}
