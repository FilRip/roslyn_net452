using System.Collections.Immutable;

using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.CodeAnalysis
{
    public readonly struct GeneratorDriverState
    {
        internal readonly ImmutableArray<ISourceGenerator> Generators;

        internal readonly ImmutableArray<GeneratorState> GeneratorStates;

        internal readonly ImmutableArray<AdditionalText> AdditionalTexts;

        internal readonly AnalyzerConfigOptionsProvider OptionsProvider;

        internal readonly ImmutableArray<PendingEdit> Edits;

        internal readonly bool EditsFailed;

        public readonly ParseOptions ParseOptions;

        public GeneratorDriverState(ParseOptions parseOptions, AnalyzerConfigOptionsProvider optionsProvider, ImmutableArray<ISourceGenerator> generators, ImmutableArray<AdditionalText> additionalTexts, ImmutableArray<GeneratorState> generatorStates, ImmutableArray<PendingEdit> edits, bool editsFailed)
        {
            Generators = generators;
            GeneratorStates = generatorStates;
            AdditionalTexts = additionalTexts;
            Edits = edits;
            ParseOptions = parseOptions;
            OptionsProvider = optionsProvider;
            EditsFailed = editsFailed;
        }

        internal GeneratorDriverState With(ImmutableArray<ISourceGenerator>? generators = null, ImmutableArray<GeneratorState>? generatorStates = null, ImmutableArray<AdditionalText>? additionalTexts = null, ImmutableArray<PendingEdit>? edits = null, bool? editsFailed = null)
        {
            return new GeneratorDriverState(ParseOptions, OptionsProvider, generators ?? Generators, additionalTexts ?? AdditionalTexts, generatorStates ?? GeneratorStates, edits ?? Edits, editsFailed ?? EditsFailed);
        }
    }
}
