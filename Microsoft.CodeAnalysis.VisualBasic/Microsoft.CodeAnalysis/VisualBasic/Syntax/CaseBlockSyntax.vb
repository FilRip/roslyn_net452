Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CaseBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _caseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax

		Friend _statements As SyntaxNode

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use CaseStatement instead.", True)>
		Public ReadOnly Property Begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax
			Get
				Return Me.CaseStatement
			End Get
		End Property

		Public ReadOnly Property CaseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax)(Me._caseStatement)
			End Get
		End Property

		Public ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal caseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax, ByVal statements As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax(kind, errors, annotations, DirectCast(caseStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCaseBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCaseBlock(Me)
		End Sub

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use AddCaseStatementCases instead.", True)>
		Public Function AddBeginCases(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax
			Return Me.AddCaseStatementCases(items)
		End Function

		Public Function AddCaseStatementCases(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax
			Return Me.WithCaseStatement(If(Me.CaseStatement IsNot Nothing, Me.CaseStatement, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CaseStatement(New Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax(-1) {})).AddCases(items))
		End Function

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._caseStatement
			ElseIf (num = 1) Then
				syntaxNode = Me._statements
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim caseStatement As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				caseStatement = Me.CaseStatement
			ElseIf (num = 1) Then
				caseStatement = MyBase.GetRed(Me._statements, 1)
			Else
				caseStatement = Nothing
			End If
			Return caseStatement
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal caseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax
			Dim caseBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax
			If (kind <> MyBase.Kind() OrElse caseStatement <> Me.CaseStatement OrElse statements <> Me.Statements) Then
				Dim caseBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CaseBlock(kind, caseStatement, statements)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				caseBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, caseBlockSyntax1, caseBlockSyntax1.WithAnnotations(annotations))
			Else
				caseBlockSyntax = Me
			End If
			Return caseBlockSyntax
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use WithCaseStatement instead.", True)>
		Public Function WithBegin(ByVal begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax
			Return Me.WithCaseStatement(begin)
		End Function

		Public Function WithCaseStatement(ByVal caseStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax
			Return Me.Update(MyBase.Kind(), caseStatement, Me.Statements)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax
			Return Me.Update(MyBase.Kind(), Me.CaseStatement, statements)
		End Function
	End Class
End Namespace