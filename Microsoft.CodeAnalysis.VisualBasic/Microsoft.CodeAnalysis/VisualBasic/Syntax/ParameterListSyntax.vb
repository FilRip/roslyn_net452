Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ParameterListSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _parameters As SyntaxNode

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)._closeParenToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)._openParenToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Parameters As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax)
			Get
				Dim parameterSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._parameters, 1)
				parameterSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax)(red, MyBase.GetChildIndex(1)))
				Return parameterSyntaxes
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal parameters As SyntaxNode, ByVal closeParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax(kind, errors, annotations, openParenToken, If(parameters IsNot Nothing, parameters.Green, Nothing), closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitParameterList(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitParameterList(Me)
		End Sub

		Public Function AddParameters(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Return Me.WithParameters(Me.Parameters.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._parameters
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._parameters, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal parameters As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax), ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			If (openParenToken <> Me.OpenParenToken OrElse parameters <> Me.Parameters OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim parameterListSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ParameterList(openParenToken, parameters, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				parameterListSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, parameterListSyntax1, parameterListSyntax1.WithAnnotations(annotations))
			Else
				parameterListSyntax = Me
			End If
			Return parameterListSyntax
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Return Me.Update(Me.OpenParenToken, Me.Parameters, closeParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Return Me.Update(openParenToken, Me.Parameters, Me.CloseParenToken)
		End Function

		Public Function WithParameters(ByVal parameters As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax
			Return Me.Update(Me.OpenParenToken, parameters, Me.CloseParenToken)
		End Function
	End Class
End Namespace