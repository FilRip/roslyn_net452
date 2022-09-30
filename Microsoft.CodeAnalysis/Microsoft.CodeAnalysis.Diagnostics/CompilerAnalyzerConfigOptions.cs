using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    internal sealed class CompilerAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly ImmutableDictionary<string, string> _backing;

        public static CompilerAnalyzerConfigOptions Empty { get; } = new CompilerAnalyzerConfigOptions(AnalyzerConfigOptions.EmptyDictionary);


        public CompilerAnalyzerConfigOptions(ImmutableDictionary<string, string> properties)
        {
            _backing = properties;
        }

        public override bool TryGetValue(string key, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? value)
        {
            return _backing.TryGetValue(key, out value);
        }
    }
}
