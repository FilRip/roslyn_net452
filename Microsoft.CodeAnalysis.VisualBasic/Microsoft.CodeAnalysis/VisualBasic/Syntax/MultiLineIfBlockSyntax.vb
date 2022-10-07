Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class MultiLineIfBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _ifStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax

		Friend _statements As SyntaxNode

		Friend _elseIfBlocks As SyntaxNode

		Friend _elseBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax

		Friend _endIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		Public ReadOnly Property ElseBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax)(Me._elseBlock, 3)
			End Get
		End Property

		Public ReadOnly Property ElseIfBlocks As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax)(MyBase.GetRed(Me._elseIfBlocks, 2))
			End Get
		End Property

		Public ReadOnly Property EndIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endIfStatement, 4)
			End Get
		End Property

		Public ReadOnly Property IfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax)(Me._ifStatement)
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

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal ifStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax, ByVal statements As SyntaxNode, ByVal elseIfBlocks As SyntaxNode, ByVal elseBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax, ByVal endIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax(kind, errors, annotations, DirectCast(ifStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing), If(elseIfBlocks IsNot Nothing, elseIfBlocks.Green, Nothing), If(elseBlock IsNot Nothing, DirectCast(elseBlock.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax), Nothing), DirectCast(endIfStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitMultiLineIfBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitMultiLineIfBlock(Me)
		End Sub

		Public Function AddElseBlockStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax
			Return Me.WithElseBlock(If(Me.ElseBlock IsNot Nothing, Me.ElseBlock, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ElseBlock()).AddStatements(items))
		End Function

		Public Function AddElseIfBlocks(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax
			Return Me.WithElseIfBlocks(Me.ElseIfBlocks.AddRange(items))
		End Function

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._ifStatement
					Exit Select
				Case 1
					syntaxNode = Me._statements
					Exit Select
				Case 2
					syntaxNode = Me._elseIfBlocks
					Exit Select
				Case 3
					syntaxNode = Me._elseBlock
					Exit Select
				Case 4
					syntaxNode = Me._endIfStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim ifStatement As SyntaxNode
			Select Case i
				Case 0
					ifStatement = Me.IfStatement
					Exit Select
				Case 1
					ifStatement = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					ifStatement = MyBase.GetRed(Me._elseIfBlocks, 2)
					Exit Select
				Case 3
					ifStatement = Me.ElseBlock
					Exit Select
				Case 4
					ifStatement = Me.EndIfStatement
					Exit Select
				Case Else
					ifStatement = Nothing
					Exit Select
			End Select
			Return ifStatement
		End Function

		Public Function Update(ByVal ifStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal elseIfBlocks As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax), ByVal elseBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax, ByVal endIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax
			Dim multiLineIfBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax
			If (ifStatement <> Me.IfStatement OrElse statements <> Me.Statements OrElse elseIfBlocks <> Me.ElseIfBlocks OrElse elseBlock <> Me.ElseBlock OrElse endIfStatement <> Me.EndIfStatement) Then
				Dim multiLineIfBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.MultiLineIfBlock(ifStatement, statements, elseIfBlocks, elseBlock, endIfStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				multiLineIfBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, multiLineIfBlockSyntax1, multiLineIfBlockSyntax1.WithAnnotations(annotations))
			Else
				multiLineIfBlockSyntax = Me
			End If
			Return multiLineIfBlockSyntax
		End Function

		Public Function WithElseBlock(ByVal elseBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax
			Return Me.Update(Me.IfStatement, Me.Statements, Me.ElseIfBlocks, elseBlock, Me.EndIfStatement)
		End Function

		Public Function WithElseIfBlocks(ByVal elseIfBlocks As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax
			Return Me.Update(Me.IfStatement, Me.Statements, elseIfBlocks, Me.ElseBlock, Me.EndIfStatement)
		End Function

		Public Function WithEndIfStatement(ByVal endIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax
			Return Me.Update(Me.IfStatement, Me.Statements, Me.ElseIfBlocks, Me.ElseBlock, endIfStatement)
		End Function

		Public Function WithIfStatement(ByVal ifStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax
			Return Me.Update(ifStatement, Me.Statements, Me.ElseIfBlocks, Me.ElseBlock, Me.EndIfStatement)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax
			Return Me.Update(Me.IfStatement, statements, Me.ElseIfBlocks, Me.ElseBlock, Me.EndIfStatement)
		End Function
	End Class
End Namespace