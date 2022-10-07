Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class GroupAggregationSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationSyntax
		Public ReadOnly Property GroupKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax)._groupKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal groupKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax(kind, errors, annotations, groupKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitGroupAggregation(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitGroupAggregation(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal groupKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupAggregationSyntax
			Dim groupAggregationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupAggregationSyntax
			If (groupKeyword = Me.GroupKeyword) Then
				groupAggregationSyntax = Me
			Else
				Dim groupAggregationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupAggregationSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.GroupAggregation(groupKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				groupAggregationSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, groupAggregationSyntax1, groupAggregationSyntax1.WithAnnotations(annotations))
			End If
			Return groupAggregationSyntax
		End Function

		Public Function WithGroupKeyword(ByVal groupKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupAggregationSyntax
			Return Me.Update(groupKeyword)
		End Function
	End Class
End Namespace