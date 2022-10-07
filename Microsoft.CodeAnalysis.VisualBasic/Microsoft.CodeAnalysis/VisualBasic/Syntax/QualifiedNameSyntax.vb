Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class QualifiedNameSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax
		Friend _left As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax

		Friend _right As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax

		Public ReadOnly Property DotToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax)._dotToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Left As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax)(Me._left)
			End Get
		End Property

		Public ReadOnly Property Right As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax)(Me._right, 2)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax, ByVal dotToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax(kind, errors, annotations, DirectCast(left.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax), dotToken, DirectCast(right.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitQualifiedName(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitQualifiedName(Me)
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

		Public Function Update(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax, ByVal dotToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax
			Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax
			If (left <> Me.Left OrElse dotToken <> Me.DotToken OrElse right <> Me.Right) Then
				Dim qualifiedNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.QualifiedName(left, dotToken, right)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				qualifiedNameSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, qualifiedNameSyntax1, qualifiedNameSyntax1.WithAnnotations(annotations))
			Else
				qualifiedNameSyntax = Me
			End If
			Return qualifiedNameSyntax
		End Function

		Public Function WithDotToken(ByVal dotToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax
			Return Me.Update(Me.Left, dotToken, Me.Right)
		End Function

		Public Function WithLeft(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax
			Return Me.Update(left, Me.DotToken, Me.Right)
		End Function

		Public Function WithRight(ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax
			Return Me.Update(Me.Left, Me.DotToken, right)
		End Function
	End Class
End Namespace