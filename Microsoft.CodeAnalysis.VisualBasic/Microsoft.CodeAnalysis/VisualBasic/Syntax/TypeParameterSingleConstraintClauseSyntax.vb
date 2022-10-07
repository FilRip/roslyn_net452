Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class TypeParameterSingleConstraintClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterConstraintClauseSyntax
		Friend _constraint As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax

		Public ReadOnly Property AsKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax)._asKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Constraint As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax)(Me._constraint, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal asKeyword As KeywordSyntax, ByVal constraint As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax(kind, errors, annotations, asKeyword, DirectCast(constraint.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitTypeParameterSingleConstraintClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitTypeParameterSingleConstraintClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._constraint
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim constraint As SyntaxNode
			If (i <> 1) Then
				constraint = Nothing
			Else
				constraint = Me.Constraint
			End If
			Return constraint
		End Function

		Public Function Update(ByVal asKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal constraint As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSingleConstraintClauseSyntax
			Dim typeParameterSingleConstraintClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSingleConstraintClauseSyntax
			If (asKeyword <> Me.AsKeyword OrElse constraint <> Me.Constraint) Then
				Dim typeParameterSingleConstraintClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSingleConstraintClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TypeParameterSingleConstraintClause(asKeyword, constraint)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				typeParameterSingleConstraintClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, typeParameterSingleConstraintClauseSyntax1, typeParameterSingleConstraintClauseSyntax1.WithAnnotations(annotations))
			Else
				typeParameterSingleConstraintClauseSyntax = Me
			End If
			Return typeParameterSingleConstraintClauseSyntax
		End Function

		Public Function WithAsKeyword(ByVal asKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSingleConstraintClauseSyntax
			Return Me.Update(asKeyword, Me.Constraint)
		End Function

		Public Function WithConstraint(ByVal constraint As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSingleConstraintClauseSyntax
			Return Me.Update(Me.AsKeyword, constraint)
		End Function
	End Class
End Namespace