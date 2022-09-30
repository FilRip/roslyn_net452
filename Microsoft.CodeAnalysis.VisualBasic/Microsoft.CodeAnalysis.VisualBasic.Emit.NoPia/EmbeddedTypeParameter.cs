using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Emit.NoPia;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
{
	internal sealed class EmbeddedTypeParameter : EmbeddedTypesManager<PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter>.CommonEmbeddedTypeParameter
	{
		protected override bool MustBeReferenceType => UnderlyingTypeParameter.AdaptedTypeParameterSymbol.HasReferenceTypeConstraint;

		protected override bool MustBeValueType => UnderlyingTypeParameter.AdaptedTypeParameterSymbol.HasValueTypeConstraint;

		protected override bool MustHaveDefaultConstructor => UnderlyingTypeParameter.AdaptedTypeParameterSymbol.HasConstructorConstraint;

		protected override string Name => UnderlyingTypeParameter.AdaptedTypeParameterSymbol.MetadataName;

		protected override ushort Index => (ushort)UnderlyingTypeParameter.AdaptedTypeParameterSymbol.Ordinal;

		public EmbeddedTypeParameter(EmbeddedMethod containingMethod, TypeParameterSymbol underlyingTypeParameter)
			: base(containingMethod, underlyingTypeParameter)
		{
		}

		protected override IEnumerable<TypeReferenceWithAttributes> GetConstraints(EmitContext context)
		{
			return ((IGenericParameter)UnderlyingTypeParameter).GetConstraints(context);
		}
	}
}
