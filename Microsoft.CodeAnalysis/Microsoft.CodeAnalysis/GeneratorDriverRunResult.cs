using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.CodeAnalysis
{
    public class GeneratorDriverRunResult
    {
        private ImmutableArray<Diagnostic> _lazyDiagnostics;

        private ImmutableArray<SyntaxTree> _lazyGeneratedTrees;

        public ImmutableArray<GeneratorRunResult> Results { get; }

        public ImmutableArray<Diagnostic> Diagnostics
        {
            get
            {
                if (_lazyDiagnostics.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _lazyDiagnostics, Results.SelectMany((GeneratorRunResult r) => r.Diagnostics).ToImmutableArray());
                }
                return _lazyDiagnostics;
            }
        }

        public ImmutableArray<SyntaxTree> GeneratedTrees
        {
            get
            {
                if (_lazyGeneratedTrees.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _lazyGeneratedTrees, Results.SelectMany((GeneratorRunResult r) => r.GeneratedSources.Select((GeneratedSourceResult g) => g.SyntaxTree)).ToImmutableArray());
                }
                return _lazyGeneratedTrees;
            }
        }

        internal GeneratorDriverRunResult(ImmutableArray<GeneratorRunResult> results)
        {
            Results = results;
        }
    }
}
