Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class ForOrForEachBlockSyntax
		Inherits ExecutableStatementSyntax
		Friend _statements As SyntaxNode

		Friend _nextStatement As NextStatementSyntax

		Public MustOverride ReadOnly Property ForOrForEachStatement As ForOrForEachStatementSyntax

		Public ReadOnly Property NextStatement As NextStatementSyntax
			Get
				Return Me.GetNextStatementCore()
			End Get
		End Property

		Public ReadOnly Property Statements As SyntaxList(Of StatementSyntax)
			Get
				Return Me.GetStatementsCore()
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Public Function AddNextStatementControlVariables(ByVal ParamArray items As ExpressionSyntax()) As ForOrForEachBlockSyntax
			Return Me.AddNextStatementControlVariablesCore(items)
		End Function

		Friend MustOverride Function AddNextStatementControlVariablesCore(ByVal ParamArray items As ExpressionSyntax()) As ForOrForEachBlockSyntax

		Public Function AddStatements(ByVal ParamArray items As StatementSyntax()) As ForOrForEachBlockSyntax
			Return Me.AddStatementsCore(items)
		End Function

		Friend MustOverride Function AddStatementsCore(ByVal ParamArray items As StatementSyntax()) As ForOrForEachBlockSyntax

		Friend Overridable Function GetNextStatementCore() As NextStatementSyntax
			Return MyBase.GetRed(Of NextStatementSyntax)(Me._nextStatement, 1)
		End Function

		Friend Overridable Function GetStatementsCore() As SyntaxList(Of StatementSyntax)
			Return New SyntaxList(Of StatementSyntax)(MyBase.GetRedAtZero(Me._statements))
		End Function

		Public Function WithNextStatement(ByVal nextStatement As NextStatementSyntax) As ForOrForEachBlockSyntax
			Return Me.WithNextStatementCore(nextStatement)
		End Function

		Friend MustOverride Function WithNextStatementCore(ByVal nextStatement As NextStatementSyntax) As ForOrForEachBlockSyntax

		Public Function WithStatements(ByVal statements As SyntaxList(Of StatementSyntax)) As ForOrForEachBlockSyntax
			Return Me.WithStatementsCore(statements)
		End Function

		Friend MustOverride Function WithStatementsCore(ByVal statements As SyntaxList(Of StatementSyntax)) As ForOrForEachBlockSyntax
	End Class
End Namespace