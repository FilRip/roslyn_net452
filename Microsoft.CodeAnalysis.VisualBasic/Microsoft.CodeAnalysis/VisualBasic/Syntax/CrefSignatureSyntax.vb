Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CrefSignatureSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _argumentTypes As SyntaxNode

		Public ReadOnly Property ArgumentTypes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax)
			Get
				Dim crefSignaturePartSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._argumentTypes, 1)
				crefSignaturePartSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax)(red, MyBase.GetChildIndex(1)))
				Return crefSignaturePartSyntaxes
			End Get
		End Property

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax)._closeParenToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax)._openParenToken, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal argumentTypes As SyntaxNode, ByVal closeParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax(kind, errors, annotations, openParenToken, If(argumentTypes IsNot Nothing, argumentTypes.Green, Nothing), closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCrefSignature(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCrefSignature(Me)
		End Sub

		Public Function AddArgumentTypes(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax
			Return Me.WithArgumentTypes(Me.ArgumentTypes.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._argumentTypes
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._argumentTypes, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal argumentTypes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax), ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax
			Dim crefSignatureSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax
			If (openParenToken <> Me.OpenParenToken OrElse argumentTypes <> Me.ArgumentTypes OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim crefSignatureSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CrefSignature(openParenToken, argumentTypes, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				crefSignatureSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, crefSignatureSyntax1, crefSignatureSyntax1.WithAnnotations(annotations))
			Else
				crefSignatureSyntax = Me
			End If
			Return crefSignatureSyntax
		End Function

		Public Function WithArgumentTypes(ByVal argumentTypes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignaturePartSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax
			Return Me.Update(Me.OpenParenToken, argumentTypes, Me.CloseParenToken)
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax
			Return Me.Update(Me.OpenParenToken, Me.ArgumentTypes, closeParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax
			Return Me.Update(openParenToken, Me.ArgumentTypes, Me.CloseParenToken)
		End Function
	End Class
End Namespace