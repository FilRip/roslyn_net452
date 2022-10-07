Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class OnErrorResumeNextStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Public ReadOnly Property ErrorKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax)._errorKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property NextKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax)._nextKeyword, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property OnKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax)._onKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property ResumeKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax)._resumeKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal resumeKeyword As KeywordSyntax, ByVal nextKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax(kind, errors, annotations, onKeyword, errorKeyword, resumeKeyword, nextKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitOnErrorResumeNextStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitOnErrorResumeNextStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal onKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal errorKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal resumeKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal nextKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax
			Dim onErrorResumeNextStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax
			If (onKeyword <> Me.OnKeyword OrElse errorKeyword <> Me.ErrorKeyword OrElse resumeKeyword <> Me.ResumeKeyword OrElse nextKeyword <> Me.NextKeyword) Then
				Dim onErrorResumeNextStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.OnErrorResumeNextStatement(onKeyword, errorKeyword, resumeKeyword, nextKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				onErrorResumeNextStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, onErrorResumeNextStatementSyntax1, onErrorResumeNextStatementSyntax1.WithAnnotations(annotations))
			Else
				onErrorResumeNextStatementSyntax = Me
			End If
			Return onErrorResumeNextStatementSyntax
		End Function

		Public Function WithErrorKeyword(ByVal errorKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax
			Return Me.Update(Me.OnKeyword, errorKeyword, Me.ResumeKeyword, Me.NextKeyword)
		End Function

		Public Function WithNextKeyword(ByVal nextKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax
			Return Me.Update(Me.OnKeyword, Me.ErrorKeyword, Me.ResumeKeyword, nextKeyword)
		End Function

		Public Function WithOnKeyword(ByVal onKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax
			Return Me.Update(onKeyword, Me.ErrorKeyword, Me.ResumeKeyword, Me.NextKeyword)
		End Function

		Public Function WithResumeKeyword(ByVal resumeKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax
			Return Me.Update(Me.OnKeyword, Me.ErrorKeyword, resumeKeyword, Me.NextKeyword)
		End Function
	End Class
End Namespace