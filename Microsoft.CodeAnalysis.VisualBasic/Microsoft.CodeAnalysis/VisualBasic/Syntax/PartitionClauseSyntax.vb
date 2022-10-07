Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class PartitionClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax
		Friend _count As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Count As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._count, 1)
			End Get
		End Property

		Public ReadOnly Property SkipOrTakeKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax)._skipOrTakeKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal skipOrTakeKeyword As KeywordSyntax, ByVal count As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax(kind, errors, annotations, skipOrTakeKeyword, DirectCast(count.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitPartitionClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitPartitionClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._count
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim count As SyntaxNode
			If (i <> 1) Then
				count = Nothing
			Else
				count = Me.Count
			End If
			Return count
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal skipOrTakeKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal count As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionClauseSyntax
			Dim partitionClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionClauseSyntax
			If (kind <> MyBase.Kind() OrElse skipOrTakeKeyword <> Me.SkipOrTakeKeyword OrElse count <> Me.Count) Then
				Dim partitionClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.PartitionClause(kind, skipOrTakeKeyword, count)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				partitionClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, partitionClauseSyntax1, partitionClauseSyntax1.WithAnnotations(annotations))
			Else
				partitionClauseSyntax = Me
			End If
			Return partitionClauseSyntax
		End Function

		Public Function WithCount(ByVal count As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionClauseSyntax
			Return Me.Update(MyBase.Kind(), Me.SkipOrTakeKeyword, count)
		End Function

		Public Function WithSkipOrTakeKeyword(ByVal skipOrTakeKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionClauseSyntax
			Return Me.Update(MyBase.Kind(), skipOrTakeKeyword, Me.Count)
		End Function
	End Class
End Namespace