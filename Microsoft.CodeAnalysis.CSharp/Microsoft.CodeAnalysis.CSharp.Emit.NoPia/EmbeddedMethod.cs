using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Emit.NoPia;

namespace Microsoft.CodeAnalysis.CSharp.Emit.NoPia
{
    public sealed class EmbeddedMethod : EmbeddedTypesManager<PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, CSharpAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter>.CommonEmbeddedMethod
    {
        public override EmbeddedTypesManager TypeManager => ContainingType.TypeManager;

        protected override bool IsAbstract => base.UnderlyingMethod.AdaptedMethodSymbol.IsAbstract;

        protected override bool IsAccessCheckedOnOverride => base.UnderlyingMethod.AdaptedMethodSymbol.IsAccessCheckedOnOverride;

        protected override bool IsConstructor => base.UnderlyingMethod.AdaptedMethodSymbol.MethodKind == MethodKind.Constructor;

        protected override bool IsExternal => base.UnderlyingMethod.AdaptedMethodSymbol.IsExternal;

        protected override bool IsHiddenBySignature => !base.UnderlyingMethod.AdaptedMethodSymbol.HidesBaseMethodsByName;

        protected override bool IsNewSlot => base.UnderlyingMethod.AdaptedMethodSymbol.IsMetadataNewSlot();

        protected override IPlatformInvokeInformation PlatformInvokeData => base.UnderlyingMethod.AdaptedMethodSymbol.GetDllImportData();

        protected override bool IsRuntimeSpecial => base.UnderlyingMethod.AdaptedMethodSymbol.HasRuntimeSpecialName;

        protected override bool IsSpecialName => base.UnderlyingMethod.AdaptedMethodSymbol.HasSpecialName;

        protected override bool IsSealed => base.UnderlyingMethod.AdaptedMethodSymbol.IsMetadataFinal;

        protected override bool IsStatic => base.UnderlyingMethod.AdaptedMethodSymbol.IsStatic;

        protected override bool IsVirtual => base.UnderlyingMethod.AdaptedMethodSymbol.IsMetadataVirtual();

        protected override bool ReturnValueIsMarshalledExplicitly => base.UnderlyingMethod.AdaptedMethodSymbol.ReturnValueIsMarshalledExplicitly;

        protected override IMarshallingInformation ReturnValueMarshallingInformation => base.UnderlyingMethod.AdaptedMethodSymbol.ReturnValueMarshallingInformation;

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

        protected override IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
        {
            return base.UnderlyingMethod.AdaptedSymbol.GetCustomAttributesToEmit(moduleBuilder);
        }

        protected override ImmutableArray<EmbeddedParameter> GetParameters()
        {
            return EmbeddedTypesManager.EmbedParameters(this, base.UnderlyingMethod.AdaptedMethodSymbol.Parameters);
        }

        protected override ImmutableArray<EmbeddedTypeParameter> GetTypeParameters()
        {
            return base.UnderlyingMethod.AdaptedMethodSymbol.TypeParameters.SelectAsArray((TypeParameterSymbol t, EmbeddedMethod m) => new EmbeddedTypeParameter(m, t.GetCciAdapter()), this);
        }

        protected override MethodImplAttributes GetImplementationAttributes(EmitContext context)
        {
            return base.UnderlyingMethod.AdaptedMethodSymbol.ImplementationAttributes;
        }
    }
}
