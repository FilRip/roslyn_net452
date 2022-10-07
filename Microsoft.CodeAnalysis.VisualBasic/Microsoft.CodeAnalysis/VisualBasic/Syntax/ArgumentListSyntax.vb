Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ArgumentListSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _arguments As SyntaxNode

		Public ReadOnly Property Arguments As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax)
			Get
				Dim argumentSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._arguments, 1)
				argumentSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax)(red, MyBase.GetChildIndex(1)))
				Return argumentSyntaxes
			End Get
		End Property

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)._closeParenToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)._openParenToken, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal arguments As SyntaxNode, ByVal closeParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax(kind, errors, annotations, openParenToken, If(arguments IsNot Nothing, arguments.Green, Nothing), closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitArgumentList(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitArgumentList(Me)
		End Sub

		Public Function AddArguments(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			Return Me.WithArguments(Me.Arguments.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._arguments
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._arguments, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal arguments As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax), ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			If (openParenToken <> Me.OpenParenToken OrElse arguments <> Me.Arguments OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim argumentListSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ArgumentList(openParenToken, arguments, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				argumentListSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, argumentListSyntax1, argumentListSyntax1.WithAnnotations(annotations))
			Else
				argumentListSyntax = Me
			End If
			Return argumentListSyntax
		End Function

		Public Function WithArguments(ByVal arguments As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			Return Me.Update(Me.OpenParenToken, arguments, Me.CloseParenToken)
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			Return Me.Update(Me.OpenParenToken, Me.Arguments, closeParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			Return Me.Update(openParenToken, Me.Arguments, Me.CloseParenToken)
		End Function
	End Class
End Namespace