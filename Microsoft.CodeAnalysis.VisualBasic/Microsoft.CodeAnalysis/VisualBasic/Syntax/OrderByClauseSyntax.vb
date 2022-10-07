Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class OrderByClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax
		Friend _orderings As SyntaxNode

		Public ReadOnly Property ByKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax)._byKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Orderings As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax)
			Get
				Dim orderingSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._orderings, 2)
				orderingSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax)(red, MyBase.GetChildIndex(2)))
				Return orderingSyntaxes
			End Get
		End Property

		Public ReadOnly Property OrderKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax)._orderKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal orderKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal byKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal orderings As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax(kind, errors, annotations, orderKeyword, byKeyword, If(orderings IsNot Nothing, orderings.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitOrderByClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitOrderByClause(Me)
		End Sub

		Public Function AddOrderings(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax
			Return Me.WithOrderings(Me.Orderings.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 2) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._orderings
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 2) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._orderings, 2)
			End If
			Return red
		End Function

		Public Function Update(ByVal orderKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal byKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal orderings As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax
			Dim orderByClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax
			If (orderKeyword <> Me.OrderKeyword OrElse byKeyword <> Me.ByKeyword OrElse orderings <> Me.Orderings) Then
				Dim orderByClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.OrderByClause(orderKeyword, byKeyword, orderings)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				orderByClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, orderByClauseSyntax1, orderByClauseSyntax1.WithAnnotations(annotations))
			Else
				orderByClauseSyntax = Me
			End If
			Return orderByClauseSyntax
		End Function

		Public Function WithByKeyword(ByVal byKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax
			Return Me.Update(Me.OrderKeyword, byKeyword, Me.Orderings)
		End Function

		Public Function WithOrderings(ByVal orderings As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax
			Return Me.Update(Me.OrderKeyword, Me.ByKeyword, orderings)
		End Function

		Public Function WithOrderKeyword(ByVal orderKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax
			Return Me.Update(orderKeyword, Me.ByKeyword, Me.Orderings)
		End Function
	End Class
End Namespace