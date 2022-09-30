using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis
{
    public readonly struct GeneratorPostInitializationContext
    {
        private readonly AdditionalSourcesCollection _additionalSources;

        public CancellationToken CancellationToken { get; }

        internal GeneratorPostInitializationContext(AdditionalSourcesCollection additionalSources, CancellationToken cancellationToken)
        {
            _additionalSources = additionalSources;
            CancellationToken = cancellationToken;
        }

        public void AddSource(string hintName, string source)
        {
            AddSource(hintName, SourceText.From(source, Encoding.UTF8));
        }

        public void AddSource(string hintName, SourceText sourceText)
        {
            _additionalSources.Add(hintName, sourceText);
        }
    }
}
