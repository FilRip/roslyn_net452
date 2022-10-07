Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class GetXmlNamespaceExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax)._closeParenToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property GetXmlNamespaceKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax)._getXmlNamespaceKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax)(Me._name, 2)
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax)._openParenToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal getXmlNamespaceKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal openParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax, ByVal closeParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax(kind, errors, annotations, getXmlNamespaceKeyword, openParenToken, If(name IsNot Nothing, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax), Nothing), closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitGetXmlNamespaceExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitGetXmlNamespaceExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 2) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._name
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim name As SyntaxNode
			If (i <> 2) Then
				name = Nothing
			Else
				name = Me.Name
			End If
			Return name
		End Function

		Public Function Update(ByVal getXmlNamespaceKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax, ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GetXmlNamespaceExpressionSyntax
			Dim getXmlNamespaceExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GetXmlNamespaceExpressionSyntax
			If (getXmlNamespaceKeyword <> Me.GetXmlNamespaceKeyword OrElse openParenToken <> Me.OpenParenToken OrElse name <> Me.Name OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim xmlNamespaceExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.GetXmlNamespaceExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.GetXmlNamespaceExpression(getXmlNamespaceKeyword, openParenToken, name, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				getXmlNamespaceExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlNamespaceExpression, xmlNamespaceExpression.WithAnnotations(annotations))
			Else
				getXmlNamespaceExpressionSyntax = Me
			End If
			Return getXmlNamespaceExpressionSyntax
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GetXmlNamespaceExpressionSyntax
			Return Me.Update(Me.GetXmlNamespaceKeyword, Me.OpenParenToken, Me.Name, closeParenToken)
		End Function

		Public Function WithGetXmlNamespaceKeyword(ByVal getXmlNamespaceKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GetXmlNamespaceExpressionSyntax
			Return Me.Update(getXmlNamespaceKeyword, Me.OpenParenToken, Me.Name, Me.CloseParenToken)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GetXmlNamespaceExpressionSyntax
			Return Me.Update(Me.GetXmlNamespaceKeyword, Me.OpenParenToken, name, Me.CloseParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GetXmlNamespaceExpressionSyntax
			Return Me.Update(Me.GetXmlNamespaceKeyword, openParenToken, Me.Name, Me.CloseParenToken)
		End Function
	End Class
End Namespace