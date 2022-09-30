using System;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public readonly struct GeneratorRunResult
    {
        public ISourceGenerator Generator { get; }

        public ImmutableArray<GeneratedSourceResult> GeneratedSources { get; }

        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public Exception? Exception { get; }

        internal GeneratorRunResult(ISourceGenerator generator, ImmutableArray<GeneratedSourceResult> generatedSources, ImmutableArray<Diagnostic> diagnostics, Exception? exception)
        {
            Generator = generator;
            GeneratedSources = generatedSources;
            Diagnostics = diagnostics;
            Exception = exception;
        }
    }
}
