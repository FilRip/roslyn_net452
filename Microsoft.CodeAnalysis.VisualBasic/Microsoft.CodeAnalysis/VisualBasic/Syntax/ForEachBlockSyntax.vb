Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ForEachBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax
		Friend _forEachStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax

		Public ReadOnly Property ForEachStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax)(Me._forEachStatement)
			End Get
		End Property

		<EditorBrowsable(EditorBrowsableState.Never)>
		Public Overrides ReadOnly Property ForOrForEachStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachStatementSyntax
			Get
				Return Me.ForEachStatement
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

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal forEachStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax, ByVal statements As SyntaxNode, ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax(kind, errors, annotations, DirectCast(forEachStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing), If(nextStatement IsNot Nothing, DirectCast(nextStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitForEachBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitForEachBlock(Me)
		End Sub

		Public Shadows Function AddNextStatementControlVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax
			Return Me.WithNextStatement(If(Me.NextStatement IsNot Nothing, Me.NextStatement, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.NextStatement()).AddControlVariables(items))
		End Function

		Friend Overrides Function AddNextStatementControlVariablesCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax
			Return Me.AddNextStatementControlVariables(items)
		End Function

		Public Shadows Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function AddStatementsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax
			Return Me.AddStatements(items)
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._forEachStatement
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
			Dim forEachStatement As SyntaxNode
			Select Case i
				Case 0
					forEachStatement = Me.ForEachStatement
					Exit Select
				Case 1
					forEachStatement = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					forEachStatement = Me.NextStatement
					Exit Select
				Case Else
					forEachStatement = Nothing
					Exit Select
			End Select
			Return forEachStatement
		End Function

		Friend Overrides Function GetStatementsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Return Me.Statements
		End Function

		Public Function Update(ByVal forEachStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax
			Dim forEachBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax
			If (forEachStatement <> Me.ForEachStatement OrElse statements <> Me.Statements OrElse nextStatement <> Me.NextStatement) Then
				Dim forEachBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ForEachBlock(forEachStatement, statements, nextStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				forEachBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, forEachBlockSyntax1, forEachBlockSyntax1.WithAnnotations(annotations))
			Else
				forEachBlockSyntax = Me
			End If
			Return forEachBlockSyntax
		End Function

		Public Function WithForEachStatement(ByVal forEachStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax
			Return Me.Update(forEachStatement, Me.Statements, Me.NextStatement)
		End Function

		Public Shadows Function WithNextStatement(ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax
			Return Me.Update(Me.ForEachStatement, Me.Statements, nextStatement)
		End Function

		Friend Overrides Function WithNextStatementCore(ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax
			Return Me.WithNextStatement(nextStatement)
		End Function

		Public Shadows Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax
			Return Me.Update(Me.ForEachStatement, statements, Me.NextStatement)
		End Function

		Friend Overrides Function WithStatementsCore(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax
			Return Me.WithStatements(statements)
		End Function
	End Class
End Namespace