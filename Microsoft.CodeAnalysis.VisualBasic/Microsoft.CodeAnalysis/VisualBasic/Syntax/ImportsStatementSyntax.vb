Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ImportsStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Friend _importsClauses As SyntaxNode

		Public ReadOnly Property ImportsClauses As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax)
			Get
				Dim importsClauseSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._importsClauses, 1)
				importsClauseSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax)(red, MyBase.GetChildIndex(1)))
				Return importsClauseSyntaxes
			End Get
		End Property

		Public ReadOnly Property ImportsKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax)._importsKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal importsKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal importsClauses As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax(kind, errors, annotations, importsKeyword, If(importsClauses IsNot Nothing, importsClauses.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitImportsStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitImportsStatement(Me)
		End Sub

		Public Function AddImportsClauses(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax
			Return Me.WithImportsClauses(Me.ImportsClauses.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._importsClauses
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._importsClauses, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal importsKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal importsClauses As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax
			Dim importsStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax
			If (importsKeyword <> Me.ImportsKeyword OrElse importsClauses <> Me.ImportsClauses) Then
				Dim importsStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ImportsStatement(importsKeyword, importsClauses)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				importsStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, importsStatementSyntax1, importsStatementSyntax1.WithAnnotations(annotations))
			Else
				importsStatementSyntax = Me
			End If
			Return importsStatementSyntax
		End Function

		Public Function WithImportsClauses(ByVal importsClauses As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax
			Return Me.Update(Me.ImportsKeyword, importsClauses)
		End Function

		Public Function WithImportsKeyword(ByVal importsKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax
			Return Me.Update(importsKeyword, Me.ImportsClauses)
		End Function
	End Class
End Namespace