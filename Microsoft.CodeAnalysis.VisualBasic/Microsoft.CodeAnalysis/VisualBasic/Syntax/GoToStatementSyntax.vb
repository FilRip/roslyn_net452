Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class GoToStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax

		Public ReadOnly Property GoToKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax)._goToKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax)(Me._label, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal goToKeyword As KeywordSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax(kind, errors, annotations, goToKeyword, DirectCast(label.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitGoToStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitGoToStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._label
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim label As SyntaxNode
			If (i <> 1) Then
				label = Nothing
			Else
				label = Me.Label
			End If
			Return label
		End Function

		Public Function Update(ByVal goToKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GoToStatementSyntax
			Dim goToStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GoToStatementSyntax
			If (goToKeyword <> Me.GoToKeyword OrElse label <> Me.Label) Then
				Dim statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.GoToStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.GoToStatement(goToKeyword, label)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				goToStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, statement, statement.WithAnnotations(annotations))
			Else
				goToStatementSyntax = Me
			End If
			Return goToStatementSyntax
		End Function

		Public Function WithGoToKeyword(ByVal goToKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GoToStatementSyntax
			Return Me.Update(goToKeyword, Me.Label)
		End Function

		Public Function WithLabel(ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GoToStatementSyntax
			Return Me.Update(Me.GoToKeyword, label)
		End Function
	End Class
End Namespace