Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SignatureOnlyPropertySymbol
		Inherits PropertySymbol
		Private ReadOnly _name As String

		Private ReadOnly _containingType As NamedTypeSymbol

		Private ReadOnly _isReadOnly As Boolean

		Private ReadOnly _isWriteOnly As Boolean

		Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

		Private ReadOnly _returnsByRef As Boolean

		Private ReadOnly _type As TypeSymbol

		Private ReadOnly _typeCustomModifiers As ImmutableArray(Of CustomModifier)

		Private ReadOnly _refCustomModifiers As ImmutableArray(Of CustomModifier)

		Private ReadOnly _isOverrides As Boolean

		Private ReadOnly _isWithEvents As Boolean

		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Throw ExceptionUtilities.Unreachable
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
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of PropertySymbol)
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property GetMethod As MethodSymbol
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsDefault As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMyGroupCollectionProperty As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return Me._isOverrides
			End Get
		End Property

		Public Overrides ReadOnly Property IsReadOnly As Boolean
			Get
				Return Me._isReadOnly
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsWithEvents As Boolean
			Get
				Return Me._isWithEvents
			End Get
		End Property

		Public Overrides ReadOnly Property IsWriteOnly As Boolean
			Get
				Return Me._isWriteOnly
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property OverriddenMembers As OverriddenMembersResult(Of PropertySymbol)
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._parameters
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._refCustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return Me._returnsByRef
			End Get
		End Property

		Public Overrides ReadOnly Property SetMethod As MethodSymbol
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._type
			End Get
		End Property

		Public Overrides ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._typeCustomModifiers
			End Get
		End Property

		Public Sub New(ByVal name As String, ByVal containingType As NamedTypeSymbol, ByVal isReadOnly As Boolean, ByVal isWriteOnly As Boolean, ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal returnsByRef As Boolean, ByVal type As TypeSymbol, ByVal typeCustomModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier), Optional ByVal isOverrides As Boolean = False, Optional ByVal isWithEvents As Boolean = False)
			MyBase.New()
			Me._name = name
			Me._containingType = containingType
			Me._isReadOnly = isReadOnly
			Me._isWriteOnly = isWriteOnly
			Me._parameters = parameters
			Me._returnsByRef = returnsByRef
			Me._type = type
			Me._typeCustomModifiers = typeCustomModifiers
			Me._refCustomModifiers = refCustomModifiers
			Me._isOverrides = isOverrides
			Me._isWithEvents = isWithEvents
		End Sub
	End Class
End Namespace