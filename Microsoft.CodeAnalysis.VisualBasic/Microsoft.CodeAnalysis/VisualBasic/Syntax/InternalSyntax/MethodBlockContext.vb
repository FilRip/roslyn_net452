Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Class MethodBlockContext
		Inherits ExecutableStatementContext
		Friend Sub New(ByVal contextKind As SyntaxKind, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(contextKind, statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(endStmt, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			Select Case MyBase.BlockKind
				Case SyntaxKind.SubBlock
				Case SyntaxKind.FunctionBlock
					Dim methodStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax = Nothing
					MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(methodStatementSyntax, endBlockStatementSyntax)
					visualBasicSyntaxNode = MyBase.SyntaxFactory.MethodBlock(MyBase.BlockKind, methodStatementSyntax, MyBase.BodyWithWeakChildren(), endBlockStatementSyntax)
					Exit Select
				Case SyntaxKind.ConstructorBlock
					Dim subNewStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax = Nothing
					MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(subNewStatementSyntax, endBlockStatementSyntax)
					visualBasicSyntaxNode = MyBase.SyntaxFactory.ConstructorBlock(subNewStatementSyntax, MyBase.BodyWithWeakChildren(), endBlockStatementSyntax)
					Exit Select
				Case SyntaxKind.OperatorBlock
					Dim operatorStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax = Nothing
					MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(operatorStatementSyntax, endBlockStatementSyntax)
					visualBasicSyntaxNode = MyBase.SyntaxFactory.OperatorBlock(operatorStatementSyntax, MyBase.BodyWithWeakChildren(), endBlockStatementSyntax)
					Exit Select
				Case SyntaxKind.GetAccessorBlock
				Case SyntaxKind.SetAccessorBlock
				Case SyntaxKind.AddHandlerAccessorBlock
				Case SyntaxKind.RemoveHandlerAccessorBlock
				Case SyntaxKind.RaiseEventAccessorBlock
					Dim accessorStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax = Nothing
					MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(accessorStatementSyntax, endBlockStatementSyntax)
					visualBasicSyntaxNode = MyBase.SyntaxFactory.AccessorBlock(MyBase.BlockKind, accessorStatementSyntax, MyBase.BodyWithWeakChildren(), endBlockStatementSyntax)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(MyBase.BlockKind)
			End Select
			MyBase.FreeStatements()
			Return visualBasicSyntaxNode
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As BlockContext
			If (MyBase.Statements.Count = 0) Then
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.BeginStatement.LastTriviaIfAny()
				If (visualBasicSyntaxNode IsNot Nothing AndAlso visualBasicSyntaxNode.Kind = SyntaxKind.ColonTrivia) Then
					node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_MethodBodyNotAtLineStart)
				End If
			End If
			If (node.Kind = SyntaxKind.ExitPropertyStatement AndAlso MyBase.BlockKind <> SyntaxKind.GetAccessorBlock AndAlso MyBase.BlockKind <> SyntaxKind.SetAccessorBlock) Then
				node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_ExitPropNot)
			End If
			Return MyBase.ProcessSyntax(node)
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			linkResult = If(node.MatchesFactoryContext(Me), MyBase.TryLinkSyntax(node, newContext), BlockContext.LinkResult.NotUsed)
			Return linkResult
		End Function
	End Class
End Namespace