Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class MeParameterSymbol
		Inherits ParameterSymbol
		Private ReadOnly _container As Symbol

		Private ReadOnly _type As TypeSymbol

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._container
			End Get
		End Property

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasOptionCompare As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsByRef As Boolean
			Get
				Return Me.Type.IsValueType
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerFilePath As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerLineNumber As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerMemberName As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExplicitByRef As Boolean
			Get
				Return Me.IsByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIDispatchConstant As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIUnknownConstant As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsMe As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataIn As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataOut As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOptional As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsParamArray As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				If (Me._container Is Nothing) Then
					Return ImmutableArray(Of Location).Empty
				End If
				Return Me._container.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return "Me"
			End Get
		End Property

		Public Overrides ReadOnly Property Ordinal As Integer
			Get
				Return -1
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._type
			End Get
		End Property

		Friend Sub New(ByVal memberSymbol As Symbol)
			MyBase.New()
			Me._container = memberSymbol
			Me._type = Me._container.ContainingType
		End Sub

		Friend Sub New(ByVal memberSymbol As Symbol, ByVal type As TypeSymbol)
			MyBase.New()
			Me._container = memberSymbol
			Me._type = type
		End Sub
	End Class
End Namespace