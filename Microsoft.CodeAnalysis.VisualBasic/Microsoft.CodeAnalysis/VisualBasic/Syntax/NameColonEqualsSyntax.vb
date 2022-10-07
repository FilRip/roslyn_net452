Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class NameColonEqualsSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax

		Public ReadOnly Property ColonEqualsToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax)._colonEqualsToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)(Me._name)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax, ByVal colonEqualsToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax(kind, errors, annotations, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax), colonEqualsToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitNameColonEquals(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitNameColonEquals(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 0) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._name
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim name As SyntaxNode
			If (i <> 0) Then
				name = Nothing
			Else
				name = Me.Name
			End If
			Return name
		End Function

		Public Function Update(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax, ByVal colonEqualsToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax
			Dim nameColonEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax
			If (name <> Me.Name OrElse colonEqualsToken <> Me.ColonEqualsToken) Then
				Dim nameColonEqualsSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.NameColonEquals(name, colonEqualsToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				nameColonEqualsSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, nameColonEqualsSyntax1, nameColonEqualsSyntax1.WithAnnotations(annotations))
			Else
				nameColonEqualsSyntax = Me
			End If
			Return nameColonEqualsSyntax
		End Function

		Public Function WithColonEqualsToken(ByVal colonEqualsToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax
			Return Me.Update(Me.Name, colonEqualsToken)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax
			Return Me.Update(name, Me.ColonEqualsToken)
		End Function
	End Class
End Namespace