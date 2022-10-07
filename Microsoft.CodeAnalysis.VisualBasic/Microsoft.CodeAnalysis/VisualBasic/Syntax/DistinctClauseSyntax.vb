Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class DistinctClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax
		Public ReadOnly Property DistinctKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax)._distinctKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal distinctKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax(kind, errors, annotations, distinctKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitDistinctClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitDistinctClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal distinctKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DistinctClauseSyntax
			Dim distinctClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DistinctClauseSyntax
			If (distinctKeyword = Me.DistinctKeyword) Then
				distinctClauseSyntax = Me
			Else
				Dim distinctClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.DistinctClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.DistinctClause(distinctKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				distinctClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, distinctClauseSyntax1, distinctClauseSyntax1.WithAnnotations(annotations))
			End If
			Return distinctClauseSyntax
		End Function

		Public Function WithDistinctKeyword(ByVal distinctKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DistinctClauseSyntax
			Return Me.Update(distinctKeyword)
		End Function
	End Class
End Namespace