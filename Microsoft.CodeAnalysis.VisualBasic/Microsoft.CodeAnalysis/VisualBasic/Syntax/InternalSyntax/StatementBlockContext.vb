Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class StatementBlockContext
		Inherits ExecutableStatementContext
		Friend Sub New(ByVal kind As SyntaxKind, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(kind, statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			Select Case MyBase.BlockKind
				Case SyntaxKind.WhileBlock
					Dim whileStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax = Nothing
					MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(whileStatementSyntax, endBlockStatementSyntax)
					visualBasicSyntaxNode = MyBase.SyntaxFactory.WhileBlock(whileStatementSyntax, MyBase.Body(), endBlockStatementSyntax)
					Exit Select
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock
					Throw ExceptionUtilities.UnexpectedValue(MyBase.BlockKind)
				Case SyntaxKind.UsingBlock
					Dim usingStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax = Nothing
					MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(usingStatementSyntax, endBlockStatementSyntax)
					visualBasicSyntaxNode = MyBase.SyntaxFactory.UsingBlock(usingStatementSyntax, MyBase.Body(), endBlockStatementSyntax)
					Exit Select
				Case SyntaxKind.SyncLockBlock
					Dim syncLockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax = Nothing
					MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(syncLockStatementSyntax, endBlockStatementSyntax)
					visualBasicSyntaxNode = MyBase.SyntaxFactory.SyncLockBlock(syncLockStatementSyntax, MyBase.Body(), endBlockStatementSyntax)
					Exit Select
				Case SyntaxKind.WithBlock
					Dim withStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax = Nothing
					MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(withStatementSyntax, endBlockStatementSyntax)
					visualBasicSyntaxNode = MyBase.SyntaxFactory.WithBlock(withStatementSyntax, MyBase.Body(), endBlockStatementSyntax)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(MyBase.BlockKind)
			End Select
			MyBase.FreeStatements()
			Return visualBasicSyntaxNode
		End Function
	End Class
End Namespace