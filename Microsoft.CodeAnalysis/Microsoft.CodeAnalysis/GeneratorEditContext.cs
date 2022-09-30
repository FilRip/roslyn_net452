using System.Threading;

namespace Microsoft.CodeAnalysis
{
    public readonly struct GeneratorEditContext
    {
        public CancellationToken CancellationToken { get; }

        public AdditionalSourcesCollection AdditionalSources { get; }

        public GeneratorEditContext(AdditionalSourcesCollection sources, CancellationToken cancellationToken = default(CancellationToken))
        {
            AdditionalSources = sources;
            CancellationToken = cancellationToken;
        }
    }
}
