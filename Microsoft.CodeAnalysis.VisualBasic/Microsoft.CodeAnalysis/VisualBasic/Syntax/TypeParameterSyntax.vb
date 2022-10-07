Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class TypeParameterSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _typeParameterConstraintClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterConstraintClauseSyntax

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax)._identifier, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property TypeParameterConstraintClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterConstraintClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterConstraintClauseSyntax)(Me._typeParameterConstraintClause, 2)
			End Get
		End Property

		Public ReadOnly Property VarianceKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax)._varianceKeyword
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, MyBase.Position, 0))
				Return syntaxToken
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal varianceKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax, ByVal typeParameterConstraintClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterConstraintClauseSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax(kind, errors, annotations, varianceKeyword, identifier, If(typeParameterConstraintClause IsNot Nothing, DirectCast(typeParameterConstraintClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitTypeParameter(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitTypeParameter(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 2) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._typeParameterConstraintClause
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim typeParameterConstraintClause As SyntaxNode
			If (i <> 2) Then
				typeParameterConstraintClause = Nothing
			Else
				typeParameterConstraintClause = Me.TypeParameterConstraintClause
			End If
			Return typeParameterConstraintClause
		End Function

		Public Function Update(ByVal varianceKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal typeParameterConstraintClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterConstraintClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax
			Dim typeParameterSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax
			If (varianceKeyword <> Me.VarianceKeyword OrElse identifier <> Me.Identifier OrElse typeParameterConstraintClause <> Me.TypeParameterConstraintClause) Then
				Dim typeParameterSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TypeParameter(varianceKeyword, identifier, typeParameterConstraintClause)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				typeParameterSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, typeParameterSyntax1, typeParameterSyntax1.WithAnnotations(annotations))
			Else
				typeParameterSyntax = Me
			End If
			Return typeParameterSyntax
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax
			Return Me.Update(Me.VarianceKeyword, identifier, Me.TypeParameterConstraintClause)
		End Function

		Public Function WithTypeParameterConstraintClause(ByVal typeParameterConstraintClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterConstraintClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax
			Return Me.Update(Me.VarianceKeyword, Me.Identifier, typeParameterConstraintClause)
		End Function

		Public Function WithVarianceKeyword(ByVal varianceKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax
			Return Me.Update(varianceKeyword, Me.Identifier, Me.TypeParameterConstraintClause)
		End Function
	End Class
End Namespace