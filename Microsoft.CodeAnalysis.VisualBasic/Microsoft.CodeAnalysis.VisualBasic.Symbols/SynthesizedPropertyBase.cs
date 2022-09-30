using System.Collections.Immutable;
using Microsoft.Cci;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SynthesizedPropertyBase : PropertySymbol
	{
		internal override FieldSymbol AssociatedField => null;

		public override bool IsDefault => false;

		public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

		public override ImmutableArray<CustomModifier> TypeCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public sealed override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public sealed override bool ReturnsByRef => false;

		internal override bool HasSpecialName => false;

		public override Accessibility DeclaredAccessibility => Accessibility.Public;

		internal override CallingConvention CallingConvention => CallingConvention.HasThis;

		public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations => ImmutableArray<PropertySymbol>.Empty;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverloads => false;

		internal override bool ShadowsExplicitly => false;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public override bool IsShared => false;

		public abstract override bool IsImplicitlyDeclared { get; }

		internal override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal override bool IsMyGroupCollectionProperty => false;
	}
}
