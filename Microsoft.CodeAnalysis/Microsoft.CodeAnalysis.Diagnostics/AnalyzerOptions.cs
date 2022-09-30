using System;
using System.Collections.Immutable;
using System.Linq;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public class AnalyzerOptions
    {
        internal static readonly AnalyzerOptions Empty = new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty);

        public ImmutableArray<AdditionalText> AdditionalFiles { get; }

        public AnalyzerConfigOptionsProvider AnalyzerConfigOptionsProvider { get; }

        public AnalyzerOptions(ImmutableArray<AdditionalText> additionalFiles, AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (optionsProvider == null)
            {
                throw new ArgumentNullException("optionsProvider");
            }
            AdditionalFiles = additionalFiles.NullToEmpty();
            AnalyzerConfigOptionsProvider = optionsProvider;
        }

        public AnalyzerOptions(ImmutableArray<AdditionalText> additionalFiles)
            : this(additionalFiles, CompilerAnalyzerConfigOptionsProvider.Empty)
        {
        }

        public AnalyzerOptions WithAdditionalFiles(ImmutableArray<AdditionalText> additionalFiles)
        {
            if (AdditionalFiles == additionalFiles)
            {
                return this;
            }
            return new AnalyzerOptions(additionalFiles);
        }

        public override bool Equals(object? obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj is AnalyzerOptions analyzerOptions)
            {
                if (!(AdditionalFiles == analyzerOptions.AdditionalFiles))
                {
                    return AdditionalFiles.SequenceEqual(analyzerOptions.AdditionalFiles, object.ReferenceEquals);
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.CombineValues(AdditionalFiles);
        }
    }
}
