Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class WithEventsPropertyEventContainerSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.EventContainerSyntax
		Friend _withEventsContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax

		Friend _property As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax

		Public ReadOnly Property DotToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax)._dotToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property [Property] As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)(Me._property, 2)
			End Get
		End Property

		Public ReadOnly Property WithEventsContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax)(Me._withEventsContainer)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal withEventsContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax, ByVal dotToken As PunctuationSyntax, ByVal [property] As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax(kind, errors, annotations, DirectCast(withEventsContainer.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax), dotToken, DirectCast([property].Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitWithEventsPropertyEventContainer(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitWithEventsPropertyEventContainer(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._withEventsContainer
			ElseIf (num = 2) Then
				syntaxNode = Me._property
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim withEventsContainer As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				withEventsContainer = Me.WithEventsContainer
			ElseIf (num = 2) Then
				withEventsContainer = Me.[Property]
			Else
				withEventsContainer = Nothing
			End If
			Return withEventsContainer
		End Function

		Public Function Update(ByVal withEventsContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax, ByVal dotToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal [property] As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsPropertyEventContainerSyntax
			Dim withEventsPropertyEventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsPropertyEventContainerSyntax
			If (withEventsContainer <> Me.WithEventsContainer OrElse dotToken <> Me.DotToken OrElse [property] <> Me.[Property]) Then
				Dim withEventsPropertyEventContainerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsPropertyEventContainerSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.WithEventsPropertyEventContainer(withEventsContainer, dotToken, [property])
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				withEventsPropertyEventContainerSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, withEventsPropertyEventContainerSyntax1, withEventsPropertyEventContainerSyntax1.WithAnnotations(annotations))
			Else
				withEventsPropertyEventContainerSyntax = Me
			End If
			Return withEventsPropertyEventContainerSyntax
		End Function

		Public Function WithDotToken(ByVal dotToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsPropertyEventContainerSyntax
			Return Me.Update(Me.WithEventsContainer, dotToken, Me.[Property])
		End Function

		Public Function WithProperty(ByVal [property] As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsPropertyEventContainerSyntax
			Return Me.Update(Me.WithEventsContainer, Me.DotToken, [property])
		End Function

		Public Function WithWithEventsContainer(ByVal withEventsContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsPropertyEventContainerSyntax
			Return Me.Update(withEventsContainer, Me.DotToken, Me.[Property])
		End Function
	End Class
End Namespace