Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ErrorStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _errorNumber As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property ErrorKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax)._errorKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property ErrorNumber As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._errorNumber, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal errorKeyword As KeywordSyntax, ByVal errorNumber As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax(kind, errors, annotations, errorKeyword, DirectCast(errorNumber.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitErrorStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitErrorStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._errorNumber
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim errorNumber As SyntaxNode
			If (i <> 1) Then
				errorNumber = Nothing
			Else
				errorNumber = Me.ErrorNumber
			End If
			Return errorNumber
		End Function

		Public Function Update(ByVal errorKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal errorNumber As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ErrorStatementSyntax
			Dim errorStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ErrorStatementSyntax
			If (errorKeyword <> Me.ErrorKeyword OrElse errorNumber <> Me.ErrorNumber) Then
				Dim errorStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ErrorStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ErrorStatement(errorKeyword, errorNumber)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				errorStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, errorStatementSyntax1, errorStatementSyntax1.WithAnnotations(annotations))
			Else
				errorStatementSyntax = Me
			End If
			Return errorStatementSyntax
		End Function

		Public Function WithErrorKeyword(ByVal errorKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ErrorStatementSyntax
			Return Me.Update(errorKeyword, Me.ErrorNumber)
		End Function

		Public Function WithErrorNumber(ByVal errorNumber As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ErrorStatementSyntax
			Return Me.Update(Me.ErrorKeyword, errorNumber)
		End Function
	End Class
End Namespace