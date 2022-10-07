Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ResumeStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax

		Public ReadOnly Property Label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax)(Me._label, 1)
			End Get
		End Property

		Public ReadOnly Property ResumeKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax)._resumeKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal resumeKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax(kind, errors, annotations, resumeKeyword, If(label IsNot Nothing, DirectCast(label.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitResumeStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitResumeStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._label
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim label As SyntaxNode
			If (i <> 1) Then
				label = Nothing
			Else
				label = Me.Label
			End If
			Return label
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal resumeKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ResumeStatementSyntax
			Dim resumeStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ResumeStatementSyntax
			If (kind <> MyBase.Kind() OrElse resumeKeyword <> Me.ResumeKeyword OrElse label <> Me.Label) Then
				Dim resumeStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ResumeStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ResumeStatement(kind, resumeKeyword, label)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				resumeStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, resumeStatementSyntax1, resumeStatementSyntax1.WithAnnotations(annotations))
			Else
				resumeStatementSyntax = Me
			End If
			Return resumeStatementSyntax
		End Function

		Public Function WithLabel(ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ResumeStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.ResumeKeyword, label)
		End Function

		Public Function WithResumeKeyword(ByVal resumeKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ResumeStatementSyntax
			Return Me.Update(MyBase.Kind(), resumeKeyword, Me.Label)
		End Function
	End Class
End Namespace