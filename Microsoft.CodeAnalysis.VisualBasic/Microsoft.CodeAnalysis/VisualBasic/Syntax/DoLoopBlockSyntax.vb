Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class DoLoopBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax

		Friend _statements As SyntaxNode

		Friend _loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax

		Public ReadOnly Property DoStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax)(Me._doStatement)
			End Get
		End Property

		Public ReadOnly Property LoopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax)(Me._loopStatement, 2)
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

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax, ByVal statements As SyntaxNode, ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(kind, errors, annotations, DirectCast(doStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing), DirectCast(loopStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitDoLoopBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitDoLoopBlock(Me)
		End Sub

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._doStatement
					Exit Select
				Case 1
					syntaxNode = Me._statements
					Exit Select
				Case 2
					syntaxNode = Me._loopStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim doStatement As SyntaxNode
			Select Case i
				Case 0
					doStatement = Me.DoStatement
					Exit Select
				Case 1
					doStatement = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					doStatement = Me.LoopStatement
					Exit Select
				Case Else
					doStatement = Nothing
					Exit Select
			End Select
			Return doStatement
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax
			Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax
			If (kind <> MyBase.Kind() OrElse doStatement <> Me.DoStatement OrElse statements <> Me.Statements OrElse loopStatement <> Me.LoopStatement) Then
				Dim doLoopBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.DoLoopBlock(kind, doStatement, statements, loopStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				doLoopBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, doLoopBlockSyntax1, doLoopBlockSyntax1.WithAnnotations(annotations))
			Else
				doLoopBlockSyntax = Me
			End If
			Return doLoopBlockSyntax
		End Function

		Public Function WithDoStatement(ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax
			Return Me.Update(MyBase.Kind(), doStatement, Me.Statements, Me.LoopStatement)
		End Function

		Public Function WithLoopStatement(ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax
			Return Me.Update(MyBase.Kind(), Me.DoStatement, Me.Statements, loopStatement)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax
			Return Me.Update(MyBase.Kind(), Me.DoStatement, statements, Me.LoopStatement)
		End Function
	End Class
End Namespace