Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Class NamespaceBlockContext
		Inherits DeclarationContext
		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(SyntaxKind.NamespaceBlock, statement, prevContext)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(kind, statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim beginStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax = DirectCast(MyBase.BeginStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax)
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(endStmt, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(beginStatement, endBlockStatementSyntax)
			Dim namespaceBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax = MyBase.SyntaxFactory.NamespaceBlock(beginStatement, MyBase.Body(), endBlockStatementSyntax)
			MyBase.FreeStatements()
			Return namespaceBlockSyntax
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As BlockContext
			Dim namespaceBlockContext As BlockContext
			Dim kind As SyntaxKind = node.Kind
			Select Case kind
				Case SyntaxKind.NamespaceBlock
				Case SyntaxKind.ModuleBlock
					MyBase.Add(node)
					namespaceBlockContext = Me
					Exit Select
				Case SyntaxKind.NamespaceStatement
					namespaceBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockContext(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
					Exit Select
				Case Else
					If (kind = SyntaxKind.ModuleStatement) Then
						namespaceBlockContext = New TypeBlockContext(SyntaxKind.ModuleBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
						Exit Select
					Else
						namespaceBlockContext = MyBase.ProcessSyntax(node)
						Exit Select
					End If
			End Select
			Return namespaceBlockContext
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			Dim kind As SyntaxKind = node.Kind
			If (kind > SyntaxKind.ModuleStatement) Then
				Select Case kind
					Case SyntaxKind.SubBlock
					Case SyntaxKind.FunctionBlock
					Case SyntaxKind.ConstructorBlock
					Case SyntaxKind.OperatorBlock
					Case SyntaxKind.PropertyBlock
					Case SyntaxKind.EventBlock
						newContext = Me
						linkResult = BlockContext.LinkResult.Crumble
						Return linkResult
					Case SyntaxKind.GetAccessorBlock
					Case SyntaxKind.SetAccessorBlock
					Case SyntaxKind.AddHandlerAccessorBlock
					Case SyntaxKind.RemoveHandlerAccessorBlock
					Case SyntaxKind.RaiseEventAccessorBlock
					Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.PropertyBlock
					Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock
					Case SyntaxKind.ParameterList
						linkResult = MyBase.TryLinkSyntax(node, newContext)
						Return linkResult
					Case SyntaxKind.SubStatement
					Case SyntaxKind.FunctionStatement
					Case SyntaxKind.SubNewStatement
						Exit Select
					Case Else
						If (CUShort(kind) - CUShort(SyntaxKind.EventStatement) <= CUShort(SyntaxKind.EmptyStatement) OrElse kind = SyntaxKind.FieldDeclaration) Then
							Exit Select
						Else
							linkResult = MyBase.TryLinkSyntax(node, newContext)
							Return linkResult
						End If
				End Select
			Else
				Select Case kind
					Case SyntaxKind.NamespaceBlock
						linkResult = MyBase.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax).EndNamespaceStatement.IsMissing)
						Return linkResult
					Case SyntaxKind.NamespaceStatement
						Exit Select
					Case SyntaxKind.ModuleBlock
						linkResult = MyBase.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockSyntax).EndBlockStatement.IsMissing)
						Return linkResult
					Case Else
						If (kind = SyntaxKind.ModuleStatement) Then
							Exit Select
						End If
						linkResult = MyBase.TryLinkSyntax(node, newContext)
						Return linkResult
				End Select
			End If
			linkResult = MyBase.UseSyntax(node, newContext, False)
			Return linkResult
		End Function
	End Class
End Namespace