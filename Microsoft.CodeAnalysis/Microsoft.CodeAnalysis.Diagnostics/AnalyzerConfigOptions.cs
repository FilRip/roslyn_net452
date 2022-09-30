using System;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public abstract class AnalyzerConfigOptions
    {
        internal static ImmutableDictionary<string, string> EmptyDictionary = ImmutableDictionary.Create<string, string>(KeyComparer);

        public static StringComparer KeyComparer { get; } = AnalyzerConfig.Section.PropertiesKeyComparer;


        public abstract bool TryGetValue(string key, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? value);
    }
}
