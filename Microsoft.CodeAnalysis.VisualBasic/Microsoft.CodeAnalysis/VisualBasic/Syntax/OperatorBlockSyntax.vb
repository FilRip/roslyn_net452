Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class OperatorBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
		Friend _operatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax

		Friend _endOperatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows ReadOnly Property Begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax
			Get
				Return Me.OperatorStatement
			End Get
		End Property

		Public Overrides ReadOnly Property BlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Get
				Return Me.OperatorStatement
			End Get
		End Property

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows ReadOnly Property [End] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return Me.EndOperatorStatement
			End Get
		End Property

		Public Overrides ReadOnly Property EndBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return Me.EndOperatorStatement
			End Get
		End Property

		Public ReadOnly Property EndOperatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endOperatorStatement, 2)
			End Get
		End Property

		Public ReadOnly Property OperatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax)(Me._operatorStatement)
			End Get
		End Property

		Public Shadows ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal operatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax, ByVal statements As SyntaxNode, ByVal endOperatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax(kind, errors, annotations, DirectCast(operatorStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing), DirectCast(endOperatorStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitOperatorBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitOperatorBlock(Me)
		End Sub

		Public Shadows Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function AddStatementsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.AddStatements(items)
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._operatorStatement
					Exit Select
				Case 1
					syntaxNode = Me._statements
					Exit Select
				Case 2
					syntaxNode = Me._endOperatorStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim operatorStatement As SyntaxNode
			Select Case i
				Case 0
					operatorStatement = Me.OperatorStatement
					Exit Select
				Case 1
					operatorStatement = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					operatorStatement = Me.EndOperatorStatement
					Exit Select
				Case Else
					operatorStatement = Nothing
					Exit Select
			End Select
			Return operatorStatement
		End Function

		Friend Overrides Function GetStatementsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Return Me.Statements
		End Function

		Public Function Update(ByVal operatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endOperatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax
			Dim operatorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax
			If (operatorStatement <> Me.OperatorStatement OrElse statements <> Me.Statements OrElse endOperatorStatement <> Me.EndOperatorStatement) Then
				Dim operatorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.OperatorBlock(operatorStatement, statements, endOperatorStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				operatorBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, operatorBlockSyntax1, operatorBlockSyntax1.WithAnnotations(annotations))
			Else
				operatorBlockSyntax = Me
			End If
			Return operatorBlockSyntax
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Function WithBegin(ByVal begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax
			Return Me.WithOperatorStatement(begin)
		End Function

		Public Overrides Function WithBlockStatement(ByVal blockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.WithOperatorStatement(DirectCast(blockStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax))
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows Function WithEnd(ByVal [end] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax
			Return Me.WithEndOperatorStatement([end])
		End Function

		Public Overrides Function WithEndBlockStatement(ByVal endBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.WithEndOperatorStatement(endBlockStatement)
		End Function

		Public Function WithEndOperatorStatement(ByVal endOperatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax
			Return Me.Update(Me.OperatorStatement, Me.Statements, endOperatorStatement)
		End Function

		Public Function WithOperatorStatement(ByVal operatorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax
			Return Me.Update(operatorStatement, Me.Statements, Me.EndOperatorStatement)
		End Function

		Public Shadows Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax
			Return Me.Update(Me.OperatorStatement, statements, Me.EndOperatorStatement)
		End Function

		Friend Overrides Function WithStatementsCore(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.WithStatements(statements)
		End Function
	End Class
End Namespace