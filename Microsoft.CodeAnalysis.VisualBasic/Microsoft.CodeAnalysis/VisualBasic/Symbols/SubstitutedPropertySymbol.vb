Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SubstitutedPropertySymbol
		Inherits PropertySymbol
		Private ReadOnly _containingType As SubstitutedNamedType

		Private ReadOnly _originalDefinition As PropertySymbol

		Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

		Private ReadOnly _getMethod As SubstitutedMethodSymbol

		Private ReadOnly _setMethod As SubstitutedMethodSymbol

		Private ReadOnly _associatedField As SubstitutedFieldSymbol

		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
			Get
				Return Me._associatedField
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return Me._originalDefinition.CallingConvention
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me._containingType
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
				Return ImplementsHelper.SubstituteExplicitInterfaceImplementations(Of PropertySymbol)(Me._originalDefinition.ExplicitInterfaceImplementations, Me.TypeSubstitution)
			End Get
		End Property

		Public Overrides ReadOnly Property GetMethod As MethodSymbol
			Get
				Return Me._getMethod
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me._originalDefinition.HasSpecialName
			End Get
		End Property

		Public Overrides ReadOnly Property IsDefault As Boolean
			Get
				Return Me._originalDefinition.IsDefault
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._originalDefinition.IsImplicitlyDeclared
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return Me._originalDefinition.IsMustOverride
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMyGroupCollectionProperty As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return Me._originalDefinition.IsNotOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Return Me._originalDefinition.IsOverloads
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return Me._originalDefinition.IsOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return Me._originalDefinition.IsOverrides
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me._originalDefinition.IsShared
			End Get
		End Property

		Public Overrides ReadOnly Property IsWithEvents As Boolean
			Get
				Return Me._originalDefinition.IsWithEvents
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._originalDefinition.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._originalDefinition.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._originalDefinition.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me.OriginalDefinition.ObsoleteAttributeData
			End Get
		End Property

		Public Overrides ReadOnly Property OriginalDefinition As PropertySymbol
			Get
				Return Me._originalDefinition
			End Get
		End Property

		Public Overrides ReadOnly Property ParameterCount As Integer
			Get
				Return Me._originalDefinition.ParameterCount
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._parameters
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.TypeSubstitution.SubstituteCustomModifiers(Me._originalDefinition.RefCustomModifiers)
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return Me._originalDefinition.ReturnsByRef
			End Get
		End Property

		Public Overrides ReadOnly Property SetMethod As MethodSymbol
			Get
				Return Me._setMethod
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._originalDefinition.Type.InternalSubstituteTypeParameters(Me.TypeSubstitution).Type
			End Get
		End Property

		Public Overrides ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.TypeSubstitution.SubstituteCustomModifiers(Me._originalDefinition.Type, Me._originalDefinition.TypeCustomModifiers)
			End Get
		End Property

		Friend ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Get
				Return Me._containingType.TypeSubstitution
			End Get
		End Property

		Public Sub New(ByVal container As SubstitutedNamedType, ByVal originalDefinition As PropertySymbol, ByVal getMethod As SubstitutedMethodSymbol, ByVal setMethod As SubstitutedMethodSymbol, ByVal associatedField As SubstitutedFieldSymbol)
			MyBase.New()
			Me._containingType = container
			Me._originalDefinition = originalDefinition
			Me._parameters = Me.SubstituteParameters()
			Me._getMethod = getMethod
			Me._setMethod = setMethod
			Me._associatedField = associatedField
			If (Me._getMethod IsNot Nothing) Then
				Me._getMethod.SetAssociatedPropertyOrEvent(Me)
			End If
			If (Me._setMethod IsNot Nothing) Then
				Me._setMethod.SetAssociatedPropertyOrEvent(Me)
			End If
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (Me <> obj) Then
				Dim substitutedPropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedPropertySymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedPropertySymbol)
				If (substitutedPropertySymbol Is Nothing) Then
					flag = False
				ElseIf (Me._originalDefinition.Equals(substitutedPropertySymbol._originalDefinition)) Then
					flag = If(Me._containingType.Equals(substitutedPropertySymbol._containingType), True, False)
				Else
					flag = False
				End If
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._originalDefinition.GetAttributes()
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._originalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim hashCode As Integer = Me._originalDefinition.GetHashCode()
			Return Hash.Combine(Of SubstitutedNamedType)(Me._containingType, hashCode)
		End Function

		Private Function SubstituteParameters() As ImmutableArray(Of ParameterSymbol)
			Dim empty As ImmutableArray(Of ParameterSymbol)
			Dim parameters As ImmutableArray(Of ParameterSymbol) = Me._originalDefinition.Parameters
			Dim length As Integer = parameters.Length
			If (length <> 0) Then
				Dim parameterSymbolArray(length - 1 + 1 - 1) As ParameterSymbol
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					parameterSymbolArray(num1) = SubstitutedParameterSymbol.CreatePropertyParameter(Me, parameters(num1))
					num1 = num1 + 1
				Loop While num1 <= num
				empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of ParameterSymbol)(parameterSymbolArray)
			Else
				empty = ImmutableArray(Of ParameterSymbol).Empty
			End If
			Return empty
		End Function
	End Class
End Namespace