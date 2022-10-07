Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class OnErrorGoToStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax

		Public ReadOnly Property ErrorKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax)._errorKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property GoToKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax)._goToKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property Label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax)(Me._label, 4)
			End Get
		End Property

		Public ReadOnly Property Minus As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As PunctuationSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax)._minus
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(3), MyBase.GetChildIndex(3)))
				Return syntaxToken
			End Get
		End Property

		Public ReadOnly Property OnKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax)._onKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal goToKeyword As KeywordSyntax, ByVal minus As PunctuationSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(kind, errors, annotations, onKeyword, errorKeyword, goToKeyword, minus, DirectCast(label.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitOnErrorGoToStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitOnErrorGoToStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 4) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._label
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim label As SyntaxNode
			If (i <> 4) Then
				label = Nothing
			Else
				label = Me.Label
			End If
			Return label
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal onKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal errorKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal goToKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal minus As Microsoft.CodeAnalysis.SyntaxToken, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax
			Dim onErrorGoToStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax
			If (kind <> MyBase.Kind() OrElse onKeyword <> Me.OnKeyword OrElse errorKeyword <> Me.ErrorKeyword OrElse goToKeyword <> Me.GoToKeyword OrElse minus <> Me.Minus OrElse label <> Me.Label) Then
				Dim statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.OnErrorGoToStatement(kind, onKeyword, errorKeyword, goToKeyword, minus, label)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				onErrorGoToStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, statement, statement.WithAnnotations(annotations))
			Else
				onErrorGoToStatementSyntax = Me
			End If
			Return onErrorGoToStatementSyntax
		End Function

		Public Function WithErrorKeyword(ByVal errorKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.OnKeyword, errorKeyword, Me.GoToKeyword, Me.Minus, Me.Label)
		End Function

		Public Function WithGoToKeyword(ByVal goToKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.OnKeyword, Me.ErrorKeyword, goToKeyword, Me.Minus, Me.Label)
		End Function

		Public Function WithLabel(ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.OnKeyword, Me.ErrorKeyword, Me.GoToKeyword, Me.Minus, label)
		End Function

		Public Function WithMinus(ByVal minus As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.OnKeyword, Me.ErrorKeyword, Me.GoToKeyword, minus, Me.Label)
		End Function

		Public Function WithOnKeyword(ByVal onKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax
			Return Me.Update(MyBase.Kind(), onKeyword, Me.ErrorKeyword, Me.GoToKeyword, Me.Minus, Me.Label)
		End Function
	End Class
End Namespace