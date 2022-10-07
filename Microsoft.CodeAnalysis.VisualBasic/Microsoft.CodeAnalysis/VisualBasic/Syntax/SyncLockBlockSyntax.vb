Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SyncLockBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _syncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax

		Friend _statements As SyntaxNode

		Friend _endSyncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		Public ReadOnly Property EndSyncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endSyncLockStatement, 2)
			End Get
		End Property

		Public ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 1))
			End Get
		End Property

		Public ReadOnly Property SyncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax)(Me._syncLockStatement)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal syncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax, ByVal statements As SyntaxNode, ByVal endSyncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax(kind, errors, annotations, DirectCast(syncLockStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing), DirectCast(endSyncLockStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSyncLockBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSyncLockBlock(Me)
		End Sub

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._syncLockStatement
					Exit Select
				Case 1
					syntaxNode = Me._statements
					Exit Select
				Case 2
					syntaxNode = Me._endSyncLockStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim syncLockStatement As SyntaxNode
			Select Case i
				Case 0
					syncLockStatement = Me.SyncLockStatement
					Exit Select
				Case 1
					syncLockStatement = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					syncLockStatement = Me.EndSyncLockStatement
					Exit Select
				Case Else
					syncLockStatement = Nothing
					Exit Select
			End Select
			Return syncLockStatement
		End Function

		Public Function Update(ByVal syncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endSyncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax
			Dim syncLockBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax
			If (syncLockStatement <> Me.SyncLockStatement OrElse statements <> Me.Statements OrElse endSyncLockStatement <> Me.EndSyncLockStatement) Then
				Dim syncLockBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SyncLockBlock(syncLockStatement, statements, endSyncLockStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				syncLockBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, syncLockBlockSyntax1, syncLockBlockSyntax1.WithAnnotations(annotations))
			Else
				syncLockBlockSyntax = Me
			End If
			Return syncLockBlockSyntax
		End Function

		Public Function WithEndSyncLockStatement(ByVal endSyncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax
			Return Me.Update(Me.SyncLockStatement, Me.Statements, endSyncLockStatement)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax
			Return Me.Update(Me.SyncLockStatement, statements, Me.EndSyncLockStatement)
		End Function

		Public Function WithSyncLockStatement(ByVal syncLockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax
			Return Me.Update(syncLockStatement, Me.Statements, Me.EndSyncLockStatement)
		End Function
	End Class
End Namespace