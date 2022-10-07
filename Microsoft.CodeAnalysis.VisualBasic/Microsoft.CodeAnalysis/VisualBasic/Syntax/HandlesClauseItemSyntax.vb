Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class HandlesClauseItemSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _eventContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventContainerSyntax

		Friend _eventMember As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax

		Public ReadOnly Property DotToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax)._dotToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property EventContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventContainerSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EventContainerSyntax)(Me._eventContainer)
			End Get
		End Property

		Public ReadOnly Property EventMember As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)(Me._eventMember, 2)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal eventContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventContainerSyntax, ByVal dotToken As PunctuationSyntax, ByVal eventMember As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax(kind, errors, annotations, DirectCast(eventContainer.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax), dotToken, DirectCast(eventMember.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitHandlesClauseItem(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitHandlesClauseItem(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._eventContainer
			ElseIf (num = 2) Then
				syntaxNode = Me._eventMember
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim eventContainer As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				eventContainer = Me.EventContainer
			ElseIf (num = 2) Then
				eventContainer = Me.EventMember
			Else
				eventContainer = Nothing
			End If
			Return eventContainer
		End Function

		Public Function Update(ByVal eventContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventContainerSyntax, ByVal dotToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal eventMember As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax
			Dim handlesClauseItemSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax
			If (eventContainer <> Me.EventContainer OrElse dotToken <> Me.DotToken OrElse eventMember <> Me.EventMember) Then
				Dim handlesClauseItemSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.HandlesClauseItem(eventContainer, dotToken, eventMember)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				handlesClauseItemSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, handlesClauseItemSyntax1, handlesClauseItemSyntax1.WithAnnotations(annotations))
			Else
				handlesClauseItemSyntax = Me
			End If
			Return handlesClauseItemSyntax
		End Function

		Public Function WithDotToken(ByVal dotToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax
			Return Me.Update(Me.EventContainer, dotToken, Me.EventMember)
		End Function

		Public Function WithEventContainer(ByVal eventContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventContainerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax
			Return Me.Update(eventContainer, Me.DotToken, Me.EventMember)
		End Function

		Public Function WithEventMember(ByVal eventMember As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax
			Return Me.Update(Me.EventContainer, Me.DotToken, eventMember)
		End Function
	End Class
End Namespace