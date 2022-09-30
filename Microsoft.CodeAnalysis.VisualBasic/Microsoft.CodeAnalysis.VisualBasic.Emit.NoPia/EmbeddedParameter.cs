using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Emit.NoPia;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
{
	internal sealed class EmbeddedParameter : EmbeddedTypesManager<PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter>.CommonEmbeddedParameter
	{
		protected override bool HasDefaultValue => UnderlyingParameter.AdaptedParameterSymbol.HasMetadataConstantValue;

		protected override bool IsIn => UnderlyingParameter.AdaptedParameterSymbol.IsMetadataIn;

		protected override bool IsOut => UnderlyingParameter.AdaptedParameterSymbol.IsMetadataOut;

		protected override bool IsOptional => UnderlyingParameter.AdaptedParameterSymbol.IsMetadataOptional;

		protected override bool IsMarshalledExplicitly => UnderlyingParameter.AdaptedParameterSymbol.IsMarshalledExplicitly;

		protected override IMarshallingInformation MarshallingInformation => UnderlyingParameter.AdaptedParameterSymbol.MarshallingInformation;

		protected override ImmutableArray<byte> MarshallingDescriptor => UnderlyingParameter.AdaptedParameterSymbol.MarshallingDescriptor;

		protected override string Name => UnderlyingParameter.AdaptedParameterSymbol.MetadataName;

		protected override IParameterTypeInformation UnderlyingParameterTypeInformation => UnderlyingParameter;

		protected override ushort Index => (ushort)UnderlyingParameter.AdaptedParameterSymbol.Ordinal;

		public EmbeddedParameter(EmbeddedTypesManager<PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter>.CommonEmbeddedMember containingPropertyOrMethod, ParameterSymbol underlyingParameter)
			: base(containingPropertyOrMethod, underlyingParameter)
		{
		}

		protected override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
		{
			return UnderlyingParameter.AdaptedParameterSymbol.GetCustomAttributesToEmit(moduleBuilder.CompilationState);
		}

		protected override MetadataConstant GetDefaultValue(EmitContext context)
		{
			return UnderlyingParameter.GetMetadataConstantValue(context);
		}
	}
}