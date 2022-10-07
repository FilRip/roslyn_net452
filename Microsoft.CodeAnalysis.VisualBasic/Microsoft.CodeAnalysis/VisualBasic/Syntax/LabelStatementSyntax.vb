Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class LabelStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Public ReadOnly Property ColonToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax)._colonToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property LabelToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax)._labelToken, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal labelToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal colonToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax(kind, errors, annotations, labelToken, colonToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitLabelStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitLabelStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal labelToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal colonToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelStatementSyntax
			Dim labelStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelStatementSyntax
			If (labelToken <> Me.LabelToken OrElse colonToken <> Me.ColonToken) Then
				Dim labelStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.LabelStatement(labelToken, colonToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				labelStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, labelStatementSyntax1, labelStatementSyntax1.WithAnnotations(annotations))
			Else
				labelStatementSyntax = Me
			End If
			Return labelStatementSyntax
		End Function

		Public Function WithColonToken(ByVal colonToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelStatementSyntax
			Return Me.Update(Me.LabelToken, colonToken)
		End Function

		Public Function WithLabelToken(ByVal labelToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelStatementSyntax
			Return Me.Update(labelToken, Me.ColonToken)
		End Function
	End Class
End Namespace