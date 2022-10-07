Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CrefOperatorReferenceSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax
		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use OperatorKeyword or a more specific property (e.g. OperatorKeyword) instead.", True)>
		Public ReadOnly Property Keyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.OperatorKeyword
			End Get
		End Property

		Public ReadOnly Property OperatorKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax)._operatorKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property OperatorToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax)._operatorToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal operatorKeyword As KeywordSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax(kind, errors, annotations, operatorKeyword, operatorToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCrefOperatorReference(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCrefOperatorReference(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal operatorKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal operatorToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax
			Dim crefOperatorReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax
			If (operatorKeyword <> Me.OperatorKeyword OrElse operatorToken <> Me.OperatorToken) Then
				Dim crefOperatorReferenceSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CrefOperatorReference(operatorKeyword, operatorToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				crefOperatorReferenceSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, crefOperatorReferenceSyntax1, crefOperatorReferenceSyntax1.WithAnnotations(annotations))
			Else
				crefOperatorReferenceSyntax = Me
			End If
			Return crefOperatorReferenceSyntax
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use OperatorKeyword or a more specific property (e.g. WithOperatorKeyword) instead.", True)>
		Public Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax
			Return Me.WithOperatorKeyword(keyword)
		End Function

		Public Function WithOperatorKeyword(ByVal operatorKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax
			Return Me.Update(operatorKeyword, Me.OperatorToken)
		End Function

		Public Function WithOperatorToken(ByVal operatorToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax
			Return Me.Update(Me.OperatorKeyword, operatorToken)
		End Function
	End Class
End Namespace