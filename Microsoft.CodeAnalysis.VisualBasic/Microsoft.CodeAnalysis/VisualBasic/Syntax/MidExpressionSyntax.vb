Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class MidExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax

		Public ReadOnly Property ArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)(Me._argumentList, 1)
			End Get
		End Property

		Public ReadOnly Property Mid As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax)._mid, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal mid As IdentifierTokenSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax(kind, errors, annotations, mid, DirectCast(argumentList.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitMidExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitMidExpression(Me)
		End Sub

		Public Function AddArgumentListArguments(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax
			Return Me.WithArgumentList(If(Me.ArgumentList IsNot Nothing, Me.ArgumentList, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ArgumentList()).AddArguments(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._argumentList
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim argumentList As SyntaxNode
			If (i <> 1) Then
				argumentList = Nothing
			Else
				argumentList = Me.ArgumentList
			End If
			Return argumentList
		End Function

		Public Function Update(ByVal mid As Microsoft.CodeAnalysis.SyntaxToken, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax
			Dim midExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax
			If (mid <> Me.Mid OrElse argumentList <> Me.ArgumentList) Then
				Dim midExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.MidExpression(mid, argumentList)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				midExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, midExpressionSyntax1, midExpressionSyntax1.WithAnnotations(annotations))
			Else
				midExpressionSyntax = Me
			End If
			Return midExpressionSyntax
		End Function

		Public Function WithArgumentList(ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax
			Return Me.Update(Me.Mid, argumentList)
		End Function

		Public Function WithMid(ByVal mid As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax
			Return Me.Update(mid, Me.ArgumentList)
		End Function
	End Class
End Namespace