Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class QualifiedCrefOperatorReferenceSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax
		Friend _left As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax

		Friend _right As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax

		Public ReadOnly Property DotToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax)._dotToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Left As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)(Me._left)
			End Get
		End Property

		Public ReadOnly Property Right As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax)(Me._right, 2)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax, ByVal dotToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax(kind, errors, annotations, DirectCast(left.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax), dotToken, DirectCast(right.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitQualifiedCrefOperatorReference(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitQualifiedCrefOperatorReference(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._left
			ElseIf (num = 2) Then
				syntaxNode = Me._right
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim left As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				left = Me.Left
			ElseIf (num = 2) Then
				left = Me.Right
			Else
				left = Nothing
			End If
			Return left
		End Function

		Public Function Update(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax, ByVal dotToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedCrefOperatorReferenceSyntax
			Dim qualifiedCrefOperatorReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedCrefOperatorReferenceSyntax
			If (left <> Me.Left OrElse dotToken <> Me.DotToken OrElse right <> Me.Right) Then
				Dim qualifiedCrefOperatorReferenceSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedCrefOperatorReferenceSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.QualifiedCrefOperatorReference(left, dotToken, right)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				qualifiedCrefOperatorReferenceSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, qualifiedCrefOperatorReferenceSyntax1, qualifiedCrefOperatorReferenceSyntax1.WithAnnotations(annotations))
			Else
				qualifiedCrefOperatorReferenceSyntax = Me
			End If
			Return qualifiedCrefOperatorReferenceSyntax
		End Function

		Public Function WithDotToken(ByVal dotToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedCrefOperatorReferenceSyntax
			Return Me.Update(Me.Left, dotToken, Me.Right)
		End Function

		Public Function WithLeft(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedCrefOperatorReferenceSyntax
			Return Me.Update(left, Me.DotToken, Me.Right)
		End Function

		Public Function WithRight(ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedCrefOperatorReferenceSyntax
			Return Me.Update(Me.Left, Me.DotToken, right)
		End Function
	End Class
End Namespace