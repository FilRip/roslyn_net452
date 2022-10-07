Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class LambdaContext
		Inherits MethodBlockContext
		Friend Overrides ReadOnly Property IsLambda As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property IsSingleLine As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(If(statement.Kind = SyntaxKind.FunctionLambdaHeader, SyntaxKind.MultiLineFunctionLambdaExpression, SyntaxKind.MultiLineSubLambdaExpression), statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim lambdaHeaderSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax = Nothing
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = MyBase.Body()
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(endStmt, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(lambdaHeaderSyntax, endBlockStatementSyntax)
			Dim multiLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax = MyBase.SyntaxFactory.MultiLineLambdaExpression(MyBase.BlockKind, lambdaHeaderSyntax, syntaxList, endBlockStatementSyntax)
			MyBase.FreeStatements()
			Return multiLineLambdaExpressionSyntax
		End Function

		Friend Overrides Function EndBlock(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext
			Return MyBase.PrevBlock
		End Function
	End Class
End Namespace