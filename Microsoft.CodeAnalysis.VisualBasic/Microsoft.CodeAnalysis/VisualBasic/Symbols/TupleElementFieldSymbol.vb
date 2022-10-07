Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class TupleElementFieldSymbol
		Inherits TupleFieldSymbol
		Private ReadOnly _locations As ImmutableArray(Of Location)

		Private ReadOnly _isImplicitlyDeclared As Boolean

		Private ReadOnly _correspondingDefaultField As TupleElementFieldSymbol

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
				If (CObj(Me._underlyingField.ContainingType) = CObj(Me._containingTuple.TupleUnderlyingType)) Then
					symbol = MyBase.AssociatedSymbol
				Else
					symbol = Nothing
				End If
				Return symbol
			End Get
		End Property

		Public Overrides ReadOnly Property CorrespondingTupleField As FieldSymbol
			Get
				Return Me._correspondingDefaultField
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				If (Me._isImplicitlyDeclared) Then
					Return ImmutableArray(Of SyntaxReference).Empty
				End If
				Return Symbol.GetDeclaringSyntaxReferenceHelper(Of VisualBasicSyntaxNode)(Me._locations)
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._isImplicitlyDeclared
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._locations
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeLayoutOffset As Nullable(Of Integer)
			Get
				Dim nullable As Nullable(Of Integer)
				If (CObj(Me._underlyingField.ContainingType) = CObj(Me._containingTuple.TupleUnderlyingType)) Then
					nullable = MyBase.TypeLayoutOffset
				Else
					nullable = Nothing
				End If
				Return nullable
			End Get
		End Property

		Public Sub New(ByVal container As TupleTypeSymbol, ByVal underlyingField As FieldSymbol, ByVal tupleElementIndex As Integer, ByVal location As Microsoft.CodeAnalysis.Location, ByVal isImplicitlyDeclared As Boolean, ByVal correspondingDefaultFieldOpt As TupleElementFieldSymbol)
			MyBase.New(container, underlyingField, If(correspondingDefaultFieldOpt Is Nothing, tupleElementIndex << 1, tupleElementIndex << 1 + 1))
			Dim empty As ImmutableArray(Of Microsoft.CodeAnalysis.Location)
			If (location Is Nothing) Then
				empty = ImmutableArray(Of Microsoft.CodeAnalysis.Location).Empty
			Else
				empty = ImmutableArray.Create(Of Microsoft.CodeAnalysis.Location)(location)
			End If
			Me._locations = empty
			Me._isImplicitlyDeclared = isImplicitlyDeclared
			Me._correspondingDefaultField = If(correspondingDefaultFieldOpt, Me)
		End Sub
	End Class
End Namespace