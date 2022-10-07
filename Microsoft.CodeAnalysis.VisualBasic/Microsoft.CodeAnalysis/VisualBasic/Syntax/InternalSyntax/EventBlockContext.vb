Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class EventBlockContext
		Inherits DeclarationContext
		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(SyntaxKind.EventBlock, statement, prevContext)
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim eventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax = Nothing
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(eventStatementSyntax, endBlockStatementSyntax)
			Dim eventBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax = MyBase.SyntaxFactory.EventBlock(eventStatementSyntax, MyBase.Body(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)(), endBlockStatementSyntax)
			MyBase.FreeStatements()
			Return eventBlockSyntax
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As BlockContext
			Dim methodBlockContext As BlockContext
			Dim kind As SyntaxKind = node.Kind
			If (CUShort(kind) - CUShort(SyntaxKind.AddHandlerAccessorBlock) <= CUShort(SyntaxKind.EmptyStatement)) Then
				MyBase.Add(node)
				methodBlockContext = Me
			Else
				Select Case kind
					Case SyntaxKind.AddHandlerAccessorStatement
						methodBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockContext(SyntaxKind.AddHandlerAccessorBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
						Exit Select
					Case SyntaxKind.RemoveHandlerAccessorStatement
						methodBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockContext(SyntaxKind.RemoveHandlerAccessorBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
						Exit Select
					Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement
					Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement
					Label0:
						methodBlockContext = Me.EndBlock(Nothing).ProcessSyntax(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InvInsideEndsEvent))
						Exit Select
					Case SyntaxKind.RaiseEventAccessorStatement
						methodBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockContext(SyntaxKind.RaiseEventAccessorBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
						Exit Select
					Case Else
						GoTo Label0
				End Select
			End If
			Return methodBlockContext
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			If (Not Me.KindEndsBlock(node.Kind)) Then
				Dim kind As SyntaxKind = node.Kind
				If (CUShort(kind) - CUShort(SyntaxKind.AddHandlerAccessorBlock) <= CUShort(SyntaxKind.EmptyStatement)) Then
					linkResult = MyBase.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax).[End].IsMissing)
				Else
					linkResult = If(CUShort(kind) - CUShort(SyntaxKind.AddHandlerAccessorStatement) <= CUShort(SyntaxKind.List) OrElse kind = SyntaxKind.RaiseEventAccessorStatement, MyBase.UseSyntax(node, newContext, False), MyBase.TryUseStatement(node, newContext))
				End If
			Else
				linkResult = MyBase.UseSyntax(node, newContext, False)
			End If
			Return linkResult
		End Function
	End Class
End Namespace