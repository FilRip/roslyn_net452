using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class SynthesizedStateMachineMethod : SynthesizedMethod, ISynthesizedMethodBodyImplementationSymbol
	{
		private readonly MethodSymbol _interfaceMethod;

		private readonly ImmutableArray<ParameterSymbol> _parameters;

		private readonly ImmutableArray<Location> _locations;

		private readonly Accessibility _accessibility;

		private readonly bool _generateDebugInfo;

		private readonly bool _hasMethodBodyDependency;

		private readonly PropertySymbol _associatedProperty;

		public StateMachineTypeSymbol StateMachineType => (StateMachineTypeSymbol)base.ContainingSymbol;

		internal override TypeSubstitution TypeMap => null;

		public override ImmutableArray<TypeSymbol> TypeArguments => ImmutableArray<TypeSymbol>.Empty;

		public override ImmutableArray<Location> Locations => _locations;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		public override TypeSymbol ReturnType => _interfaceMethod.ReturnType;

		public override bool IsShared => false;

		public override bool IsSub => _interfaceMethod.IsSub;

		public override bool IsVararg => _interfaceMethod.IsVararg;

		public override int Arity => 0;

		public override Accessibility DeclaredAccessibility => _accessibility;

		internal override int ParameterCount => _parameters.Length;

		internal override bool HasSpecialName => false;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray.Create(_interfaceMethod);

		public override Symbol AssociatedSymbol => _associatedProperty;

		internal override bool GenerateDebugInfoImpl => _generateDebugInfo;

		public bool HasMethodBodyDependency => _hasMethodBodyDependency;

		public IMethodSymbolInternal Method => StateMachineType.KickoffMethod;

		protected SynthesizedStateMachineMethod(StateMachineTypeSymbol stateMachineType, string name, MethodSymbol interfaceMethod, SyntaxNode syntax, Accessibility declaredAccessibility, bool generateDebugInfo, bool hasMethodBodyDependency, PropertySymbol associatedProperty = null)
			: base(syntax, stateMachineType, name, isShared: false)
		{
			_locations = ImmutableArray.Create(syntax.GetLocation());
			_accessibility = declaredAccessibility;
			_generateDebugInfo = generateDebugInfo;
			_hasMethodBodyDependency = hasMethodBodyDependency;
			_interfaceMethod = interfaceMethod;
			ParameterSymbol[] array = new ParameterSymbol[_interfaceMethod.ParameterCount - 1 + 1];
			int num = array.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				ParameterSymbol parameterSymbol = _interfaceMethod.Parameters[i];
				array[i] = SynthesizedMethod.WithNewContainerAndType(this, parameterSymbol.Type, parameterSymbol);
			}
			_parameters = array.AsImmutableOrNull();
			_associatedProperty = associatedProperty;
		}

		internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			return true;
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			return StateMachineType.KickoffMethod.CalculateLocalSyntaxOffset(localPosition, localTree);
		}
	}
}
