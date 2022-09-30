using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Emit.NoPia;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
{
	internal sealed class EmbeddedField : EmbeddedTypesManager<PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter>.CommonEmbeddedField
	{
		internal override EmbeddedTypesManager TypeManager => ContainingType.TypeManager;

		protected override bool IsCompileTimeConstant => base.UnderlyingField.AdaptedFieldSymbol.IsMetadataConstant;

		protected override bool IsNotSerialized => base.UnderlyingField.AdaptedFieldSymbol.IsNotSerialized;

		protected override bool IsReadOnly => base.UnderlyingField.AdaptedFieldSymbol.IsReadOnly;

		protected override bool IsRuntimeSpecial => base.UnderlyingField.AdaptedFieldSymbol.HasRuntimeSpecialName;

		protected override bool IsSpecialName => base.UnderlyingField.AdaptedFieldSymbol.HasSpecialName;

		protected override bool IsStatic => base.UnderlyingField.AdaptedFieldSymbol.IsShared;

		protected override bool IsMarshalledExplicitly => base.UnderlyingField.AdaptedFieldSymbol.IsMarshalledExplicitly;

		protected override IMarshallingInformation MarshallingInformation => base.UnderlyingField.AdaptedFieldSymbol.MarshallingInformation;

		protected override ImmutableArray<byte> MarshallingDescriptor => base.UnderlyingField.AdaptedFieldSymbol.MarshallingDescriptor;

		protected override int? TypeLayoutOffset => base.UnderlyingField.AdaptedFieldSymbol.TypeLayoutOffset;

		protected override TypeMemberVisibility Visibility => PEModuleBuilder.MemberVisibility(base.UnderlyingField.AdaptedFieldSymbol);

		protected override string Name => base.UnderlyingField.AdaptedFieldSymbol.MetadataName;

		public EmbeddedField(EmbeddedType containingType, FieldSymbol underlyingField)
			: base(containingType, underlyingField)
		{
		}

		protected override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
		{
			return base.UnderlyingField.AdaptedFieldSymbol.GetCustomAttributesToEmit(moduleBuilder.CompilationState);
		}

		protected override MetadataConstant GetCompileTimeValue(EmitContext context)
		{
			return base.UnderlyingField.GetMetadataConstantValue(context);
		}
	}
}
