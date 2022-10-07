Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class InterpolatedStringTextSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringContentSyntax
		Public ReadOnly Property TextToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax)._textToken, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal textToken As InterpolatedStringTextTokenSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax(kind, errors, annotations, textToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitInterpolatedStringText(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitInterpolatedStringText(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal textToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringTextSyntax
			Dim interpolatedStringTextSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringTextSyntax
			If (textToken = Me.TextToken) Then
				interpolatedStringTextSyntax = Me
			Else
				Dim interpolatedStringTextSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringTextSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.InterpolatedStringText(textToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				interpolatedStringTextSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, interpolatedStringTextSyntax1, interpolatedStringTextSyntax1.WithAnnotations(annotations))
			End If
			Return interpolatedStringTextSyntax
		End Function

		Public Function WithTextToken(ByVal textToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringTextSyntax
			Return Me.Update(textToken)
		End Function
	End Class
End Namespace