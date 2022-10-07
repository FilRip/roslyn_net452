Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ForBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax
		Friend _forStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		Public Overrides ReadOnly Property ForOrForEachStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachStatementSyntax
			Get
				Return Me.ForStatement
			End Get
		End Property

		Public ReadOnly Property ForStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax)(Me._forStatement)
			End Get
		End Property

		Public Shadows ReadOnly Property NextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax)(Me._nextStatement, 2)
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

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal forStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax, ByVal statements As SyntaxNode, ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax(kind, errors, annotations, DirectCast(forStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing), If(nextStatement IsNot Nothing, DirectCast(nextStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitForBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitForBlock(Me)
		End Sub

		Public Shadows Function AddNextStatementControlVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax
			Return Me.WithNextStatement(If(Me.NextStatement IsNot Nothing, Me.NextStatement, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.NextStatement()).AddControlVariables(items))
		End Function

		Friend Overrides Function AddNextStatementControlVariablesCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax
			Return Me.AddNextStatementControlVariables(items)
		End Function

		Public Shadows Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function AddStatementsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax
			Return Me.AddStatements(items)
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._forStatement
					Exit Select
				Case 1
					syntaxNode = Me._statements
					Exit Select
				Case 2
					syntaxNode = Me._nextStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNextStatementCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax
			Return Me.NextStatement
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim forStatement As SyntaxNode
			Select Case i
				Case 0
					forStatement = Me.ForStatement
					Exit Select
				Case 1
					forStatement = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					forStatement = Me.NextStatement
					Exit Select
				Case Else
					forStatement = Nothing
					Exit Select
			End Select
			Return forStatement
		End Function

		Friend Overrides Function GetStatementsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Return Me.Statements
		End Function

		Public Function Update(ByVal forStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax
			Dim forBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax
			If (forStatement <> Me.ForStatement OrElse statements <> Me.Statements OrElse nextStatement <> Me.NextStatement) Then
				Dim forBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ForBlock(forStatement, statements, nextStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				forBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, forBlockSyntax1, forBlockSyntax1.WithAnnotations(annotations))
			Else
				forBlockSyntax = Me
			End If
			Return forBlockSyntax
		End Function

		Public Function WithForStatement(ByVal forStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax
			Return Me.Update(forStatement, Me.Statements, Me.NextStatement)
		End Function

		Public Shadows Function WithNextStatement(ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax
			Return Me.Update(Me.ForStatement, Me.Statements, nextStatement)
		End Function

		Friend Overrides Function WithNextStatementCore(ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax
			Return Me.WithNextStatement(nextStatement)
		End Function

		Public Shadows Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax
			Return Me.Update(Me.ForStatement, statements, Me.NextStatement)
		End Function

		Friend Overrides Function WithStatementsCore(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax
			Return Me.WithStatements(statements)
		End Function
	End Class
End Namespace