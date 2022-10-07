Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class OmittedArgumentSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax
		Public ReadOnly Property Empty As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax)._empty, MyBase.Position, 0)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsNamed As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal empty As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax(kind, errors, annotations, empty), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitOmittedArgument(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitOmittedArgument(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		Public NotOverridable Overrides Function GetExpression() As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal empty As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OmittedArgumentSyntax
			Dim omittedArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.OmittedArgumentSyntax
			If (empty = Me.Empty) Then
				omittedArgumentSyntax = Me
			Else
				Dim omittedArgumentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.OmittedArgumentSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.OmittedArgument(empty)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				omittedArgumentSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, omittedArgumentSyntax1, omittedArgumentSyntax1.WithAnnotations(annotations))
			End If
			Return omittedArgumentSyntax
		End Function

		Public Function WithEmpty(ByVal empty As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OmittedArgumentSyntax
			Return Me.Update(empty)
		End Function
	End Class
End Namespace