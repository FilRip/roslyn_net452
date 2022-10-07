Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SynthesizedMethodBase
		Inherits MethodSymbol
		Protected ReadOnly m_containingType As NamedTypeSymbol

		Private _lazyMeParameter As ParameterSymbol

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return 0
			End Get
		End Property

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return DirectCast((If(Me.IsShared, 0, 32) Or If(Me.IsGenericMethod, 16, 0)), Microsoft.Cci.CallingConvention)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me.m_containingType
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me.m_containingType
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Return ImmutableArray(Of MethodSymbol).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				If (Me.m_containingType.IsComImport AndAlso Not Me.m_containingType.IsInterface) Then
					Return MethodImplAttributes.CodeTypeMask Or MethodImplAttributes.Native Or MethodImplAttributes.OPTIL Or MethodImplAttributes.Runtime Or MethodImplAttributes.InternalCall
				End If
				Return MethodImplAttributes.IL
			End Get
		End Property

		Public Overrides ReadOnly Property IsAsync As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsExternalMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsInitOnly As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsIterator As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMethodKindBasedOnSyntax As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsVararg As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property OverriddenMethod As MethodSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return ImmutableArray(Of ParameterSymbol).Empty
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ReturnTypeMarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
			Get
				Return ImmutableArray(Of TypeSymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return ImmutableArray(Of TypeParameterSymbol).Empty
			End Get
		End Property

		Protected Sub New(ByVal container As NamedTypeSymbol)
			MyBase.New()
			Me.m_containingType = container
		End Sub

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return ImmutableArray(Of String).Empty
		End Function

		Public NotOverridable Overrides Function GetDllImportData() As DllImportData
			Return Nothing
		End Function

		Public Overrides Function GetReturnTypeAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return ImmutableArray(Of VisualBasicAttributeData).Empty
		End Function

		Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend NotOverridable Overrides Function TryGetMeParameter(<Out> ByRef meParameter As ParameterSymbol) As Boolean
			If (Not Me.IsShared) Then
				If (Me._lazyMeParameter Is Nothing) Then
					Interlocked.CompareExchange(Of ParameterSymbol)(Me._lazyMeParameter, New MeParameterSymbol(Me), Nothing)
				End If
				meParameter = Me._lazyMeParameter
			Else
				meParameter = Nothing
			End If
			Return True
		End Function
	End Class
End Namespace