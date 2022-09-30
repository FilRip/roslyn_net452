using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class SynthesizedLambdaCacheFieldSymbol : SynthesizedFieldSymbol, ISynthesizedMethodBodyImplementationSymbol
	{
		private readonly MethodSymbol _topLevelMethod;

		public bool HasMethodBodyDependency => false;

		public IMethodSymbolInternal Method => _topLevelMethod;

		public SynthesizedLambdaCacheFieldSymbol(NamedTypeSymbol containingType, Symbol implicitlyDefinedBy, TypeSymbol type, string name, MethodSymbol topLevelMethod, Accessibility accessibility = Accessibility.Private, bool isReadOnly = false, bool isShared = false, bool isSpecialNameAndRuntimeSpecial = false)
			: base(containingType, implicitlyDefinedBy, type, name, accessibility, isReadOnly, isShared, isSpecialNameAndRuntimeSpecial)
		{
			_topLevelMethod = topLevelMethod;
		}
	}
}
