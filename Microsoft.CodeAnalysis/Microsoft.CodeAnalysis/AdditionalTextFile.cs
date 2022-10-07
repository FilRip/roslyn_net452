using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class AdditionalTextFile : AdditionalText
    {
        private readonly CommandLineSourceFile _sourceFile;

        private readonly CommonCompiler _compiler;

        private readonly Lazy<SourceText?> _text;

        private IList<DiagnosticInfo> _diagnostics;

        public override string Path => _sourceFile.Path;

        internal IList<DiagnosticInfo> Diagnostics => _diagnostics;

        public AdditionalTextFile(CommandLineSourceFile sourceFile, CommonCompiler compiler)
        {
            _sourceFile = sourceFile;
            _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
            _diagnostics = SpecializedCollections.EmptyList<DiagnosticInfo>();
            _text = new Lazy<SourceText>(ReadText);
        }

        private SourceText? ReadText()
        {
            List<DiagnosticInfo> diagnostics = new();
            SourceText? result = _compiler.TryReadFileContent(_sourceFile, diagnostics);
            _diagnostics = diagnostics;
            return result;
        }

        public override SourceText? GetText(CancellationToken cancellationToken = default)
        {
            return _text.Value;
        }
    }
}
