Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlProcessingInstructionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Public ReadOnly Property LessThanQuestionToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax)._lessThanQuestionToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax)._name, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property QuestionGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax)._questionGreaterThanToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property TextTokens As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax)._textTokens
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, Me.GetChildPosition(2), MyBase.GetChildIndex(2)))
				Return syntaxTokenLists
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanQuestionToken As PunctuationSyntax, ByVal name As XmlNameTokenSyntax, ByVal textTokens As GreenNode, ByVal questionGreaterThanToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax(kind, errors, annotations, lessThanQuestionToken, name, textTokens, questionGreaterThanToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlProcessingInstruction(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlProcessingInstruction(Me)
		End Sub

		Public Function AddTextTokens(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax
			Return Me.WithTextTokens(Me.TextTokens.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal lessThanQuestionToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal name As Microsoft.CodeAnalysis.SyntaxToken, ByVal textTokens As SyntaxTokenList, ByVal questionGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax
			Dim xmlProcessingInstructionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax
			If (lessThanQuestionToken <> Me.LessThanQuestionToken OrElse name <> Me.Name OrElse textTokens <> Me.TextTokens OrElse questionGreaterThanToken <> Me.QuestionGreaterThanToken) Then
				Dim xmlProcessingInstructionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlProcessingInstruction(lessThanQuestionToken, name, textTokens, questionGreaterThanToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlProcessingInstructionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlProcessingInstructionSyntax1, xmlProcessingInstructionSyntax1.WithAnnotations(annotations))
			Else
				xmlProcessingInstructionSyntax = Me
			End If
			Return xmlProcessingInstructionSyntax
		End Function

		Public Function WithLessThanQuestionToken(ByVal lessThanQuestionToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax
			Return Me.Update(lessThanQuestionToken, Me.Name, Me.TextTokens, Me.QuestionGreaterThanToken)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax
			Return Me.Update(Me.LessThanQuestionToken, name, Me.TextTokens, Me.QuestionGreaterThanToken)
		End Function

		Public Function WithQuestionGreaterThanToken(ByVal questionGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax
			Return Me.Update(Me.LessThanQuestionToken, Me.Name, Me.TextTokens, questionGreaterThanToken)
		End Function

		Public Function WithTextTokens(ByVal textTokens As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax
			Return Me.Update(Me.LessThanQuestionToken, Me.Name, textTokens, Me.QuestionGreaterThanToken)
		End Function
	End Class
End Namespace