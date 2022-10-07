Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class TypeConstraintSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax
		Friend _type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax

		Public ReadOnly Property Type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(Me._type)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax(kind, errors, annotations, DirectCast(type.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitTypeConstraint(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitTypeConstraint(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 0) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._type
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim type As SyntaxNode
			If (i <> 0) Then
				type = Nothing
			Else
				type = Me.Type
			End If
			Return type
		End Function

		Public Function Update(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeConstraintSyntax
			Dim typeConstraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeConstraintSyntax
			If (type = Me.Type) Then
				typeConstraintSyntax = Me
			Else
				Dim typeConstraintSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeConstraintSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TypeConstraint(type)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				typeConstraintSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, typeConstraintSyntax1, typeConstraintSyntax1.WithAnnotations(annotations))
			End If
			Return typeConstraintSyntax
		End Function

		Public Function WithType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeConstraintSyntax
			Return Me.Update(type)
		End Function
	End Class
End Namespace