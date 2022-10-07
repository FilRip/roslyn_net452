Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class StopOrEndStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Public ReadOnly Property StopOrEndKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax)._stopOrEndKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal stopOrEndKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax(kind, errors, annotations, stopOrEndKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitStopOrEndStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitStopOrEndStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal stopOrEndKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StopOrEndStatementSyntax
			Dim stopOrEndStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StopOrEndStatementSyntax
			If (kind <> MyBase.Kind() OrElse stopOrEndKeyword <> Me.StopOrEndKeyword) Then
				Dim stopOrEndStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.StopOrEndStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.StopOrEndStatement(kind, stopOrEndKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				stopOrEndStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, stopOrEndStatementSyntax1, stopOrEndStatementSyntax1.WithAnnotations(annotations))
			Else
				stopOrEndStatementSyntax = Me
			End If
			Return stopOrEndStatementSyntax
		End Function

		Public Function WithStopOrEndKeyword(ByVal stopOrEndKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StopOrEndStatementSyntax
			Return Me.Update(MyBase.Kind(), stopOrEndKeyword)
		End Function
	End Class
End Namespace