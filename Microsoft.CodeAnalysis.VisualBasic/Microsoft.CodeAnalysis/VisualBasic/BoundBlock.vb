Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundBlock
		Inherits BoundStatement
		Private ReadOnly _StatementListSyntax As SyntaxList(Of StatementSyntax)

		Private ReadOnly _Locals As ImmutableArray(Of LocalSymbol)

		Private ReadOnly _Statements As ImmutableArray(Of BoundStatement)

		Public ReadOnly Property Locals As ImmutableArray(Of LocalSymbol)
			Get
				Return Me._Locals
			End Get
		End Property

		Public ReadOnly Property StatementListSyntax As SyntaxList(Of StatementSyntax)
			Get
				Return Me._StatementListSyntax
			End Get
		End Property

		Public ReadOnly Property Statements As ImmutableArray(Of BoundStatement)
			Get
				Return Me._Statements
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal statementListSyntax As SyntaxList(Of StatementSyntax), ByVal locals As ImmutableArray(Of LocalSymbol), ByVal statements As ImmutableArray(Of BoundStatement), Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.Block, syntax, If(hasErrors, True, statements.NonNullAndHasErrors()))
			Me._StatementListSyntax = statementListSyntax
			Me._Locals = locals
			Me._Statements = statements
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitBlock(Me)
		End Function

		Public Function Update(ByVal statementListSyntax As SyntaxList(Of StatementSyntax), ByVal locals As ImmutableArray(Of LocalSymbol), ByVal statements As ImmutableArray(Of BoundStatement)) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			If (statementListSyntax <> Me.StatementListSyntax OrElse locals <> Me.Locals OrElse statements <> Me.Statements) Then
				Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(MyBase.Syntax, statementListSyntax, locals, statements, MyBase.HasErrors)
				boundBlock1.CopyAttributes(Me)
				boundBlock = boundBlock1
			Else
				boundBlock = Me
			End If
			Return boundBlock
		End Function
	End Class
End Namespace