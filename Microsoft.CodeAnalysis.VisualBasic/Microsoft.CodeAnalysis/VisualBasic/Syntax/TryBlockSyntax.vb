Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class TryBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _tryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax

		Friend _statements As SyntaxNode

		Friend _catchBlocks As SyntaxNode

		Friend _finallyBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax

		Friend _endTryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		Public ReadOnly Property CatchBlocks As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax)(MyBase.GetRed(Me._catchBlocks, 2))
			End Get
		End Property

		Public ReadOnly Property EndTryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endTryStatement, 4)
			End Get
		End Property

		Public ReadOnly Property FinallyBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax)(Me._finallyBlock, 3)
			End Get
		End Property

		Public ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 1))
			End Get
		End Property

		Public ReadOnly Property TryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax)(Me._tryStatement)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal tryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax, ByVal statements As SyntaxNode, ByVal catchBlocks As SyntaxNode, ByVal finallyBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax, ByVal endTryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax(kind, errors, annotations, DirectCast(tryStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing), If(catchBlocks IsNot Nothing, catchBlocks.Green, Nothing), If(finallyBlock IsNot Nothing, DirectCast(finallyBlock.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax), Nothing), DirectCast(endTryStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitTryBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitTryBlock(Me)
		End Sub

		Public Function AddCatchBlocks(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax
			Return Me.WithCatchBlocks(Me.CatchBlocks.AddRange(items))
		End Function

		Public Function AddFinallyBlockStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax
			Return Me.WithFinallyBlock(If(Me.FinallyBlock IsNot Nothing, Me.FinallyBlock, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.FinallyBlock()).AddStatements(items))
		End Function

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._tryStatement
					Exit Select
				Case 1
					syntaxNode = Me._statements
					Exit Select
				Case 2
					syntaxNode = Me._catchBlocks
					Exit Select
				Case 3
					syntaxNode = Me._finallyBlock
					Exit Select
				Case 4
					syntaxNode = Me._endTryStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim tryStatement As SyntaxNode
			Select Case i
				Case 0
					tryStatement = Me.TryStatement
					Exit Select
				Case 1
					tryStatement = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					tryStatement = MyBase.GetRed(Me._catchBlocks, 2)
					Exit Select
				Case 3
					tryStatement = Me.FinallyBlock
					Exit Select
				Case 4
					tryStatement = Me.EndTryStatement
					Exit Select
				Case Else
					tryStatement = Nothing
					Exit Select
			End Select
			Return tryStatement
		End Function

		Public Function Update(ByVal tryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal catchBlocks As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax), ByVal finallyBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax, ByVal endTryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax
			Dim tryBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax
			If (tryStatement <> Me.TryStatement OrElse statements <> Me.Statements OrElse catchBlocks <> Me.CatchBlocks OrElse finallyBlock <> Me.FinallyBlock OrElse endTryStatement <> Me.EndTryStatement) Then
				Dim tryBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TryBlock(tryStatement, statements, catchBlocks, finallyBlock, endTryStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				tryBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, tryBlockSyntax1, tryBlockSyntax1.WithAnnotations(annotations))
			Else
				tryBlockSyntax = Me
			End If
			Return tryBlockSyntax
		End Function

		Public Function WithCatchBlocks(ByVal catchBlocks As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax
			Return Me.Update(Me.TryStatement, Me.Statements, catchBlocks, Me.FinallyBlock, Me.EndTryStatement)
		End Function

		Public Function WithEndTryStatement(ByVal endTryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax
			Return Me.Update(Me.TryStatement, Me.Statements, Me.CatchBlocks, Me.FinallyBlock, endTryStatement)
		End Function

		Public Function WithFinallyBlock(ByVal finallyBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax
			Return Me.Update(Me.TryStatement, Me.Statements, Me.CatchBlocks, finallyBlock, Me.EndTryStatement)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax
			Return Me.Update(Me.TryStatement, statements, Me.CatchBlocks, Me.FinallyBlock, Me.EndTryStatement)
		End Function

		Public Function WithTryStatement(ByVal tryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax
			Return Me.Update(tryStatement, Me.Statements, Me.CatchBlocks, Me.FinallyBlock, Me.EndTryStatement)
		End Function
	End Class
End Namespace