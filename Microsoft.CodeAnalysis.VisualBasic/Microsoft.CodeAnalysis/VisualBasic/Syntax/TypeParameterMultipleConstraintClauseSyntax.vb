Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class TypeParameterMultipleConstraintClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterConstraintClauseSyntax
		Friend _constraints As SyntaxNode

		Public ReadOnly Property AsKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax)._asKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property CloseBraceToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax)._closeBraceToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property Constraints As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax)
			Get
				Dim constraintSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._constraints, 2)
				constraintSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax)(red, MyBase.GetChildIndex(2)))
				Return constraintSyntaxes
			End Get
		End Property

		Public ReadOnly Property OpenBraceToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax)._openBraceToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal asKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal openBraceToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal constraints As SyntaxNode, ByVal closeBraceToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax(kind, errors, annotations, asKeyword, openBraceToken, If(constraints IsNot Nothing, constraints.Green, Nothing), closeBraceToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitTypeParameterMultipleConstraintClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitTypeParameterMultipleConstraintClause(Me)
		End Sub

		Public Function AddConstraints(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax
			Return Me.WithConstraints(Me.Constraints.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 2) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._constraints
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 2) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._constraints, 2)
			End If
			Return red
		End Function

		Public Function Update(ByVal asKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal openBraceToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal constraints As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax), ByVal closeBraceToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax
			Dim typeParameterMultipleConstraintClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax
			If (asKeyword <> Me.AsKeyword OrElse openBraceToken <> Me.OpenBraceToken OrElse constraints <> Me.Constraints OrElse closeBraceToken <> Me.CloseBraceToken) Then
				Dim typeParameterMultipleConstraintClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TypeParameterMultipleConstraintClause(asKeyword, openBraceToken, constraints, closeBraceToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				typeParameterMultipleConstraintClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, typeParameterMultipleConstraintClauseSyntax1, typeParameterMultipleConstraintClauseSyntax1.WithAnnotations(annotations))
			Else
				typeParameterMultipleConstraintClauseSyntax = Me
			End If
			Return typeParameterMultipleConstraintClauseSyntax
		End Function

		Public Function WithAsKeyword(ByVal asKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax
			Return Me.Update(asKeyword, Me.OpenBraceToken, Me.Constraints, Me.CloseBraceToken)
		End Function

		Public Function WithCloseBraceToken(ByVal closeBraceToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax
			Return Me.Update(Me.AsKeyword, Me.OpenBraceToken, Me.Constraints, closeBraceToken)
		End Function

		Public Function WithConstraints(ByVal constraints As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstraintSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax
			Return Me.Update(Me.AsKeyword, Me.OpenBraceToken, constraints, Me.CloseBraceToken)
		End Function

		Public Function WithOpenBraceToken(ByVal openBraceToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax
			Return Me.Update(Me.AsKeyword, openBraceToken, Me.Constraints, Me.CloseBraceToken)
		End Function
	End Class
End Namespace