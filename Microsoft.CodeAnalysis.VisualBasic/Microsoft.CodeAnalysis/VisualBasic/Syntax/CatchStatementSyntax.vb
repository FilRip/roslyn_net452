Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CatchStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
		Friend _identifierName As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax

		Friend _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax

		Friend _whenClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax

		Public ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)(Me._asClause, 2)
			End Get
		End Property

		Public ReadOnly Property CatchKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax)._catchKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property IdentifierName As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)(Me._identifierName, 1)
			End Get
		End Property

		Public ReadOnly Property WhenClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax)(Me._whenClause, 3)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal catchKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal identifierName As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax, ByVal whenClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax(kind, errors, annotations, catchKeyword, If(identifierName IsNot Nothing, DirectCast(identifierName.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax), Nothing), If(asClause IsNot Nothing, DirectCast(asClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax), Nothing), If(whenClause IsNot Nothing, DirectCast(whenClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCatchStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCatchStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 1
					syntaxNode = Me._identifierName
					Exit Select
				Case 2
					syntaxNode = Me._asClause
					Exit Select
				Case 3
					syntaxNode = Me._whenClause
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim identifierName As SyntaxNode
			Select Case i
				Case 1
					identifierName = Me.IdentifierName
					Exit Select
				Case 2
					identifierName = Me.AsClause
					Exit Select
				Case 3
					identifierName = Me.WhenClause
					Exit Select
				Case Else
					identifierName = Nothing
					Exit Select
			End Select
			Return identifierName
		End Function

		Public Function Update(ByVal catchKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal identifierName As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax, ByVal whenClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax
			Dim catchStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax
			If (catchKeyword <> Me.CatchKeyword OrElse identifierName <> Me.IdentifierName OrElse asClause <> Me.AsClause OrElse whenClause <> Me.WhenClause) Then
				Dim catchStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CatchStatement(catchKeyword, identifierName, asClause, whenClause)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				catchStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, catchStatementSyntax1, catchStatementSyntax1.WithAnnotations(annotations))
			Else
				catchStatementSyntax = Me
			End If
			Return catchStatementSyntax
		End Function

		Public Function WithAsClause(ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax
			Return Me.Update(Me.CatchKeyword, Me.IdentifierName, asClause, Me.WhenClause)
		End Function

		Public Function WithCatchKeyword(ByVal catchKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax
			Return Me.Update(catchKeyword, Me.IdentifierName, Me.AsClause, Me.WhenClause)
		End Function

		Public Function WithIdentifierName(ByVal identifierName As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax
			Return Me.Update(Me.CatchKeyword, identifierName, Me.AsClause, Me.WhenClause)
		End Function

		Public Function WithWhenClause(ByVal whenClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax
			Return Me.Update(Me.CatchKeyword, Me.IdentifierName, Me.AsClause, whenClause)
		End Function
	End Class
End Namespace