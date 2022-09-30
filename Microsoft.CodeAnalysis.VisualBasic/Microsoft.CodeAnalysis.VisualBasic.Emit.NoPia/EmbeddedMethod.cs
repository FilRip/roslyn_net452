using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Emit.NoPia;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
{
	internal sealed class EmbeddedMethod : EmbeddedTypesManager<PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter>.CommonEmbeddedMethod
	{
		internal override EmbeddedTypesManager TypeManager => ContainingType.TypeManager;

		protected override bool IsAbstract => base.UnderlyingMethod.AdaptedMethodSymbol.IsMustOverride;

		protected override bool IsAccessCheckedOnOverride => base.UnderlyingMethod.AdaptedMethodSymbol.IsAccessCheckedOnOverride;

		protected override bool IsConstructor => base.UnderlyingMethod.AdaptedMethodSymbol.MethodKind == MethodKind.Constructor;

		protected override bool IsExternal => base.UnderlyingMethod.AdaptedMethodSymbol.IsExternal;

		protected override bool IsHiddenBySignature => base.UnderlyingMethod.AdaptedMethodSymbol.IsHiddenBySignature;

		protected override bool IsNewSlot => base.UnderlyingMethod.AdaptedMethodSymbol.IsMetadataNewSlot();

		protected override IPlatformInvokeInformation PlatformInvokeData => base.UnderlyingMethod.AdaptedMethodSymbol.GetDllImportData();

		protected override bool IsRuntimeSpecial => base.UnderlyingMethod.AdaptedMethodSymbol.HasRuntimeSpecialName;

		protected override bool IsSpecialName => base.UnderlyingMethod.AdaptedMethodSymbol.HasSpecialName;

		protected override bool IsSealed => base.UnderlyingMethod.AdaptedMethodSymbol.IsMetadataFinal;

		protected override bool IsStatic => base.UnderlyingMethod.AdaptedMethodSymbol.IsShared;

		protected override bool IsVirtual => SymbolExtensions.IsMetadataVirtual(base.UnderlyingMethod.AdaptedMethodSymbol);

		protected override bool ReturnValueIsMarshalledExplicitly => base.UnderlyingMethod.AdaptedMethodSymbol.ReturnValueIsMarshalledExplicitly;

		protected override IMarshallingInformation ReturnValueMarshallingInformation => base.UnderlyingMethod.AdaptedMethodSymbol.ReturnTypeMarshallingInformation;

		protected override ImmutableArray<byte> ReturnValueMarshallingDescriptor => base.UnderlyingMethod.AdaptedMethodSymbol.ReturnValueMarshallingDescriptor;

		protected override TypeMemberVisibility Visibility => PEModuleBuilder.MemberVisibility(base.UnderlyingMethod.AdaptedMethodSymbol);

		protected override string Name => base.UnderlyingMethod.AdaptedMethodSymbol.MetadataName;

		protected override bool AcceptsExtraArguments => base.UnderlyingMethod.AdaptedMethodSymbol.IsVararg;

		protected override ISignature UnderlyingMethodSignature => base.UnderlyingMethod;

		protected override INamespace ContainingNamespace => base.UnderlyingMethod.AdaptedMethodSymbol.ContainingNamespace.GetCciAdapter();

		public EmbeddedMethod(EmbeddedType containingType, MethodSymbol underlyingMethod)
			: base(containingType, underlyingMethod)
		{
		}

		protected override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
		{
			return base.UnderlyingMethod.AdaptedMethodSymbol.GetCustomAttributesToEmit(moduleBuilder.CompilationState);
		}

		protected override ImmutableArray<EmbeddedParameter> GetParameters()
		{
			return EmbeddedTypesManager.EmbedParameters(this, base.UnderlyingMethod.AdaptedMethodSymbol.Parameters);
		}

		protected override ImmutableArray<EmbeddedTypeParameter> GetTypeParameters()
		{
			return base.UnderlyingMethod.AdaptedMethodSymbol.TypeParameters.SelectAsArray((TypeParameterSymbol typeParameter, EmbeddedMethod container) => new EmbeddedTypeParameter(container, typeParameter.GetCciAdapter()), this);
		}

		protected override MethodImplAttributes GetImplementationAttributes(EmitContext context)
		{
			return base.UnderlyingMethod.AdaptedMethodSymbol.ImplementationAttributes;
		}
	}
}
