using System;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public abstract class AnalyzerReference
    {
        public abstract string? FullPath { get; }

        public virtual string Display => string.Empty;

        public abstract object Id { get; }

        public abstract ImmutableArray<DiagnosticAnalyzer> GetAnalyzersForAllLanguages();

        public abstract ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(string language);

        public virtual ImmutableArray<ISourceGenerator> GetGeneratorsForAllLanguages()
        {
            return ImmutableArray<ISourceGenerator>.Empty;
        }

        [Obsolete("Use GetGenerators(string language) or GetGeneratorsForAllLanguages()")]
        public virtual ImmutableArray<ISourceGenerator> GetGenerators()
        {
            return ImmutableArray<ISourceGenerator>.Empty;
        }

        public virtual ImmutableArray<ISourceGenerator> GetGenerators(string language)
        {
            return ImmutableArray<ISourceGenerator>.Empty;
        }
    }
}
