Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SpecialConstraintSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax
		Public ReadOnly Property ConstraintKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax)._constraintKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal constraintKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax(kind, errors, annotations, constraintKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSpecialConstraint(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSpecialConstraint(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal constraintKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SpecialConstraintSyntax
			Dim specialConstraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SpecialConstraintSyntax
			If (kind <> MyBase.Kind() OrElse constraintKeyword <> Me.ConstraintKeyword) Then
				Dim specialConstraintSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SpecialConstraintSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SpecialConstraint(kind, constraintKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				specialConstraintSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, specialConstraintSyntax1, specialConstraintSyntax1.WithAnnotations(annotations))
			Else
				specialConstraintSyntax = Me
			End If
			Return specialConstraintSyntax
		End Function

		Public Function WithConstraintKeyword(ByVal constraintKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SpecialConstraintSyntax
			Return Me.Update(MyBase.Kind(), constraintKeyword)
		End Function
	End Class
End Namespace