using System.Collections.Immutable;
using Microsoft.Cci;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedOverridingWithEventsProperty : PropertySymbol
	{
		private readonly PropertySymbol _baseProperty;

		private readonly SourceNamedTypeSymbol _containingType;

		private readonly SynthesizedWithEventsAccessorSymbol _getter;

		private readonly SynthesizedWithEventsAccessorSymbol _setter;

		public override bool IsWithEvents => true;

		internal override bool ShadowsExplicitly => true;

		internal override CallingConvention CallingConvention => _baseProperty.CallingConvention;

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public override Accessibility DeclaredAccessibility => _baseProperty.DeclaredAccessibility;

		public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations => ImmutableArray<PropertySymbol>.Empty;

		public override MethodSymbol GetMethod => _getter;

		public override MethodSymbol SetMethod => _setter;

		internal override FieldSymbol AssociatedField => null;

		public override bool IsDefault => false;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverloads => false;

		public override bool IsOverridable => true;

		public override bool IsOverrides => true;

		public override bool IsShared => false;

		public override ImmutableArray<Location> Locations => _containingType.Locations;

		public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

		public override int ParameterCount => 0;

		public override bool ReturnsByRef => _baseProperty.ReturnsByRef;

		public override TypeSymbol Type => _baseProperty.Type;

		public override ImmutableArray<CustomModifier> TypeCustomModifiers => _baseProperty.TypeCustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => _baseProperty.RefCustomModifiers;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override string Name => _baseProperty.Name;

		internal override bool HasSpecialName => false;

		public override bool IsImplicitlyDeclared => true;

		internal override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal override bool IsMyGroupCollectionProperty => false;

		internal SynthesizedOverridingWithEventsProperty(PropertySymbol baseProperty, SourceNamedTypeSymbol container)
		{
			_baseProperty = baseProperty;
			_containingType = container;
			_getter = new SynthesizedWithEventsGetAccessorSymbol(container, this);
			_setter = new SynthesizedWithEventsSetAccessorSymbol(container, this, baseProperty.SetMethod.ReturnType, "WithEventsValue");
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return _containingType.GetLexicalSortKey();
		}
	}
}
