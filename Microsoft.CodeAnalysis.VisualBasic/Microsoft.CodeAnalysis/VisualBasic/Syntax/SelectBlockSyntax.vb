Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SelectBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _selectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax

		Friend _caseBlocks As SyntaxNode

		Friend _endSelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		Public ReadOnly Property CaseBlocks As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax)(MyBase.GetRed(Me._caseBlocks, 1))
			End Get
		End Property

		Public ReadOnly Property EndSelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endSelectStatement, 2)
			End Get
		End Property

		Public ReadOnly Property SelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax)(Me._selectStatement)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal selectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax, ByVal caseBlocks As SyntaxNode, ByVal endSelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax(kind, errors, annotations, DirectCast(selectStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax), If(caseBlocks IsNot Nothing, caseBlocks.Green, Nothing), DirectCast(endSelectStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSelectBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSelectBlock(Me)
		End Sub

		Public Function AddCaseBlocks(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax
			Return Me.WithCaseBlocks(Me.CaseBlocks.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._selectStatement
					Exit Select
				Case 1
					syntaxNode = Me._caseBlocks
					Exit Select
				Case 2
					syntaxNode = Me._endSelectStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim selectStatement As SyntaxNode
			Select Case i
				Case 0
					selectStatement = Me.SelectStatement
					Exit Select
				Case 1
					selectStatement = MyBase.GetRed(Me._caseBlocks, 1)
					Exit Select
				Case 2
					selectStatement = Me.EndSelectStatement
					Exit Select
				Case Else
					selectStatement = Nothing
					Exit Select
			End Select
			Return selectStatement
		End Function

		Public Function Update(ByVal selectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax, ByVal caseBlocks As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax), ByVal endSelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax
			Dim selectBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax
			If (selectStatement <> Me.SelectStatement OrElse caseBlocks <> Me.CaseBlocks OrElse endSelectStatement <> Me.EndSelectStatement) Then
				Dim selectBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SelectBlock(selectStatement, caseBlocks, endSelectStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				selectBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, selectBlockSyntax1, selectBlockSyntax1.WithAnnotations(annotations))
			Else
				selectBlockSyntax = Me
			End If
			Return selectBlockSyntax
		End Function

		Public Function WithCaseBlocks(ByVal caseBlocks As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax
			Return Me.Update(Me.SelectStatement, caseBlocks, Me.EndSelectStatement)
		End Function

		Public Function WithEndSelectStatement(ByVal endSelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax
			Return Me.Update(Me.SelectStatement, Me.CaseBlocks, endSelectStatement)
		End Function

		Public Function WithSelectStatement(ByVal selectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax
			Return Me.Update(selectStatement, Me.CaseBlocks, Me.EndSelectStatement)
		End Function
	End Class
End Namespace