Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SingleLineElseClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _statements As SyntaxNode

		Public ReadOnly Property ElseKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax)._elseKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal elseKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal statements As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax(kind, errors, annotations, elseKeyword, If(statements IsNot Nothing, statements.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSingleLineElseClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSingleLineElseClause(Me)
		End Sub

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._statements
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._statements, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal elseKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax
			Dim singleLineElseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax
			If (elseKeyword <> Me.ElseKeyword OrElse statements <> Me.Statements) Then
				Dim singleLineElseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SingleLineElseClause(elseKeyword, statements)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				singleLineElseClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, singleLineElseClauseSyntax1, singleLineElseClauseSyntax1.WithAnnotations(annotations))
			Else
				singleLineElseClauseSyntax = Me
			End If
			Return singleLineElseClauseSyntax
		End Function

		Public Function WithElseKeyword(ByVal elseKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax
			Return Me.Update(elseKeyword, Me.Statements)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax
			Return Me.Update(Me.ElseKeyword, statements)
		End Function
	End Class
End Namespace