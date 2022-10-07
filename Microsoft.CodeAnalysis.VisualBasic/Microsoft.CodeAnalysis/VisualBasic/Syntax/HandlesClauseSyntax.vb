Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class HandlesClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _events As SyntaxNode

		Public ReadOnly Property Events As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax)
			Get
				Dim handlesClauseItemSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._events, 1)
				handlesClauseItemSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax)(red, MyBase.GetChildIndex(1)))
				Return handlesClauseItemSyntaxes
			End Get
		End Property

		Public ReadOnly Property HandlesKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax)._handlesKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal handlesKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal events As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax(kind, errors, annotations, handlesKeyword, If(events IsNot Nothing, events.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitHandlesClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitHandlesClause(Me)
		End Sub

		Public Function AddEvents(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax
			Return Me.[WithEvents](Me.Events.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._events
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._events, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal handlesKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal events As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax
			Dim handlesClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax
			If (handlesKeyword <> Me.HandlesKeyword OrElse events <> Me.Events) Then
				Dim handlesClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.HandlesClause(handlesKeyword, events)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				handlesClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, handlesClauseSyntax1, handlesClauseSyntax1.WithAnnotations(annotations))
			Else
				handlesClauseSyntax = Me
			End If
			Return handlesClauseSyntax
		End Function

		Public Function [WithEvents](ByVal events As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax
			Return Me.Update(Me.HandlesKeyword, events)
		End Function

		Public Function WithHandlesKeyword(ByVal handlesKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseSyntax
			Return Me.Update(handlesKeyword, Me.Events)
		End Function
	End Class
End Namespace