Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class ExecutableStatementContext
		Inherits DeclarationContext
		Friend Overrides ReadOnly Property IsSingleLine As Boolean
			Get
				Return MyBase.PrevBlock.IsSingleLine
			End Get
		End Property

		Friend Sub New(ByVal contextKind As SyntaxKind, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(contextKind, statement, prevContext)
		End Sub

		Friend NotOverridable Overrides Function Parse() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Return MyBase.Parser.ParseStatementInMethodBody()
		End Function

		Friend Overrides Function ProcessStatementTerminator(ByVal lambdaContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim kind As SyntaxKind = MyBase.Parser.CurrentToken.Kind
			Dim isSingleLine As Boolean = Me.IsSingleLine
			If (Not isSingleLine OrElse CUShort(kind) - CUShort(SyntaxKind.StatementTerminatorToken) > CUShort(SyntaxKind.List)) Then
				Dim flag As Boolean = False
				If (kind = SyntaxKind.ColonToken) Then
					flag = Not Me.IsSingleLine
				ElseIf (kind = SyntaxKind.StatementTerminatorToken) Then
					flag = True
				End If
				If (lambdaContext Is Nothing OrElse MyBase.Parser.IsNextStatementInsideLambda(Me, lambdaContext, flag)) Then
					MyBase.Parser.ConsumeStatementTerminator(isSingleLine)
					blockContext = Me
				Else
					blockContext = Me.EndLambda()
				End If
			Else
				blockContext = Me.EndBlock(Nothing).ProcessStatementTerminator(lambdaContext)
			End If
			Return blockContext
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim func As Func(Of SyntaxKind, Boolean)
			Dim func1 As Func(Of SyntaxKind, Boolean)
			If (Not Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.IsDeclarationStatement(node.Kind)) Then
				Dim kind As SyntaxKind = node.Kind
				If (CUShort(kind) - CUShort(SyntaxKind.OptionStatement) <= CUShort(SyntaxKind.List) OrElse CUShort(kind) - CUShort(SyntaxKind.InheritsStatement) <= CUShort(SyntaxKind.List)) Then
					If (ExecutableStatementContext._Closure$__.$I2-1 Is Nothing) Then
						func = Function(s As SyntaxKind)
							If (SyntaxFacts.IsMethodBlock(s) OrElse s = SyntaxKind.ConstructorBlock OrElse s = SyntaxKind.OperatorBlock OrElse SyntaxFacts.IsAccessorBlock(s) OrElse SyntaxFacts.IsMultiLineLambdaExpression(s)) Then
								Return True
							End If
							Return SyntaxFacts.IsSingleLineLambdaExpression(s)
						End Function
						ExecutableStatementContext._Closure$__.$I2-1 = func
					Else
						func = ExecutableStatementContext._Closure$__.$I2-1
					End If
					If (Me.FindNearest(func) Is Nothing) Then
						node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InvInsideBlock, New [Object]() { SyntaxFacts.GetBlockName(MyBase.BlockKind) })
					Else
						node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InvInsideProc)
					End If
					MyBase.Add(node)
					blockContext = Me
				Else
					blockContext = If(MyBase.TryProcessExecutableStatement(node), MyBase.ProcessSyntax(node))
				End If
			Else
				If (ExecutableStatementContext._Closure$__.$I2-0 Is Nothing) Then
					func1 = Function(s As SyntaxKind)
						If (SyntaxFacts.IsMethodBlock(s) OrElse s = SyntaxKind.ConstructorBlock OrElse s = SyntaxKind.OperatorBlock) Then
							Return True
						End If
						Return SyntaxFacts.IsAccessorBlock(s)
					End Function
					ExecutableStatementContext._Closure$__.$I2-0 = func1
				Else
					func1 = ExecutableStatementContext._Closure$__.$I2-0
				End If
				Dim blockContext1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = Me.FindNearest(func1)
				If (blockContext1 Is Nothing) Then
					node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InvInsideBlock, New [Object]() { SyntaxFacts.GetBlockName(MyBase.BlockKind) })
					blockContext = MyBase.ProcessSyntax(node)
				Else
					Dim prevBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = blockContext1.PrevBlock
					Me.RecoverFromMissingEnd(prevBlock)
					blockContext = prevBlock.ProcessSyntax(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InvInsideEndsProc))
				End If
			End If
			Return blockContext
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			Dim kind As SyntaxKind = node.Kind
			If (kind <= SyntaxKind.SingleLineElseClause) Then
				Select Case kind
					Case SyntaxKind.OptionStatement
					Case SyntaxKind.ImportsStatement
					Case SyntaxKind.NamespaceStatement
					Case SyntaxKind.InheritsStatement
					Case SyntaxKind.ImplementsStatement
						linkResult = MyBase.UseSyntax(node, newContext, False)
						Exit Select
					Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement
					Case SyntaxKind.SimpleImportsClause
					Case SyntaxKind.XmlNamespaceImportsClause
					Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause
					Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause
					Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.EnumBlock
					Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NamespaceBlock
					Case 64
					Case 65
					Case SyntaxKind.TypeParameterList
					Case SyntaxKind.TypeParameter
					Case 68
					Case SyntaxKind.List Or SyntaxKind.EndIfStatement
					Case SyntaxKind.TypeParameterSingleConstraintClause
					Case SyntaxKind.TypeParameterMultipleConstraintClause
					Case SyntaxKind.NewConstraint
					Case SyntaxKind.ClassConstraint
					Case SyntaxKind.StructureConstraint
					Case SyntaxKind.TypeConstraint
					Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.NewConstraint
					Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint
					Case SyntaxKind.EnumMemberDeclaration
					Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.PropertyBlock
					Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock
					Case SyntaxKind.ParameterList
					Case 100
					Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement
					Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement
					Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement
					Case SyntaxKind.ImplementsClause
					Case SyntaxKind.HandlesClause
					Case SyntaxKind.KeywordEventContainer
					Case SyntaxKind.WithEventsEventContainer
					Case SyntaxKind.WithEventsPropertyEventContainer
					Case SyntaxKind.HandlesClauseItem
					Case SyntaxKind.IncompleteMember
						linkResult = MyBase.TryLinkStatement(node, newContext)
						Return linkResult
					Case SyntaxKind.NamespaceBlock
					Case SyntaxKind.ModuleBlock
					Case SyntaxKind.StructureBlock
					Case SyntaxKind.InterfaceBlock
					Case SyntaxKind.ClassBlock
					Case SyntaxKind.EnumBlock
					Case SyntaxKind.SubBlock
					Case SyntaxKind.FunctionBlock
					Case SyntaxKind.ConstructorBlock
					Case SyntaxKind.OperatorBlock
					Case SyntaxKind.GetAccessorBlock
					Case SyntaxKind.SetAccessorBlock
					Case SyntaxKind.AddHandlerAccessorBlock
					Case SyntaxKind.RemoveHandlerAccessorBlock
					Case SyntaxKind.RaiseEventAccessorBlock
					Case SyntaxKind.PropertyBlock
					Case SyntaxKind.EventBlock
					Case SyntaxKind.FieldDeclaration
						newContext = Me
						linkResult = BlockContext.LinkResult.Crumble
						Return linkResult
					Case SyntaxKind.ModuleStatement
					Case SyntaxKind.StructureStatement
					Case SyntaxKind.InterfaceStatement
					Case SyntaxKind.ClassStatement
						If (DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax).Modifiers.Any()) Then
							newContext = Me
							linkResult = BlockContext.LinkResult.NotUsed
							Exit Select
						Else
							linkResult = MyBase.UseSyntax(node, newContext, False)
							Exit Select
						End If
					Case SyntaxKind.EnumStatement
						If (DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax).Modifiers.Any()) Then
							newContext = Me
							linkResult = BlockContext.LinkResult.NotUsed
							Exit Select
						Else
							linkResult = MyBase.UseSyntax(node, newContext, False)
							Exit Select
						End If
					Case SyntaxKind.SubStatement
					Case SyntaxKind.FunctionStatement
					Case SyntaxKind.EventStatement
					Case SyntaxKind.OperatorStatement
					Case SyntaxKind.PropertyStatement
						If (DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax).Modifiers.Any(New SyntaxKind() { SyntaxKind.DimKeyword, SyntaxKind.ConstKeyword })) Then
							newContext = Me
							linkResult = BlockContext.LinkResult.NotUsed
							Exit Select
						Else
							linkResult = MyBase.UseSyntax(node, newContext, False)
							Exit Select
						End If
					Case SyntaxKind.SubNewStatement
					Case SyntaxKind.DeclareSubStatement
					Case SyntaxKind.DeclareFunctionStatement
					Case SyntaxKind.DelegateSubStatement
					Case SyntaxKind.DelegateFunctionStatement
						If (DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax).Modifiers.Any()) Then
							newContext = Me
							linkResult = BlockContext.LinkResult.NotUsed
							Exit Select
						Else
							linkResult = MyBase.UseSyntax(node, newContext, False)
							Exit Select
						End If
					Case SyntaxKind.GetAccessorStatement
					Case SyntaxKind.SetAccessorStatement
					Case SyntaxKind.AddHandlerAccessorStatement
					Case SyntaxKind.RemoveHandlerAccessorStatement
					Case SyntaxKind.RaiseEventAccessorStatement
						newContext = Me
						linkResult = BlockContext.LinkResult.NotUsed
						Exit Select
					Case Else
						If (kind = SyntaxKind.AttributeList OrElse kind = SyntaxKind.SingleLineElseClause) Then
							newContext = Me
							linkResult = BlockContext.LinkResult.Crumble
							Return linkResult
						End If
						linkResult = MyBase.TryLinkStatement(node, newContext)
						Return linkResult
				End Select
			ElseIf (kind > SyntaxKind.FinallyBlock) Then
				If (kind = SyntaxKind.CaseBlock OrElse kind = SyntaxKind.CaseElseBlock) Then
					newContext = Me
					linkResult = BlockContext.LinkResult.Crumble
					Return linkResult
				End If
				linkResult = MyBase.TryLinkStatement(node, newContext)
				Return linkResult
			Else
				If (CUShort(kind) - CUShort(SyntaxKind.ElseIfBlock) <= CUShort(SyntaxKind.List) OrElse CUShort(kind) - CUShort(SyntaxKind.CatchBlock) <= CUShort(SyntaxKind.List)) Then
					newContext = Me
					linkResult = BlockContext.LinkResult.Crumble
					Return linkResult
				End If
				linkResult = MyBase.TryLinkStatement(node, newContext)
				Return linkResult
			End If
			Return linkResult
		End Function
	End Class
End Namespace