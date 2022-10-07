Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class OptionStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Public ReadOnly Property NameKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)._nameKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property OptionKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)._optionKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property ValueKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)._valueKeyword
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(2), MyBase.GetChildIndex(2)))
				Return syntaxToken
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal optionKeyword As KeywordSyntax, ByVal nameKeyword As KeywordSyntax, ByVal valueKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax(kind, errors, annotations, optionKeyword, nameKeyword, valueKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitOptionStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitOptionStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal optionKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal nameKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal valueKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax
			Dim optionStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax
			If (optionKeyword <> Me.OptionKeyword OrElse nameKeyword <> Me.NameKeyword OrElse valueKeyword <> Me.ValueKeyword) Then
				Dim optionStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.OptionStatement(optionKeyword, nameKeyword, valueKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				optionStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, optionStatementSyntax1, optionStatementSyntax1.WithAnnotations(annotations))
			Else
				optionStatementSyntax = Me
			End If
			Return optionStatementSyntax
		End Function

		Public Function WithNameKeyword(ByVal nameKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax
			Return Me.Update(Me.OptionKeyword, nameKeyword, Me.ValueKeyword)
		End Function

		Public Function WithOptionKeyword(ByVal optionKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax
			Return Me.Update(optionKeyword, Me.NameKeyword, Me.ValueKeyword)
		End Function

		Public Function WithValueKeyword(ByVal valueKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax
			Return Me.Update(Me.OptionKeyword, Me.NameKeyword, valueKeyword)
		End Function
	End Class
End Namespace