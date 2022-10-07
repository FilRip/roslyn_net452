Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class PropertyBlockContext
		Inherits DeclarationContext
		Private ReadOnly _isPropertyBlock As Boolean

		Private ReadOnly Property IsPropertyBlock As Boolean
			Get
				If (Me._isPropertyBlock) Then
					Return True
				End If
				Return MyBase.Statements.Count > 0
			End Get
		End Property

		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext, ByVal isPropertyBlock As Boolean)
			MyBase.New(SyntaxKind.PropertyBlock, statement, prevContext)
			Me._isPropertyBlock = isPropertyBlock
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim propertyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax = Nothing
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(endStmt, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(propertyStatementSyntax, endBlockStatementSyntax)
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax) = Me._statements.ToList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)()
			MyBase.FreeStatements()
			If (list.Any()) Then
				propertyStatementSyntax = PropertyBlockContext.ReportErrorIfHasInitializer(propertyStatementSyntax)
			End If
			Return MyBase.SyntaxFactory.PropertyBlock(propertyStatementSyntax, list, endBlockStatementSyntax)
		End Function

		Friend Overrides Function EndBlock(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			If (Me.IsPropertyBlock OrElse endStmt IsNot Nothing) Then
				blockContext = MyBase.EndBlock(endStmt)
			Else
				Dim beginStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax = DirectCast(MyBase.BeginStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax)
				If (beginStatement.ParameterList IsNot Nothing AndAlso beginStatement.ParameterList.Parameters.Count > 0) Then
					Dim kind As SyntaxKind = beginStatement.Kind
					Dim node As GreenNode = beginStatement.AttributeLists.Node
					Dim modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax) = beginStatement.Modifiers
					beginStatement = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax(kind, node, modifiers.Node, beginStatement.PropertyKeyword, beginStatement.Identifier, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)(beginStatement.ParameterList, ERRID.ERR_AutoPropertyCantHaveParams), beginStatement.AsClause, beginStatement.Initializer, beginStatement.ImplementsClause)
				End If
				Dim prevBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = MyBase.PrevBlock
				prevBlock.Add(beginStatement)
				blockContext = prevBlock
			End If
			Return blockContext
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim methodBlockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim kind As SyntaxKind = node.Kind
			If (CUShort(kind) - CUShort(SyntaxKind.GetAccessorBlock) <= CUShort(SyntaxKind.List)) Then
				MyBase.Add(node)
				methodBlockContext = Me
			ElseIf (kind = SyntaxKind.GetAccessorStatement) Then
				methodBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockContext(SyntaxKind.GetAccessorBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
			ElseIf (kind = SyntaxKind.SetAccessorStatement) Then
				methodBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockContext(SyntaxKind.SetAccessorBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
			Else
				Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = Me.EndBlock(Nothing)
				If (Me.IsPropertyBlock) Then
					node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InvInsideEndsProperty)
				End If
				methodBlockContext = blockContext.ProcessSyntax(node)
			End If
			Return methodBlockContext
		End Function

		Friend Shared Function ReportErrorIfHasInitializer(ByVal propertyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax
			If (propertyStatement.Initializer IsNot Nothing OrElse propertyStatement.AsClause IsNot Nothing AndAlso TypeOf propertyStatement.AsClause Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax) Then
				propertyStatement = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax)(propertyStatement, ERRID.ERR_InitializedExpandedProperty)
			End If
			Return propertyStatement
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			If (Not Me.KindEndsBlock(node.Kind)) Then
				Dim kind As SyntaxKind = node.Kind
				If (CUShort(kind) - CUShort(SyntaxKind.GetAccessorBlock) <= CUShort(SyntaxKind.List)) Then
					linkResult = MyBase.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax).[End].IsMissing)
				ElseIf (CUShort(kind) - CUShort(SyntaxKind.GetAccessorStatement) > CUShort(SyntaxKind.List)) Then
					newContext = Me
					linkResult = BlockContext.LinkResult.Crumble
				Else
					linkResult = MyBase.UseSyntax(node, newContext, False)
				End If
			Else
				linkResult = MyBase.UseSyntax(node, newContext, False)
			End If
			Return linkResult
		End Function
	End Class
End Namespace