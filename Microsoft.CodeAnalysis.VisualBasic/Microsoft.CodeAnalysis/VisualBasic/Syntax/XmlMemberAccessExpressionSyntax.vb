Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlMemberAccessExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _base As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax

		Public ReadOnly Property Base As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._base)
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(Me._name, 4)
			End Get
		End Property

		Public ReadOnly Property Token1 As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax)._token1, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Token2 As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As PunctuationSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax)._token2
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(2), MyBase.GetChildIndex(2)))
				Return syntaxToken
			End Get
		End Property

		Public ReadOnly Property Token3 As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As PunctuationSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax)._token3
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(3), MyBase.GetChildIndex(3)))
				Return syntaxToken
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal token1 As PunctuationSyntax, ByVal token2 As PunctuationSyntax, ByVal token3 As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(kind, errors, annotations, If(base IsNot Nothing, DirectCast(base.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), Nothing), token1, token2, token3, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlMemberAccessExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlMemberAccessExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._base
			ElseIf (num = 4) Then
				syntaxNode = Me._name
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim base As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				base = Me.Base
			ElseIf (num = 4) Then
				base = Me.Name
			Else
				base = Nothing
			End If
			Return base
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal token1 As Microsoft.CodeAnalysis.SyntaxToken, ByVal token2 As Microsoft.CodeAnalysis.SyntaxToken, ByVal token3 As Microsoft.CodeAnalysis.SyntaxToken, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax
			Dim xmlMemberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax
			If (kind <> MyBase.Kind() OrElse base <> Me.Base OrElse token1 <> Me.Token1 OrElse token2 <> Me.Token2 OrElse token3 <> Me.Token3 OrElse name <> Me.Name) Then
				Dim xmlMemberAccessExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlMemberAccessExpression(kind, base, token1, token2, token3, name)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlMemberAccessExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlMemberAccessExpressionSyntax1, xmlMemberAccessExpressionSyntax1.WithAnnotations(annotations))
			Else
				xmlMemberAccessExpressionSyntax = Me
			End If
			Return xmlMemberAccessExpressionSyntax
		End Function

		Public Function WithBase(ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax
			Return Me.Update(MyBase.Kind(), base, Me.Token1, Me.Token2, Me.Token3, Me.Name)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax
			Return Me.Update(MyBase.Kind(), Me.Base, Me.Token1, Me.Token2, Me.Token3, name)
		End Function

		Public Function WithToken1(ByVal token1 As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax
			Return Me.Update(MyBase.Kind(), Me.Base, token1, Me.Token2, Me.Token3, Me.Name)
		End Function

		Public Function WithToken2(ByVal token2 As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax
			Return Me.Update(MyBase.Kind(), Me.Base, Me.Token1, token2, Me.Token3, Me.Name)
		End Function

		Public Function WithToken3(ByVal token3 As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax
			Return Me.Update(MyBase.Kind(), Me.Base, Me.Token1, Me.Token2, token3, Me.Name)
		End Function
	End Class
End Namespace