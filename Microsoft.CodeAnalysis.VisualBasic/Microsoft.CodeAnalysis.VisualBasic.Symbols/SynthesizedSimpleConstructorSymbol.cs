using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SynthesizedSimpleConstructorSymbol : SynthesizedConstructorBase, ISynthesizedMethodBodyImplementationSymbol
	{
		private ImmutableArray<ParameterSymbol> _parameters;

		internal sealed override int ParameterCount => _parameters.Length;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		public bool HasMethodBodyDependency => false;

		public IMethodSymbolInternal Method => ((ISynthesizedMethodBodyImplementationSymbol)base.ContainingSymbol).Method;

		internal sealed override bool GenerateDebugInfoImpl => false;

		public SynthesizedSimpleConstructorSymbol(NamedTypeSymbol container)
			: base(VisualBasicSyntaxTree.DummyReference, container, isShared: false, null, null)
		{
		}

		internal void SetParameters(ImmutableArray<ParameterSymbol> parameters)
		{
			_parameters = parameters;
		}

		internal sealed override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
