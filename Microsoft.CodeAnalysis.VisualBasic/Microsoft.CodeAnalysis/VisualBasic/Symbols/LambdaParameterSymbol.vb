Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class LambdaParameterSymbol
		Inherits ParameterSymbol
		Private ReadOnly _location As ImmutableArray(Of Location)

		Private ReadOnly _name As String

		Private ReadOnly _type As TypeSymbol

		Private ReadOnly _ordinal As UShort

		Private ReadOnly _isByRef As Boolean

		Public NotOverridable Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Symbol.GetDeclaringSyntaxReferenceHelper(Of ParameterSyntax)(Me.Locations)
			End Get
		End Property

		Friend Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
			Get
				Return Nothing
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasOptionCompare As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsByRef As Boolean
			Get
				Return Me._isByRef
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

		Friend NotOverridable Overrides ReadOnly Property IsExplicitByRef As Boolean
			Get
				Return Me._isByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIDispatchConstant As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIUnknownConstant As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMetadataIn As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMetadataOut As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOptional As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsParamArray As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._location
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me._ordinal
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._type
			End Get
		End Property

		Protected Sub New(ByVal name As String, ByVal ordinal As Integer, ByVal type As TypeSymbol, ByVal isByRef As Boolean, ByVal location As Microsoft.CodeAnalysis.Location)
			MyBase.New()
			Me._name = name
			Me._ordinal = CUShort(ordinal)
			Me._type = type
			If (location Is Nothing) Then
				Me._location = ImmutableArray(Of Microsoft.CodeAnalysis.Location).Empty
			Else
				Me._location = ImmutableArray.Create(Of Microsoft.CodeAnalysis.Location)(location)
			End If
			Me._isByRef = isByRef
		End Sub
	End Class
End Namespace