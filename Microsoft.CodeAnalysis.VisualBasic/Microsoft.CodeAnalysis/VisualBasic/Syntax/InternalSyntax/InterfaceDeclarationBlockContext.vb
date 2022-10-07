Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class InterfaceDeclarationBlockContext
		Inherits TypeBlockContext
		Friend Sub New(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(SyntaxKind.InterfaceBlock, statement, prevContext)
		End Sub

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As BlockContext
			Dim typeBlockContext As BlockContext
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind
			While True
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me._state
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None) Then
					If (node.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement) Then
						Me._state = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement
					Else
						Me._state = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement
					End If
				ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement) Then
					If (kind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement) Then
						If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement) Then
							GoTo Label1
						End If
						If (CUShort(kind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement)) Then
							MyBase.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InvInsideInterface))
						Else
							Select Case kind
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement
									typeBlockContext = Me.EndBlock(Nothing).ProcessSyntax(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InvInsideEndsInterface))
									Return typeBlockContext
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock
									MyBase.Add(node)
									Exit Select
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement
									typeBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockContext(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
									Return typeBlockContext
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement
									typeBlockContext = New InterfaceDeclarationBlockContext(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
									Return typeBlockContext
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement
									typeBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockContext(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
									Return typeBlockContext
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement
									typeBlockContext = New EnumDeclarationBlockContext(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
									Return typeBlockContext
								Case Else
									GoTo Label0
							End Select
						End If
					ElseIf (kind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember) Then
						Select Case kind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement
								GoTo Label1
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
								MyBase.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_NewInInterface))
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
							Case 100
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
								GoTo Label0
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement
								Dim eventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)
								If (eventStatementSyntax.CustomKeyword IsNot Nothing) Then
									eventStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)(eventStatementSyntax, ERRID.ERR_CustomEventInvInInterface)
								End If
								MyBase.Add(eventStatementSyntax)
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement
								node = PropertyBlockContext.ReportErrorIfHasInitializer(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax))
								MyBase.Add(node)
								Exit Select
							Case Else
								If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember) Then
									node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InterfaceMemberSyntax)
									MyBase.Add(node)
									Exit Select
								Else
									GoTo Label0
								End If
						End Select
					ElseIf (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration) Then
						MyBase.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InterfaceMemberSyntax))
					Else
						If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement) Then
							GoTo Label0
						End If
						node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InvOutsideProc)
						MyBase.Add(node)
					End If
				Label4:
					typeBlockContext = Me
					Exit While
				ElseIf (node.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement) Then
					Me._inheritsDecls = MyBase.BaseDeclarations(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)()
					Me._state = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement
				Else
					MyBase.Add(node)
					typeBlockContext = Me
					Exit While
				End If
			End While
			Return typeBlockContext
		Label0:
			MyBase.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InvInsideInterface))
			GoTo Label4
		Label1:
			MyBase.Add(node)
			GoTo Label4
		End Function

		Friend Overrides Function RecoverFromMismatchedEnd(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			blockContext = If(CUShort(statement.Kind) - CUShort(SyntaxKind.EndSubStatement) > CUShort(SyntaxKind.EndStructureStatement), MyBase.RecoverFromMismatchedEnd(statement), Me.ProcessSyntax(statement))
			Return blockContext
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			If (Not Me.KindEndsBlock(node.Kind)) Then
				Dim kind As SyntaxKind = node.Kind
				If (CUShort(kind) - CUShort(SyntaxKind.EndSubStatement) > CUShort(SyntaxKind.EndStructureStatement)) Then
					Select Case kind
						Case SyntaxKind.StructureBlock
						Case SyntaxKind.InterfaceBlock
						Case SyntaxKind.ClassBlock
							linkResult = MyBase.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockSyntax).EndBlockStatement.IsMissing)
							Return linkResult
						Case SyntaxKind.EnumBlock
							linkResult = MyBase.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax).EndEnumStatement.IsMissing)
							Return linkResult
						Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.EnumBlock
						Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NamespaceBlock
							newContext = Me
							linkResult = BlockContext.LinkResult.Crumble
							Return linkResult
						Case SyntaxKind.InheritsStatement
							Exit Select
						Case Else
							Select Case kind
								Case SyntaxKind.SubStatement
								Case SyntaxKind.FunctionStatement
								Case SyntaxKind.SubNewStatement
								Case SyntaxKind.DelegateSubStatement
								Case SyntaxKind.DelegateFunctionStatement
								Case SyntaxKind.EventStatement
								Case SyntaxKind.PropertyStatement
									Exit Select
								Case SyntaxKind.DeclareSubStatement
								Case SyntaxKind.DeclareFunctionStatement
								Case 100
								Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement
								Case SyntaxKind.OperatorStatement
									newContext = Me
									linkResult = BlockContext.LinkResult.Crumble
									Return linkResult
								Case Else
									newContext = Me
									linkResult = BlockContext.LinkResult.Crumble
									Return linkResult
							End Select

					End Select
				End If
				linkResult = MyBase.UseSyntax(node, newContext, False)
			Else
				linkResult = MyBase.UseSyntax(node, newContext, False)
			End If
			Return linkResult
		End Function
	End Class
End Namespace