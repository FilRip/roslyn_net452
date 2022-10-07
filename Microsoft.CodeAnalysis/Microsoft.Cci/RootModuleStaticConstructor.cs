using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Debugging;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.Cci
{
    internal sealed class RootModuleStaticConstructor : IMethodDefinition, ITypeDefinitionMember, ITypeMemberReference, IReference, INamedEntity, IDefinition, IMethodReference, ISignature, IMethodBody
    {
        public ITypeDefinition ContainingTypeDefinition { get; }

        public string Name => ".cctor";

        public IEnumerable<IGenericMethodParameter> GenericParameters => SpecializedCollections.EmptyEnumerable<IGenericMethodParameter>();

        public bool IsImplicitlyDeclared => true;

        public bool HasDeclarativeSecurity => false;

        public bool IsAbstract => false;

        public bool IsAccessCheckedOnOverride => false;

        public bool IsConstructor => false;

        public bool IsExternal => false;

        public bool IsHiddenBySignature => true;

        public bool IsNewSlot => false;

        public bool IsPlatformInvoke => false;

        public bool IsRuntimeSpecial => true;

        public bool IsSealed => false;

        public bool IsSpecialName => true;

        public bool IsStatic => true;

        public bool IsVirtual => false;

        public ImmutableArray<IParameterDefinition> Parameters => ImmutableArray<IParameterDefinition>.Empty;

        public IPlatformInvokeInformation PlatformInvokeData => null;

        public bool RequiresSecurityObject => false;

        public bool ReturnValueIsMarshalledExplicitly => false;

        public IMarshallingInformation ReturnValueMarshallingInformation => null;

        public ImmutableArray<byte> ReturnValueMarshallingDescriptor => default;

        public IEnumerable<SecurityAttribute> SecurityAttributes => null;

        public INamespace ContainingNamespace => null;

        public TypeMemberVisibility Visibility => TypeMemberVisibility.Private;

        public bool AcceptsExtraArguments => false;

        public ushort GenericParameterCount => 0;

        public bool IsGeneric => false;

        public ImmutableArray<IParameterTypeInformation> ExtraParameters => ImmutableArray<IParameterTypeInformation>.Empty;

        public IGenericMethodInstanceReference AsGenericMethodInstanceReference => null;

        public ISpecializedMethodReference AsSpecializedMethodReference => null;

        public CallingConvention CallingConvention => CallingConvention.Default;

        public ushort ParameterCount => 0;

        public ImmutableArray<ICustomModifier> ReturnValueCustomModifiers => ImmutableArray<ICustomModifier>.Empty;

        public ImmutableArray<ICustomModifier> RefCustomModifiers => ImmutableArray<ICustomModifier>.Empty;

        public bool ReturnValueIsByRef => false;

        public ushort MaxStack => 0;

        public ImmutableArray<byte> IL { get; }

        public IMethodDefinition MethodDefinition => this;

        public ImmutableArray<ExceptionHandlerRegion> ExceptionRegions => ImmutableArray<ExceptionHandlerRegion>.Empty;

        public bool AreLocalsZeroed => false;

        public bool HasStackalloc => false;

        public ImmutableArray<ILocalDefinition> LocalVariables => ImmutableArray<ILocalDefinition>.Empty;

        public StateMachineMoveNextBodyDebugInfo MoveNextBodyInfo => null;

        public ImmutableArray<SequencePoint> SequencePoints => ImmutableArray<SequencePoint>.Empty;

        public bool HasDynamicLocalVariables => false;

        public ImmutableArray<LocalScope> LocalScopes => ImmutableArray<LocalScope>.Empty;

        public IImportScope ImportScope => null;

        public DebugId MethodId => default;

        public ImmutableArray<StateMachineHoistedLocalScope> StateMachineHoistedLocalScopes => ImmutableArray<StateMachineHoistedLocalScope>.Empty;

        public string StateMachineTypeName => null;

        public ImmutableArray<EncHoistedLocalInfo> StateMachineHoistedLocalSlots => ImmutableArray<EncHoistedLocalInfo>.Empty;

        public ImmutableArray<ITypeReference> StateMachineAwaiterSlots => ImmutableArray<ITypeReference>.Empty;

        public ImmutableArray<ClosureDebugInfo> ClosureDebugInfo => ImmutableArray<Microsoft.CodeAnalysis.CodeGen.ClosureDebugInfo>.Empty;

        public ImmutableArray<LambdaDebugInfo> LambdaDebugInfo => ImmutableArray<Microsoft.CodeAnalysis.CodeGen.LambdaDebugInfo>.Empty;

        public DynamicAnalysisMethodBodyData DynamicAnalysisData => null;

        public RootModuleStaticConstructor(ITypeDefinition containingTypeDefinition, ImmutableArray<byte> il)
        {
            ContainingTypeDefinition = containingTypeDefinition;
            IL = il;
        }

        public IMethodBody GetBody(EmitContext context)
        {
            return this;
        }

        public IDefinition AsDefinition(EmitContext context)
        {
            return this;
        }

        ISymbolInternal IReference.GetInternalSymbol()
        {
            return null;
        }

        public void Dispatch(MetadataVisitor visitor)
        {
            visitor.Visit((IMethodDefinition)this);
        }

        public IEnumerable<ICustomAttribute> GetAttributes(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<ICustomAttribute>();
        }

        public ITypeReference GetContainingType(EmitContext context)
        {
            return ContainingTypeDefinition;
        }

        public MethodImplAttributes GetImplementationAttributes(EmitContext context)
        {
            return MethodImplAttributes.IL;
        }

        public ImmutableArray<IParameterTypeInformation> GetParameters(EmitContext context)
        {
            return ImmutableArray<IParameterTypeInformation>.Empty;
        }

        public IMethodDefinition GetResolvedMethod(EmitContext context)
        {
            return this;
        }

        public IEnumerable<ICustomAttribute> GetReturnValueAttributes(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<ICustomAttribute>();
        }

        public ITypeReference GetType(EmitContext context)
        {
            return context.Module.GetPlatformType(PlatformType.SystemVoid, context);
        }

        public sealed override bool Equals(object obj)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public sealed override int GetHashCode()
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
