Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class ReducedExtensionPropertySymbol
		Inherits PropertySymbol
		Private ReadOnly _originalDefinition As PropertySymbol

		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
			Get
				Return Me._originalDefinition.AssociatedField
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return Me._originalDefinition.CallingConvention
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._originalDefinition.ContainingSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me._originalDefinition.DeclaredAccessibility
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._originalDefinition.DeclaringSyntaxReferences
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of PropertySymbol)
			Get
				Return ImmutableArray(Of PropertySymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property GetMethod As MethodSymbol
			Get
				Return Me.ReduceAccessorIfAny(Me._originalDefinition.GetMethod)
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me._originalDefinition.HasSpecialName
			End Get
		End Property

		Public Overrides ReadOnly Property IsDefault As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMyGroupCollectionProperty As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._originalDefinition.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._originalDefinition.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me._originalDefinition.ObsoleteAttributeData
			End Get
		End Property

		Public Overrides ReadOnly Property ParameterCount As Integer
			Get
				Return 0
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return ImmutableArray(Of ParameterSymbol).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property ReceiverType As TypeSymbol
			Get
				Return Me._originalDefinition.Parameters(0).Type
			End Get
		End Property

		Friend Overrides ReadOnly Property ReducedFrom As PropertySymbol
			Get
				Return Me._originalDefinition
			End Get
		End Property

		Friend Overrides ReadOnly Property ReducedFromDefinition As PropertySymbol
			Get
				Return Me._originalDefinition
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._originalDefinition.RefCustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return Me._originalDefinition.ReturnsByRef
			End Get
		End Property

		Public Overrides ReadOnly Property SetMethod As MethodSymbol
			Get
				Return Me.ReduceAccessorIfAny(Me._originalDefinition.SetMethod)
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._originalDefinition.Type
			End Get
		End Property

		Public Overrides ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._originalDefinition.TypeCustomModifiers
			End Get
		End Property

		Public Sub New(ByVal originalDefinition As PropertySymbol)
			MyBase.New()
			Me._originalDefinition = originalDefinition
		End Sub

		Private Function ReduceAccessorIfAny(ByVal methodOpt As MethodSymbol) As ReducedExtensionPropertySymbol.ReducedExtensionAccessorSymbol
			If (methodOpt Is Nothing) Then
				Return Nothing
			End If
			Return New ReducedExtensionPropertySymbol.ReducedExtensionAccessorSymbol(Me, methodOpt)
		End Function

		Private NotInheritable Class ReducedAccessorParameterSymbol
			Inherits ReducedParameterSymbolBase
			Private ReadOnly _propertyOrAccessor As Symbol

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me._propertyOrAccessor
				End Get
			End Property

			Public Overrides ReadOnly Property Type As TypeSymbol
				Get
					Return Me.m_CurriedFromParameter.Type
				End Get
			End Property

			Public Sub New(ByVal propertyOrAccessor As Symbol, ByVal underlyingParameter As ParameterSymbol)
				MyBase.New(underlyingParameter)
				Me._propertyOrAccessor = propertyOrAccessor
			End Sub

			Public Shared Function MakeParameters(ByVal propertyOrAccessor As Symbol, ByVal originalParameters As ImmutableArray(Of ParameterSymbol)) As ImmutableArray(Of ParameterSymbol)
				Dim empty As ImmutableArray(Of ParameterSymbol)
				Dim length As Integer = originalParameters.Length
				If (length > 1) Then
					Dim reducedAccessorParameterSymbol(length - 2 + 1 - 1) As ParameterSymbol
					Dim num As Integer = length - 2
					Dim num1 As Integer = 0
					Do
						reducedAccessorParameterSymbol(num1) = New ReducedExtensionPropertySymbol.ReducedAccessorParameterSymbol(propertyOrAccessor, originalParameters(num1 + 1))
						num1 = num1 + 1
					Loop While num1 <= num
					empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of ParameterSymbol)(reducedAccessorParameterSymbol)
				Else
					empty = ImmutableArray(Of ParameterSymbol).Empty
				End If
				Return empty
			End Function
		End Class

		Private NotInheritable Class ReducedExtensionAccessorSymbol
			Inherits MethodSymbol
			Private ReadOnly _associatedProperty As ReducedExtensionPropertySymbol

			Private ReadOnly _originalDefinition As MethodSymbol

			Private _lazyParameters As ImmutableArray(Of ParameterSymbol)

			Public Overrides ReadOnly Property Arity As Integer
				Get
					Return 0
				End Get
			End Property

			Public Overrides ReadOnly Property AssociatedSymbol As Symbol
				Get
					Return Me._associatedProperty
				End Get
			End Property

			Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
				Get
					Return Microsoft.Cci.CallingConvention.HasThis
				End Get
			End Property

			Friend Overrides ReadOnly Property CallsiteReducedFromMethod As MethodSymbol
				Get
					Return Me._originalDefinition
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me._originalDefinition.ContainingSymbol
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
				Get
					Return Me._originalDefinition.DeclaredAccessibility
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Return Me._originalDefinition.DeclaringSyntaxReferences
				End Get
			End Property

			Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
				Get
					Return ImmutableArray(Of MethodSymbol).Empty
				End Get
			End Property

			Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
				Get
					Return Me._originalDefinition.GenerateDebugInfo
				End Get
			End Property

			Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
				Get
					Return Me._originalDefinition.HasDeclarativeSecurity
				End Get
			End Property

			Friend Overrides ReadOnly Property HasSpecialName As Boolean
				Get
					Return Me._originalDefinition.HasSpecialName
				End Get
			End Property

			Friend Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
				Get
					Return Me._originalDefinition.ImplementationAttributes
				End Get
			End Property

			Public Overrides ReadOnly Property IsAsync As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsExtensionMethod As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsExternalMethod As Boolean
				Get
					Return Me._originalDefinition.IsExternalMethod
				End Get
			End Property

			Public Overrides ReadOnly Property IsInitOnly As Boolean
				Get
					Return Me._originalDefinition.IsInitOnly
				End Get
			End Property

			Public Overrides ReadOnly Property IsIterator As Boolean
				Get
					Return False
				End Get
			End Property

			Friend Overrides ReadOnly Property IsMethodKindBasedOnSyntax As Boolean
				Get
					Return Me._originalDefinition.IsMethodKindBasedOnSyntax
				End Get
			End Property

			Public Overrides ReadOnly Property IsMustOverride As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsNotOverridable As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsOverloads As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsOverridable As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsOverrides As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsShared As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsSub As Boolean
				Get
					Return Me._originalDefinition.IsSub
				End Get
			End Property

			Public Overrides ReadOnly Property IsVararg As Boolean
				Get
					Return Me._originalDefinition.IsVararg
				End Get
			End Property

			Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
				Get
					Return Me._originalDefinition.Locations
				End Get
			End Property

			Public Overrides ReadOnly Property MethodKind As MethodKind
				Get
					Return Me._originalDefinition.MethodKind
				End Get
			End Property

			Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
				Get
					Return Me._originalDefinition.ObsoleteAttributeData
				End Get
			End Property

			Friend Overrides ReadOnly Property ParameterCount As Integer
				Get
					Return Me._originalDefinition.ParameterCount - 1
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					If (Me._lazyParameters.IsDefault) Then
						ImmutableInterlocked.InterlockedInitialize(Of ParameterSymbol)(Me._lazyParameters, ReducedExtensionPropertySymbol.ReducedAccessorParameterSymbol.MakeParameters(Me, Me._originalDefinition.Parameters))
					End If
					Return Me._lazyParameters
				End Get
			End Property

			Public Overrides ReadOnly Property ReducedFrom As MethodSymbol
				Get
					Return Me._originalDefinition
				End Get
			End Property

			Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return Me._originalDefinition.RefCustomModifiers
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnsByRef As Boolean
				Get
					Return Me._originalDefinition.ReturnsByRef
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnType As TypeSymbol
				Get
					Return Me._originalDefinition.ReturnType
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return Me._originalDefinition.ReturnTypeCustomModifiers
				End Get
			End Property

			Friend Overrides ReadOnly Property ReturnTypeMarshallingInformation As MarshalPseudoCustomAttributeData
				Get
					Return Me._originalDefinition.ReturnTypeMarshallingInformation
				End Get
			End Property

			Friend Overrides ReadOnly Property Syntax As SyntaxNode
				Get
					Return Me._originalDefinition.Syntax
				End Get
			End Property

			Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
				Get
					Return Me._originalDefinition.TypeArguments
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return Me._originalDefinition.TypeParameters
				End Get
			End Property

			Public Sub New(ByVal associatedProperty As ReducedExtensionPropertySymbol, ByVal originalDefinition As MethodSymbol)
				MyBase.New()
				Me._associatedProperty = associatedProperty
				Me._originalDefinition = originalDefinition
			End Sub

			Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
				Throw ExceptionUtilities.Unreachable
			End Function

			Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
				Return Me._originalDefinition.GetAppliedConditionalSymbols()
			End Function

			Public Overrides Function GetDllImportData() As DllImportData
				Return Me._originalDefinition.GetDllImportData()
			End Function

			Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
				Return Me._originalDefinition.GetSecurityInformation()
			End Function

			Friend Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
				Return False
			End Function
		End Class
	End Class
End Namespace