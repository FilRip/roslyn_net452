using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

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
            if (compiler == null)
            {
                throw new ArgumentNullException("compiler");
            }
            _sourceFile = sourceFile;
            _compiler = compiler;
            _diagnostics = SpecializedCollections.EmptyList<DiagnosticInfo>();
            _text = new Lazy<SourceText>(ReadText);
        }

        private SourceText? ReadText()
        {
            List<DiagnosticInfo> diagnostics = new List<DiagnosticInfo>();
            SourceText? result = _compiler.TryReadFileContent(_sourceFile, diagnostics);
            _diagnostics = diagnostics;
            return result;
        }

        public override SourceText? GetText(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _text.Value;
        }
    }
}
