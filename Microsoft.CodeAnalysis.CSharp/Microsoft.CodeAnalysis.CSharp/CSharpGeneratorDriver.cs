using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class CSharpGeneratorDriver : GeneratorDriver
    {
        public override CommonMessageProvider MessageProvider => Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance;

        public CSharpGeneratorDriver(CSharpParseOptions parseOptions, ImmutableArray<ISourceGenerator> generators, AnalyzerConfigOptionsProvider optionsProvider, ImmutableArray<AdditionalText> additionalTexts)
            : base(parseOptions, generators, optionsProvider, additionalTexts)
        {
        }

        private CSharpGeneratorDriver(GeneratorDriverState state)
            : base(state)
        {
        }

        public static CSharpGeneratorDriver Create(params ISourceGenerator[] generators)
        {
            return Create(generators, null, null, null);
        }

        public static CSharpGeneratorDriver Create(IEnumerable<ISourceGenerator> generators, IEnumerable<AdditionalText>? additionalTexts = null, CSharpParseOptions? parseOptions = null, AnalyzerConfigOptionsProvider? optionsProvider = null)
        {
            return new CSharpGeneratorDriver(parseOptions ?? CSharpParseOptions.Default, generators.ToImmutableArray(), optionsProvider ?? CompilerAnalyzerConfigOptionsProvider.Empty, additionalTexts.AsImmutableOrEmpty());
        }

        public override SyntaxTree ParseGeneratedSourceText(GeneratedSourceText input, string fileName, CancellationToken cancellationToken)
        {
            return CSharpSyntaxTree.ParseTextLazy(input.Text, (CSharpParseOptions)_state.ParseOptions, fileName);
        }

        public override GeneratorDriver FromState(GeneratorDriverState state)
        {
            return new CSharpGeneratorDriver(state);
        }

        public override AdditionalSourcesCollection CreateSourcesCollection()
        {
            return new AdditionalSourcesCollection(".cs");
        }
    }
}
