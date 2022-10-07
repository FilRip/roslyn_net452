Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class TypeParameterListSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _parameters As SyntaxNode

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)._closeParenToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property OfKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)._ofKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)._openParenToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Parameters As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax)
			Get
				Dim typeParameterSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._parameters, 2)
				typeParameterSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax)(red, MyBase.GetChildIndex(2)))
				Return typeParameterSyntaxes
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal ofKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal parameters As SyntaxNode, ByVal closeParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax(kind, errors, annotations, openParenToken, ofKeyword, If(parameters IsNot Nothing, parameters.Green, Nothing), closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitTypeParameterList(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitTypeParameterList(Me)
		End Sub

		Public Function AddParameters(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax
			Return Me.WithParameters(Me.Parameters.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 2) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._parameters
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 2) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._parameters, 2)
			End If
			Return red
		End Function

		Public Function Update(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal ofKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal parameters As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax), ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax
			If (openParenToken <> Me.OpenParenToken OrElse ofKeyword <> Me.OfKeyword OrElse parameters <> Me.Parameters OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim typeParameterListSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TypeParameterList(openParenToken, ofKeyword, parameters, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				typeParameterListSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, typeParameterListSyntax1, typeParameterListSyntax1.WithAnnotations(annotations))
			Else
				typeParameterListSyntax = Me
			End If
			Return typeParameterListSyntax
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax
			Return Me.Update(Me.OpenParenToken, Me.OfKeyword, Me.Parameters, closeParenToken)
		End Function

		Public Function WithOfKeyword(ByVal ofKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax
			Return Me.Update(Me.OpenParenToken, ofKeyword, Me.Parameters, Me.CloseParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax
			Return Me.Update(openParenToken, Me.OfKeyword, Me.Parameters, Me.CloseParenToken)
		End Function

		Public Function WithParameters(ByVal parameters As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax
			Return Me.Update(Me.OpenParenToken, Me.OfKeyword, parameters, Me.CloseParenToken)
		End Function
	End Class
End Namespace