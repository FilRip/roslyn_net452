Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend MustInherit Class MethodReference
		Inherits TypeMemberReference
		Implements IMethodReference
		Protected ReadOnly m_UnderlyingMethod As MethodSymbol

		Public Overridable ReadOnly Property AsGenericMethodInstanceReference As IGenericMethodInstanceReference Implements IMethodReference.AsGenericMethodInstanceReference
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable ReadOnly Property AsSpecializedMethodReference As ISpecializedMethodReference Implements IMethodReference.AsSpecializedMethodReference
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property IMethodReferenceAcceptsExtraArguments As Boolean Implements IMethodReference.AcceptsExtraArguments
			Get
				Return Me.m_UnderlyingMethod.IsVararg
			End Get
		End Property

		ReadOnly Property IMethodReferenceExtraParameters As ImmutableArray(Of IParameterTypeInformation) Implements IMethodReference.ExtraParameters
			Get
				Return ImmutableArray(Of IParameterTypeInformation).Empty
			End Get
		End Property

		ReadOnly Property IMethodReferenceGenericParameterCount As UShort Implements IMethodReference.GenericParameterCount
			Get
				Return CUShort(Me.m_UnderlyingMethod.Arity)
			End Get
		End Property

		ReadOnly Property IMethodReferenceIsGeneric As Boolean Implements IMethodReference.IsGeneric
			Get
				Return Me.m_UnderlyingMethod.IsGenericMethod
			End Get
		End Property

		ReadOnly Property IMethodReferenceParameterCount As UShort
			Get
				Return CUShort(Me.m_UnderlyingMethod.ParameterCount)
			End Get
		End Property

		ReadOnly Property ISignatureCallingConvention As CallingConvention
			Get
				Return Me.m_UnderlyingMethod.CallingConvention
			End Get
		End Property

		ReadOnly Property ISignatureRefCustomModifiers As ImmutableArray(Of ICustomModifier)
			Get
				Return Me.m_UnderlyingMethod.RefCustomModifiers.[As](Of ICustomModifier)()
			End Get
		End Property

		ReadOnly Property ISignatureReturnValueCustomModifiers As ImmutableArray(Of ICustomModifier)
			Get
				Return Me.m_UnderlyingMethod.ReturnTypeCustomModifiers.[As](Of ICustomModifier)()
			End Get
		End Property

		ReadOnly Property ISignatureReturnValueIsByRef As Boolean
			Get
				Return Me.m_UnderlyingMethod.ReturnsByRef
			End Get
		End Property

		Protected Overrides ReadOnly Property UnderlyingSymbol As Symbol
			Get
				Return Me.m_UnderlyingMethod
			End Get
		End Property

		Public Sub New(ByVal underlyingMethod As MethodSymbol)
			MyBase.New()
			Me.m_UnderlyingMethod = underlyingMethod
		End Sub

		Private Function IMethodReferenceGetResolvedMethod(ByVal context As EmitContext) As IMethodDefinition Implements IMethodReference.GetResolvedMethod
			Return Nothing
		End Function

		Private Function ISignatureGetParameters(ByVal context As EmitContext) As ImmutableArray(Of IParameterTypeInformation)
			Return DirectCast(context.[Module], PEModuleBuilder).Translate(Me.m_UnderlyingMethod.Parameters)
		End Function

		Private Function ISignatureGetType(ByVal context As EmitContext) As ITypeReference
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Dim returnType As TypeSymbol = Me.m_UnderlyingMethod.ReturnType
			Return [module].Translate(returnType, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
		End Function
	End Class
End Namespace