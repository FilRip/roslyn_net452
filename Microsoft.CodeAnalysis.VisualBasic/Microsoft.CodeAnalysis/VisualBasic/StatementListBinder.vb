Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class StatementListBinder
		Inherits BlockBaseBinder
		Private ReadOnly _statementList As SyntaxList(Of StatementSyntax)

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

		Public Sub New(ByVal containing As Binder, ByVal statementList As SyntaxList(Of StatementSyntax))
			MyBase.New(containing)
			Me._locals = New ImmutableArray(Of LocalSymbol)()
			Me._statementList = statementList
		End Sub

		Private Function BuildLocals() As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
			Dim empty As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
			Dim flag As Boolean
			Dim initializer As EqualsValueSyntax
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol) = Nothing
			Dim enumerator As SyntaxList(Of StatementSyntax).Enumerator = Me._statementList.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As StatementSyntax = enumerator.Current
				If (current.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LocalDeclarationStatement) Then
					Continue While
				End If
				If (instance Is Nothing) Then
					instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).GetInstance()
				End If
				Dim localDeclarationStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax)
				Dim localDeclarationKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalDeclarationKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalDeclarationKind.Variable
				Dim enumerator1 As SyntaxTokenList.Enumerator = localDeclarationStatementSyntax.Modifiers.GetEnumerator()
				While enumerator1.MoveNext()
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = enumerator1.Current.Kind()
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstKeyword) Then
						localDeclarationKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalDeclarationKind.Constant
						Exit While
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword) Then
						localDeclarationKind = Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalDeclarationKind.[Static]
					End If
				End While
				Dim enumerator2 As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax).Enumerator = localDeclarationStatementSyntax.Declarators.GetEnumerator()
				While enumerator2.MoveNext()
					Dim variableDeclaratorSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax = enumerator2.Current
					Dim asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax = variableDeclaratorSyntax.AsClause
					If (variableDeclaratorSyntax.Initializer Is Nothing) Then
						flag = False
					Else
						flag = If(variableDeclaratorSyntax.AsClause Is Nothing, True, variableDeclaratorSyntax.AsClause.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause)
					End If
					Dim flag1 As Boolean = flag
					Dim names As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax) = variableDeclaratorSyntax.Names
					Dim count As Integer = names.Count - 1
					For i As Integer = 0 To count
						Dim item As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax = names(i)
						Dim containingMember As Symbol = Me.ContainingMember
						Dim identifier As SyntaxToken = item.Identifier
						Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax = item
						Dim asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax = asClause
						If (Not flag1 OrElse i <> names.Count - 1) Then
							initializer = Nothing
						Else
							initializer = variableDeclaratorSyntax.Initializer
						End If
						Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol.Create(containingMember, Me, identifier, modifiedIdentifierSyntax, asClauseSyntax, initializer, localDeclarationKind)
						instance.Add(localSymbol)
					Next

				End While
			End While
			If (instance Is Nothing) Then
				empty = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Empty
			Else
				empty = instance.ToImmutableAndFree()
			End If
			Return empty
		End Function
	End Class
End Namespace