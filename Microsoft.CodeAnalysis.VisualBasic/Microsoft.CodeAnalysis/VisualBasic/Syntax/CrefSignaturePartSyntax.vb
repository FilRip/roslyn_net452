Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CrefSignaturePartSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax

		Public ReadOnly Property Modifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax)._modifier
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, MyBase.Position, 0))
				Return syntaxToken
			End Get
		End Property

		Public ReadOnly Property Type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(Me._type, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal modifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax(kind, errors, annotations, modifier, If(type IsNot Nothing, DirectCast(type.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCrefSignaturePart(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCrefSignaturePart(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._type
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim type As SyntaxNode
			If (i <> 1) Then
				type = Nothing
			Else
				type = Me.Type
			End If
			Return type
		End Function

		Public Function Update(ByVal modifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax
			Dim crefSignaturePartSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax
			If (modifier <> Me.Modifier OrElse type <> Me.Type) Then
				Dim crefSignaturePartSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CrefSignaturePart(modifier, type)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				crefSignaturePartSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, crefSignaturePartSyntax1, crefSignaturePartSyntax1.WithAnnotations(annotations))
			Else
				crefSignaturePartSyntax = Me
			End If
			Return crefSignaturePartSyntax
		End Function

		Public Function WithModifier(ByVal modifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax
			Return Me.Update(modifier, Me.Type)
		End Function

		Public Function WithType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax
			Return Me.Update(Me.Modifier, type)
		End Function
	End Class
End Namespace