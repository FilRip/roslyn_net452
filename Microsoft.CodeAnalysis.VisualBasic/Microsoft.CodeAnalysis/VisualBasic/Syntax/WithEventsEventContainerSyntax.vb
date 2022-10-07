Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class WithEventsEventContainerSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.EventContainerSyntax
		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax)._identifier, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal identifier As IdentifierTokenSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax(kind, errors, annotations, identifier), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitWithEventsEventContainer(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitWithEventsEventContainer(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax
			Dim withEventsEventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax
			If (identifier = Me.Identifier) Then
				withEventsEventContainerSyntax = Me
			Else
				Dim withEventsEventContainerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.WithEventsEventContainer(identifier)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				withEventsEventContainerSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, withEventsEventContainerSyntax1, withEventsEventContainerSyntax1.WithAnnotations(annotations))
			End If
			Return withEventsEventContainerSyntax
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithEventsEventContainerSyntax
			Return Me.Update(identifier)
		End Function
	End Class
End Namespace