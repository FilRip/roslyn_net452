Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CollectionRangeVariableSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax

		Friend _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax

		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)(Me._asClause, 1)
			End Get
		End Property

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 3)
			End Get
		End Property

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)(Me._identifier)
			End Get
		End Property

		Public ReadOnly Property InKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)._inKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax, ByVal inKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax(kind, errors, annotations, DirectCast(identifier.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax), If(asClause IsNot Nothing, DirectCast(asClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax), Nothing), inKeyword, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCollectionRangeVariable(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCollectionRangeVariable(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._identifier
					Exit Select
				Case 1
					syntaxNode = Me._asClause
					Exit Select
				Case 2
				Label0:
					syntaxNode = Nothing
					Exit Select
				Case 3
					syntaxNode = Me._expression
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim identifier As SyntaxNode
			Select Case i
				Case 0
					identifier = Me.Identifier
					Exit Select
				Case 1
					identifier = Me.AsClause
					Exit Select
				Case 2
				Label0:
					identifier = Nothing
					Exit Select
				Case 3
					identifier = Me.Expression
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return identifier
		End Function

		Public Function Update(ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax, ByVal inKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax
			Dim collectionRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax
			If (identifier <> Me.Identifier OrElse asClause <> Me.AsClause OrElse inKeyword <> Me.InKeyword OrElse expression <> Me.Expression) Then
				Dim collectionRangeVariableSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CollectionRangeVariable(identifier, asClause, inKeyword, expression)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				collectionRangeVariableSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, collectionRangeVariableSyntax1, collectionRangeVariableSyntax1.WithAnnotations(annotations))
			Else
				collectionRangeVariableSyntax = Me
			End If
			Return collectionRangeVariableSyntax
		End Function

		Public Function WithAsClause(ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax
			Return Me.Update(Me.Identifier, asClause, Me.InKeyword, Me.Expression)
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax
			Return Me.Update(Me.Identifier, Me.AsClause, Me.InKeyword, expression)
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax
			Return Me.Update(identifier, Me.AsClause, Me.InKeyword, Me.Expression)
		End Function

		Public Function WithInKeyword(ByVal inKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax
			Return Me.Update(Me.Identifier, Me.AsClause, inKeyword, Me.Expression)
		End Function
	End Class
End Namespace