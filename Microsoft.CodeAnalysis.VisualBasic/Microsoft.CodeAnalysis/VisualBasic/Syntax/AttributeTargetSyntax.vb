Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class AttributeTargetSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Public ReadOnly Property AttributeModifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax)._attributeModifier, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property ColonToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax)._colonToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeModifier As KeywordSyntax, ByVal colonToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax(kind, errors, annotations, attributeModifier, colonToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitAttributeTarget(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitAttributeTarget(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal attributeModifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal colonToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax
			Dim attributeTargetSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax
			If (attributeModifier <> Me.AttributeModifier OrElse colonToken <> Me.ColonToken) Then
				Dim attributeTargetSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.AttributeTarget(attributeModifier, colonToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				attributeTargetSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, attributeTargetSyntax1, attributeTargetSyntax1.WithAnnotations(annotations))
			Else
				attributeTargetSyntax = Me
			End If
			Return attributeTargetSyntax
		End Function

		Public Function WithAttributeModifier(ByVal attributeModifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax
			Return Me.Update(attributeModifier, Me.ColonToken)
		End Function

		Public Function WithColonToken(ByVal colonToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeTargetSyntax
			Return Me.Update(Me.AttributeModifier, colonToken)
		End Function
	End Class
End Namespace