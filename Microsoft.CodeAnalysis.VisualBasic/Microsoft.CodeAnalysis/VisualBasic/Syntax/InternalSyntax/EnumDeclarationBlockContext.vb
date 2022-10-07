Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class EnumDeclarationBlockContext
		Inherits DeclarationContext
		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(SyntaxKind.EnumBlock, statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim enumStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax = Nothing
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(endStmt, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(enumStatementSyntax, endBlockStatementSyntax)
			Dim enumBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax = MyBase.SyntaxFactory.EnumBlock(enumStatementSyntax, MyBase.Body(), endBlockStatementSyntax)
			MyBase.FreeStatements()
			Return enumBlockSyntax
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			If (node.Kind <> SyntaxKind.EnumMemberDeclaration) Then
				If (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.IsExecutableStatementOrItsPart(node)) Then
					MyBase.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InvInsideEnum))
					blockContext = Me
					Return blockContext
				End If
				blockContext = Me.EndBlock(Nothing).ProcessSyntax(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InvInsideEndsEnum))
				Return blockContext
			Else
				MyBase.Add(node)
			End If
			blockContext = Me
			Return blockContext
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			Dim kind As SyntaxKind = node.Kind
			If (kind <> SyntaxKind.NamespaceBlock AndAlso CUShort(kind) - CUShort(SyntaxKind.ModuleBlock) > 4) Then
				Select Case kind
					Case SyntaxKind.EnumMemberDeclaration
						linkResult = MyBase.UseSyntax(node, newContext, False)
						Return linkResult
					Case SyntaxKind.SubBlock
					Case SyntaxKind.FunctionBlock
					Case SyntaxKind.ConstructorBlock
					Case SyntaxKind.OperatorBlock
					Case SyntaxKind.PropertyBlock
					Case SyntaxKind.EventBlock
						Exit Select
					Case SyntaxKind.GetAccessorBlock
					Case SyntaxKind.SetAccessorBlock
					Case SyntaxKind.AddHandlerAccessorBlock
					Case SyntaxKind.RemoveHandlerAccessorBlock
					Case SyntaxKind.RaiseEventAccessorBlock
						linkResult = MyBase.TryLinkSyntax(node, newContext)
						Return linkResult
					Case Else
						linkResult = MyBase.TryLinkSyntax(node, newContext)
						Return linkResult
				End Select
			End If
			newContext = Me
			linkResult = BlockContext.LinkResult.Crumble
			Return linkResult
		End Function
	End Class
End Namespace