Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedOverridingWithEventsProperty
		Inherits PropertySymbol
		Private ReadOnly _baseProperty As PropertySymbol

		Private ReadOnly _containingType As SourceNamedTypeSymbol

		Private ReadOnly _getter As SynthesizedWithEventsAccessorSymbol

		Private ReadOnly _setter As SynthesizedWithEventsAccessorSymbol

		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return Me._baseProperty.CallingConvention
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
				Return Me._baseProperty.DeclaredAccessibility
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of PropertySymbol)
			Get
				Return ImmutableArray(Of PropertySymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property GetMethod As MethodSymbol
			Get
				Return Me._getter
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsDefault As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
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
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsWithEvents As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._containingType.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._baseProperty.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
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

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._baseProperty.RefCustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return Me._baseProperty.ReturnsByRef
			End Get
		End Property

		Public Overrides ReadOnly Property SetMethod As MethodSymbol
			Get
				Return Me._setter
			End Get
		End Property

		Friend Overrides ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._baseProperty.Type
			End Get
		End Property

		Public Overrides ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._baseProperty.TypeCustomModifiers
			End Get
		End Property

		Friend Sub New(ByVal baseProperty As PropertySymbol, ByVal container As SourceNamedTypeSymbol)
			MyBase.New()
			Me._baseProperty = baseProperty
			Me._containingType = container
			Me._getter = New SynthesizedWithEventsGetAccessorSymbol(container, Me)
			Me._setter = New SynthesizedWithEventsSetAccessorSymbol(container, Me, baseProperty.SetMethod.ReturnType, "WithEventsValue")
		End Sub

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return Me._containingType.GetLexicalSortKey()
		End Function
	End Class
End Namespace