using System.Collections.Immutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class SynthesizedStateMachineProperty : SynthesizedPropertyBase, ISynthesizedMethodBodyImplementationSymbol
	{
		private readonly SynthesizedStateMachineMethod _getter;

		private readonly string _name;

		private PropertySymbol ImplementedProperty => (PropertySymbol)_getter.ExplicitInterfaceImplementations[0].AssociatedSymbol;

		public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations => ImmutableArray.Create(ImplementedProperty);

		public override string Name => _name;

		public override Symbol ContainingSymbol => _getter.ContainingSymbol;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override MethodSymbol GetMethod => _getter;

		public override bool IsImplicitlyDeclared => true;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override MethodSymbol SetMethod => null;

		public override TypeSymbol Type => _getter.ReturnType;

		public override ImmutableArray<ParameterSymbol> Parameters => _getter.Parameters;

		public override int ParameterCount => _getter.ParameterCount;

		public override ImmutableArray<CustomModifier> TypeCustomModifiers => _getter.ReturnTypeCustomModifiers;

		public override Accessibility DeclaredAccessibility => _getter.DeclaredAccessibility;

		internal override CallingConvention CallingConvention => _getter.CallingConvention;

		public override bool IsShared => _getter.IsShared;

		public bool HasMethodBodyDependency => _getter.HasMethodBodyDependency;

		public IMethodSymbolInternal Method => ((ISynthesizedMethodBodyImplementationSymbol)ContainingSymbol).Method;

		internal SynthesizedStateMachineProperty(StateMachineTypeSymbol stateMachineType, string name, MethodSymbol interfacePropertyGetter, SyntaxNode syntax, Accessibility declaredAccessibility)
		{
			_name = name;
			_getter = new SynthesizedStateMachineDebuggerNonUserCodeMethod(stateMachineType, (name.Length != 7) ? "IEnumerator.get_Current" : "get_Current", interfacePropertyGetter, syntax, declaredAccessibility, hasMethodBodyDependency: false, this);
		}
	}
}
