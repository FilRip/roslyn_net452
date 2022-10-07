Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class UsingBlockBinder
		Inherits BlockBaseBinder
		Private ReadOnly _syntax As UsingBlockSyntax

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

		Public Sub New(ByVal enclosing As Binder, ByVal syntax As UsingBlockSyntax)
			MyBase.New(enclosing)
			Me._locals = New ImmutableArray(Of LocalSymbol)()
			Me._syntax = syntax
		End Sub

		Private Function BuildLocals() As ImmutableArray(Of LocalSymbol)
			Dim empty As ImmutableArray(Of LocalSymbol)
			Dim flag As Boolean
			Dim initializer As EqualsValueSyntax
			Dim variables As SeparatedSyntaxList(Of VariableDeclaratorSyntax) = Me._syntax.UsingStatement.Variables
			If (variables.Count <= 0) Then
				empty = ImmutableArray(Of LocalSymbol).Empty
			Else
				Dim instance As ArrayBuilder(Of LocalSymbol) = ArrayBuilder(Of LocalSymbol).GetInstance()
				Dim enumerator As SeparatedSyntaxList(Of VariableDeclaratorSyntax).Enumerator = variables.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As VariableDeclaratorSyntax = enumerator.Current
					If (current.Initializer Is Nothing) Then
						flag = False
					Else
						flag = If(current.AsClause Is Nothing, True, current.AsClause.Kind() <> SyntaxKind.AsNewClause)
					End If
					Dim flag1 As Boolean = flag
					Dim names As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax) = current.Names
					Dim count As Integer = names.Count - 1
					For i As Integer = 0 To count
						Dim item As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax = names(i)
						Dim localSymbols As ArrayBuilder(Of LocalSymbol) = instance
						Dim containingMember As Symbol = Me.ContainingMember
						Dim identifier As SyntaxToken = item.Identifier
						Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax = item
						Dim asClause As AsClauseSyntax = current.AsClause
						If (Not flag1 OrElse i <> names.Count - 1) Then
							initializer = Nothing
						Else
							initializer = current.Initializer
						End If
						localSymbols.Add(LocalSymbol.Create(containingMember, Me, identifier, modifiedIdentifierSyntax, asClause, initializer, LocalDeclarationKind.[Using]))
					Next

				End While
				empty = instance.ToImmutableAndFree()
			End If
			Return empty
		End Function
	End Class
End Namespace