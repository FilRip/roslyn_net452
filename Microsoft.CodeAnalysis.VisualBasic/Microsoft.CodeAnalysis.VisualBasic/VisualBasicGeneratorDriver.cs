using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public class VisualBasicGeneratorDriver : GeneratorDriver
	{
		internal override CommonMessageProvider MessageProvider => Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance;

		private VisualBasicGeneratorDriver(GeneratorDriverState state)
			: base(state)
		{
		}

		internal VisualBasicGeneratorDriver(VisualBasicParseOptions parseOptions, ImmutableArray<ISourceGenerator> generators, AnalyzerConfigOptionsProvider optionsProvider, ImmutableArray<AdditionalText> additionalTexts)
			: base(parseOptions, generators, optionsProvider, additionalTexts)
		{
		}

		internal override GeneratorDriver FromState(GeneratorDriverState state)
		{
			return new VisualBasicGeneratorDriver(state);
		}

		internal override SyntaxTree ParseGeneratedSourceText(GeneratedSourceText input, string fileName, CancellationToken cancellationToken)
		{
			return VisualBasicSyntaxTree.ParseTextLazy(input.Text, (VisualBasicParseOptions)_state.ParseOptions, fileName);
		}

		public static VisualBasicGeneratorDriver Create(ImmutableArray<ISourceGenerator> generators, ImmutableArray<AdditionalText> additionalTexts = default(ImmutableArray<AdditionalText>), VisualBasicParseOptions parseOptions = null, AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider = null)
		{
			return new VisualBasicGeneratorDriver(parseOptions, generators, analyzerConfigOptionsProvider, additionalTexts);
		}

		internal override AdditionalSourcesCollection CreateSourcesCollection()
		{
			return new AdditionalSourcesCollection(".vb");
		}
	}
}
