Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class DoStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
		Friend _whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax

		Public ReadOnly Property DoKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax)._doKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property WhileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax)(Me._whileOrUntilClause, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal doKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(kind, errors, annotations, doKeyword, If(whileOrUntilClause IsNot Nothing, DirectCast(whileOrUntilClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitDoStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitDoStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._whileOrUntilClause
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim whileOrUntilClause As SyntaxNode
			If (i <> 1) Then
				whileOrUntilClause = Nothing
			Else
				whileOrUntilClause = Me.WhileOrUntilClause
			End If
			Return whileOrUntilClause
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal doKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax
			Dim doStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax
			If (kind <> MyBase.Kind() OrElse doKeyword <> Me.DoKeyword OrElse whileOrUntilClause <> Me.WhileOrUntilClause) Then
				Dim doStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.DoStatement(kind, doKeyword, whileOrUntilClause)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				doStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, doStatementSyntax1, doStatementSyntax1.WithAnnotations(annotations))
			Else
				doStatementSyntax = Me
			End If
			Return doStatementSyntax
		End Function

		Public Function WithDoKeyword(ByVal doKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax
			Return Me.Update(MyBase.Kind(), doKeyword, Me.WhileOrUntilClause)
		End Function

		Public Function WithWhileOrUntilClause(ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.DoKeyword, whileOrUntilClause)
		End Function
	End Class
End Namespace