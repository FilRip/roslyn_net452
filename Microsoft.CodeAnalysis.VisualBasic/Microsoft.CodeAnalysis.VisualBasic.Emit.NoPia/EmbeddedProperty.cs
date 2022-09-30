using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit.NoPia;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
{
	internal sealed class EmbeddedProperty : EmbeddedTypesManager<PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter>.CommonEmbeddedProperty
	{
		protected override bool IsRuntimeSpecial => base.UnderlyingProperty.AdaptedPropertySymbol.HasRuntimeSpecialName;

		protected override bool IsSpecialName => base.UnderlyingProperty.AdaptedPropertySymbol.HasSpecialName;

		protected override ISignature UnderlyingPropertySignature => base.UnderlyingProperty;

		protected override EmbeddedType ContainingType => base.AnAccessor.ContainingType;

		protected override TypeMemberVisibility Visibility => PEModuleBuilder.MemberVisibility(base.UnderlyingProperty.AdaptedPropertySymbol);

		protected override string Name => base.UnderlyingProperty.AdaptedPropertySymbol.MetadataName;

		public EmbeddedProperty(PropertySymbol underlyingProperty, EmbeddedMethod getter, EmbeddedMethod setter)
			: base(underlyingProperty, getter, setter)
		{
		}

		protected override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
		{
			return base.UnderlyingProperty.AdaptedPropertySymbol.GetCustomAttributesToEmit(moduleBuilder.CompilationState);
		}

		protected override ImmutableArray<EmbeddedParameter> GetParameters()
		{
			return EmbeddedTypesManager.EmbedParameters(this, base.UnderlyingProperty.AdaptedPropertySymbol.Parameters);
		}
	}
}
