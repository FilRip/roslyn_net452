Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class PartitionWhileClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax
		Friend _condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._condition, 2)
			End Get
		End Property

		Public ReadOnly Property SkipOrTakeKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax)._skipOrTakeKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property WhileKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax)._whileKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal skipOrTakeKeyword As KeywordSyntax, ByVal whileKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax(kind, errors, annotations, skipOrTakeKeyword, whileKeyword, DirectCast(condition.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitPartitionWhileClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitPartitionWhileClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 2) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._condition
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim condition As SyntaxNode
			If (i <> 2) Then
				condition = Nothing
			Else
				condition = Me.Condition
			End If
			Return condition
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal skipOrTakeKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal whileKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax
			Dim partitionWhileClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax
			If (kind <> MyBase.Kind() OrElse skipOrTakeKeyword <> Me.SkipOrTakeKeyword OrElse whileKeyword <> Me.WhileKeyword OrElse condition <> Me.Condition) Then
				Dim partitionWhileClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.PartitionWhileClause(kind, skipOrTakeKeyword, whileKeyword, condition)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				partitionWhileClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, partitionWhileClauseSyntax1, partitionWhileClauseSyntax1.WithAnnotations(annotations))
			Else
				partitionWhileClauseSyntax = Me
			End If
			Return partitionWhileClauseSyntax
		End Function

		Public Function WithCondition(ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax
			Return Me.Update(MyBase.Kind(), Me.SkipOrTakeKeyword, Me.WhileKeyword, condition)
		End Function

		Public Function WithSkipOrTakeKeyword(ByVal skipOrTakeKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax
			Return Me.Update(MyBase.Kind(), skipOrTakeKeyword, Me.WhileKeyword, Me.Condition)
		End Function

		Public Function WithWhileKeyword(ByVal whileKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax
			Return Me.Update(MyBase.Kind(), Me.SkipOrTakeKeyword, whileKeyword, Me.Condition)
		End Function
	End Class
End Namespace