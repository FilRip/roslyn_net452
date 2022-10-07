Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class CatchBlockBinder
		Inherits BlockBaseBinder
		Private ReadOnly _syntax As CatchBlockSyntax

		Private _locals As ImmutableArray(Of LocalSymbol)

		Friend Overrides ReadOnly Property Locals As ImmutableArray(Of LocalSymbol)
			Get
				If (Me._locals.IsDefault) Then
					Dim localSymbols As ImmutableArray(Of LocalSymbol) = Me.BuildLocals()
					Dim localSymbols1 As ImmutableArray(Of LocalSymbol) = New ImmutableArray(Of LocalSymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of LocalSymbol)(Me._locals, localSymbols, localSymbols1)
				End If
				Return Me._locals
			End Get
		End Property

		Public Sub New(ByVal enclosing As Binder, ByVal syntax As CatchBlockSyntax)
			MyBase.New(enclosing)
			Me._locals = New ImmutableArray(Of LocalSymbol)()
			Me._syntax = syntax
		End Sub

		Private Function BuildLocals() As ImmutableArray(Of LocalSymbol)
			Dim empty As ImmutableArray(Of LocalSymbol)
			Dim catchStatement As CatchStatementSyntax = Me._syntax.CatchStatement
			Dim asClause As SimpleAsClauseSyntax = catchStatement.AsClause
			If (asClause Is Nothing) Then
				empty = ImmutableArray(Of LocalSymbol).Empty
			Else
				empty = ImmutableArray.Create(Of LocalSymbol)(LocalSymbol.Create(Me.ContainingMember, Me, catchStatement.IdentifierName.Identifier, Nothing, asClause, Nothing, LocalDeclarationKind.[Catch]))
			End If
			Return empty
		End Function
	End Class
End Namespace