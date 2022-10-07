Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class TupleTypeSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
		Friend _elements As SyntaxNode

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax)._closeParenToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property Elements As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleElementSyntax)
			Get
				Dim tupleElementSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleElementSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._elements, 1)
				tupleElementSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleElementSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleElementSyntax)(red, MyBase.GetChildIndex(1)))
				Return tupleElementSyntaxes
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax)._openParenToken, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal elements As SyntaxNode, ByVal closeParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax(kind, errors, annotations, openParenToken, If(elements IsNot Nothing, elements.Green, Nothing), closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitTupleType(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitTupleType(Me)
		End Sub

		Public Function AddElements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleElementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleTypeSyntax
			Return Me.WithElements(Me.Elements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._elements
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._elements, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal elements As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleElementSyntax), ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleTypeSyntax
			Dim tupleTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleTypeSyntax
			If (openParenToken <> Me.OpenParenToken OrElse elements <> Me.Elements OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim tupleTypeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleTypeSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TupleType(openParenToken, elements, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				tupleTypeSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, tupleTypeSyntax1, tupleTypeSyntax1.WithAnnotations(annotations))
			Else
				tupleTypeSyntax = Me
			End If
			Return tupleTypeSyntax
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleTypeSyntax
			Return Me.Update(Me.OpenParenToken, Me.Elements, closeParenToken)
		End Function

		Public Function WithElements(ByVal elements As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleElementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleTypeSyntax
			Return Me.Update(Me.OpenParenToken, elements, Me.CloseParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleTypeSyntax
			Return Me.Update(openParenToken, Me.Elements, Me.CloseParenToken)
		End Function
	End Class
End Namespace