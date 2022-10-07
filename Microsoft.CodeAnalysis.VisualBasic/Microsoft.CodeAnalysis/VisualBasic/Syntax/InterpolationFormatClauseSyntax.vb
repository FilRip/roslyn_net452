Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class InterpolationFormatClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Public ReadOnly Property ColonToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax)._colonToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property FormatStringToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax)._formatStringToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal colonToken As PunctuationSyntax, ByVal formatStringToken As InterpolatedStringTextTokenSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax(kind, errors, annotations, colonToken, formatStringToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitInterpolationFormatClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitInterpolationFormatClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal colonToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal formatStringToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax
			Dim interpolationFormatClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax
			If (colonToken <> Me.ColonToken OrElse formatStringToken <> Me.FormatStringToken) Then
				Dim interpolationFormatClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.InterpolationFormatClause(colonToken, formatStringToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				interpolationFormatClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, interpolationFormatClauseSyntax1, interpolationFormatClauseSyntax1.WithAnnotations(annotations))
			Else
				interpolationFormatClauseSyntax = Me
			End If
			Return interpolationFormatClauseSyntax
		End Function

		Public Function WithColonToken(ByVal colonToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax
			Return Me.Update(colonToken, Me.FormatStringToken)
		End Function

		Public Function WithFormatStringToken(ByVal formatStringToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolationFormatClauseSyntax
			Return Me.Update(Me.ColonToken, formatStringToken)
		End Function
	End Class
End Namespace