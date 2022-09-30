using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedSimpleMethodSymbol : SynthesizedRegularMethodBase
	{
		private ImmutableArray<ParameterSymbol> _parameters;

		private readonly MethodSymbol _overriddenMethod;

		private readonly ImmutableArray<MethodSymbol> _interfaceMethods;

		private readonly bool _isOverloads;

		private readonly TypeSymbol _returnType;

		public override bool IsOverloads => _isOverloads;

		public override bool IsOverrides => (object)_overriddenMethod != null;

		public override MethodSymbol OverriddenMethod => _overriddenMethod;

		public override Accessibility DeclaredAccessibility => Accessibility.Public;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => _interfaceMethods;

		public override bool IsSub => TypeSymbolExtensions.IsVoidType(_returnType);

		public override TypeSymbol ReturnType => _returnType;

		internal override int ParameterCount => _parameters.Length;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		internal override bool GenerateDebugInfoImpl => false;

		public SynthesizedSimpleMethodSymbol(NamedTypeSymbol container, string name, TypeSymbol returnType, MethodSymbol overriddenMethod = null, MethodSymbol interfaceMethod = null, bool isOverloads = false)
			: base(VisualBasicSyntaxTree.Dummy.GetRoot(), container, name)
		{
			_returnType = returnType;
			_overriddenMethod = overriddenMethod;
			_isOverloads = isOverloads;
			_interfaceMethods = (((object)interfaceMethod == null) ? ImmutableArray<MethodSymbol>.Empty : ImmutableArray.Create(interfaceMethod));
		}

		internal void SetParameters(ImmutableArray<ParameterSymbol> parameters)
		{
			_parameters = parameters;
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
