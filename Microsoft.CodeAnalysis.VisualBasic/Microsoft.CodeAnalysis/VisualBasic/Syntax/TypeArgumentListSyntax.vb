Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class TypeArgumentListSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _arguments As SyntaxNode

		Public ReadOnly Property Arguments As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			Get
				Dim typeSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._arguments, 2)
				typeSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(red, MyBase.GetChildIndex(2)))
				Return typeSyntaxes
			End Get
		End Property

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)._closeParenToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property OfKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)._ofKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)._openParenToken, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal ofKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal arguments As SyntaxNode, ByVal closeParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax(kind, errors, annotations, openParenToken, ofKeyword, If(arguments IsNot Nothing, arguments.Green, Nothing), closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitTypeArgumentList(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitTypeArgumentList(Me)
		End Sub

		Public Function AddArguments(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax
			Return Me.WithArguments(Me.Arguments.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 2) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._arguments
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 2) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._arguments, 2)
			End If
			Return red
		End Function

		Public Function Update(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal ofKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal arguments As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax), ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax
			Dim typeArgumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax
			If (openParenToken <> Me.OpenParenToken OrElse ofKeyword <> Me.OfKeyword OrElse arguments <> Me.Arguments OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim typeArgumentListSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TypeArgumentList(openParenToken, ofKeyword, arguments, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				typeArgumentListSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, typeArgumentListSyntax1, typeArgumentListSyntax1.WithAnnotations(annotations))
			Else
				typeArgumentListSyntax = Me
			End If
			Return typeArgumentListSyntax
		End Function

		Public Function WithArguments(ByVal arguments As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax
			Return Me.Update(Me.OpenParenToken, Me.OfKeyword, arguments, Me.CloseParenToken)
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax
			Return Me.Update(Me.OpenParenToken, Me.OfKeyword, Me.Arguments, closeParenToken)
		End Function

		Public Function WithOfKeyword(ByVal ofKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax
			Return Me.Update(Me.OpenParenToken, ofKeyword, Me.Arguments, Me.CloseParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax
			Return Me.Update(openParenToken, Me.OfKeyword, Me.Arguments, Me.CloseParenToken)
		End Function
	End Class
End Namespace