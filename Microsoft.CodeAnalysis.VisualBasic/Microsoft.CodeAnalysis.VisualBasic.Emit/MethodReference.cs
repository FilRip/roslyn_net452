using System.Collections.Immutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal abstract class MethodReference : TypeMemberReference, IMethodReference
	{
		protected readonly MethodSymbol m_UnderlyingMethod;

		protected override Symbol UnderlyingSymbol => m_UnderlyingMethod;

		private bool IMethodReferenceAcceptsExtraArguments => m_UnderlyingMethod.IsVararg;

		private ushort IMethodReferenceGenericParameterCount => (ushort)m_UnderlyingMethod.Arity;

		private bool IMethodReferenceIsGeneric => m_UnderlyingMethod.IsGenericMethod;

		private ushort IMethodReferenceParameterCount => (ushort)m_UnderlyingMethod.ParameterCount;

		private ImmutableArray<IParameterTypeInformation> IMethodReferenceExtraParameters => ImmutableArray<IParameterTypeInformation>.Empty;

		private CallingConvention ISignatureCallingConvention => m_UnderlyingMethod.CallingConvention;

		private ImmutableArray<ICustomModifier> ISignatureReturnValueCustomModifiers => m_UnderlyingMethod.ReturnTypeCustomModifiers.As<ICustomModifier>();

		private ImmutableArray<ICustomModifier> ISignatureRefCustomModifiers => m_UnderlyingMethod.RefCustomModifiers.As<ICustomModifier>();

		private bool ISignatureReturnValueIsByRef => m_UnderlyingMethod.ReturnsByRef;

		public virtual IGenericMethodInstanceReference AsGenericMethodInstanceReference => null;

		public virtual ISpecializedMethodReference AsSpecializedMethodReference => null;

		public MethodReference(MethodSymbol underlyingMethod)
		{
			m_UnderlyingMethod = underlyingMethod;
		}

		private IMethodDefinition IMethodReferenceGetResolvedMethod(EmitContext context)
		{
			return null;
		}

		IMethodDefinition IMethodReference.GetResolvedMethod(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IMethodReferenceGetResolvedMethod
			return this.IMethodReferenceGetResolvedMethod(context);
		}

		private ImmutableArray<IParameterTypeInformation> ISignatureGetParameters(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).Translate(m_UnderlyingMethod.Parameters);
		}

		ImmutableArray<IParameterTypeInformation> ISignature.GetParameters(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISignatureGetParameters
			return this.ISignatureGetParameters(context);
		}

		private ITypeReference ISignatureGetType(EmitContext context)
		{
			PEModuleBuilder obj = (PEModuleBuilder)context.Module;
			TypeSymbol returnType = m_UnderlyingMethod.ReturnType;
			return obj.Translate(returnType, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
		}

		ITypeReference ISignature.GetType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISignatureGetType
			return this.ISignatureGetType(context);
		}
	}
}
