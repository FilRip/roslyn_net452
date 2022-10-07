Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CaseStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
		Friend _cases As SyntaxNode

		Public ReadOnly Property CaseKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax)._caseKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Cases As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax)
			Get
				Dim caseClauseSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._cases, 1)
				caseClauseSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax)(red, MyBase.GetChildIndex(1)))
				Return caseClauseSyntaxes
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal caseKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal cases As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax(kind, errors, annotations, caseKeyword, If(cases IsNot Nothing, cases.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCaseStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCaseStatement(Me)
		End Sub

		Public Function AddCases(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax
			Return Me.WithCases(Me.Cases.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._cases
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._cases, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal caseKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal cases As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax
			Dim caseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax
			If (kind <> MyBase.Kind() OrElse caseKeyword <> Me.CaseKeyword OrElse cases <> Me.Cases) Then
				Dim caseStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CaseStatement(kind, caseKeyword, cases)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				caseStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, caseStatementSyntax1, caseStatementSyntax1.WithAnnotations(annotations))
			Else
				caseStatementSyntax = Me
			End If
			Return caseStatementSyntax
		End Function

		Public Function WithCaseKeyword(ByVal caseKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax
			Return Me.Update(MyBase.Kind(), caseKeyword, Me.Cases)
		End Function

		Public Function WithCases(ByVal cases As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.CaseKeyword, cases)
		End Function
	End Class
End Namespace