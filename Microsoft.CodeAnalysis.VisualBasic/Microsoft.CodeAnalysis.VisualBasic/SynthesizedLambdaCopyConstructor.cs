using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class SynthesizedLambdaCopyConstructor : SynthesizedLambdaConstructor
	{
		private readonly ImmutableArray<ParameterSymbol> _parameters;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		internal override bool GenerateDebugInfoImpl => false;

		internal SynthesizedLambdaCopyConstructor(SyntaxNode syntaxNode, LambdaFrame containingType)
			: base(syntaxNode, containingType)
		{
			_parameters = ImmutableArray.Create((ParameterSymbol)new SourceSimpleParameterSymbol(this, "arg0", 0, containingType, null));
		}
	}
}
