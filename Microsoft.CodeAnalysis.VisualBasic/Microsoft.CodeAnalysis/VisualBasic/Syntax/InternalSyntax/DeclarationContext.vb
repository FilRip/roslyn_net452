Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class DeclarationContext
		Inherits BlockContext
		Friend Overrides ReadOnly Property IsSingleLine As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal context As BlockContext)
			MyBase.New(kind, statement, context)
		End Sub

		Friend Overrides Function EndBlock(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.CreateBlockSyntax(endStmt)
			Return MyBase.PrevBlock.ProcessSyntax(visualBasicSyntaxNode)
		End Function

		Friend Overrides Function Parse() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Return MyBase.Parser.ParseDeclarationStatement()
		End Function

		Friend Overrides Function ProcessStatementTerminator(ByVal lambdaContext As BlockContext) As BlockContext
			MyBase.Parser.ConsumeStatementTerminator(False)
			Return Me
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As BlockContext
			Dim typeBlockContext As BlockContext
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind
			If (kind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement) Then
				If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement) Then
					Select Case kind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement
							node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_OptionStmtWrongOrder)
							MyBase.Add(node)
							typeBlockContext = Me
							Return typeBlockContext
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement
							node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_ImportsMustBeFirst)
							MyBase.Add(node)
							typeBlockContext = Me
							Return typeBlockContext
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock
						Case 64
						Case 65
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter
						Case 68
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList
						Case 100
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClauseItem
							If (Not SyntaxFacts.IsEndBlockLoopOrNextStatement(node.Kind)) Then
								node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_ExecutableAsDeclaration)
							End If
							MyBase.Add(node)
							typeBlockContext = Me
							Return typeBlockContext
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock
							MyBase.Add(node)
							typeBlockContext = Me
							Return typeBlockContext
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement
							Dim flag As Boolean = True
							Dim diagnostics As DiagnosticInfo() = node.GetDiagnostics()
							If (diagnostics IsNot Nothing) Then
								Dim diagnosticInfoArray As DiagnosticInfo() = diagnostics
								Dim num As Integer = 0
								While num < CInt(diagnosticInfoArray.Length)
									Dim code As Integer = diagnosticInfoArray(num).Code
									If (code = 30289 OrElse code = 30618) Then
										flag = False
										Exit While
									Else
										num = num + 1
									End If
								End While
							End If
							If (flag) Then
								node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_NamespaceNotAtNamespace)
							End If
							Dim prevBlock As BlockContext = MyBase.PrevBlock
							Me.RecoverFromMissingEnd(prevBlock)
							typeBlockContext = prevBlock.ProcessSyntax(node)
							Return typeBlockContext
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement
							Dim beginStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = MyBase.BeginStatement
							If (beginStatement Is Nothing OrElse beginStatement.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement) Then
								node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InheritsStmtWrongOrder)
							Else
								node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_BadInterfaceOrderOnInherits)
							End If
							MyBase.Add(node)
							typeBlockContext = Me
							Return typeBlockContext
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement
							node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_ImplementsStmtWrongOrder)
							MyBase.Add(node)
							typeBlockContext = Me
							Return typeBlockContext
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement
							node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_ModuleNotAtNamespace)
							typeBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockContext(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
							Return typeBlockContext
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
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration
							GoTo Label4
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement
							Dim eventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)
							If (eventStatementSyntax.CustomKeyword IsNot Nothing) Then
								If (Not eventStatementSyntax.AsClause.AsKeyword.IsMissing) Then
									typeBlockContext = New EventBlockContext(eventStatementSyntax, Me)
									Return typeBlockContext
								Else
									node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)(eventStatementSyntax, ERRID.ERR_CustomEventRequiresAs)
								End If
							End If
							MyBase.Add(node)
							typeBlockContext = Me
							Return typeBlockContext
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
							If (Not MyBase.Parser.IsFirstStatementOnLine(node.GetFirstToken())) Then
								node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_MethodMustBeFirstStatementOnLine)
							End If
							If (MyBase.BlockKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock) Then
								node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_OperatorDeclaredInModule)
							End If
							typeBlockContext = New MethodBlockContext(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
							Return typeBlockContext
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement
							Dim propertyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax)
							Dim modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax) = propertyStatementSyntax.Modifiers
							Dim flag1 As Boolean = False
							If (modifiers.Any()) Then
								If (Not modifiers.Any(505)) Then
									flag1 = modifiers.Any(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DefaultKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword })
								Else
									node = PropertyBlockContext.ReportErrorIfHasInitializer(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax))
									MyBase.Add(node)
									typeBlockContext = Me
									Return typeBlockContext
								End If
							End If
							typeBlockContext = New PropertyBlockContext(propertyStatementSyntax, Me, flag1)
							Return typeBlockContext
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement
							node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_ExpectedDeclaration)
							MyBase.Add(node)
							typeBlockContext = Me
							Return typeBlockContext
						Case Else
							If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement) Then
								node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_AttributeStmtWrongOrder)
								MyBase.Add(node)
								typeBlockContext = Me
								Return typeBlockContext
							Else
								If (Not SyntaxFacts.IsEndBlockLoopOrNextStatement(node.Kind)) Then
									node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_ExecutableAsDeclaration)
								End If
								MyBase.Add(node)
								typeBlockContext = Me
								Return typeBlockContext
							End If
					End Select
					If (Not MyBase.Parser.IsFirstStatementOnLine(node.GetFirstToken())) Then
						node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_MethodMustBeFirstStatementOnLine)
					End If
					Dim methodBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax)
					If (Not methodBaseSyntax.Modifiers.Any(505)) Then
						typeBlockContext = New MethodBlockContext(syntaxKind, methodBaseSyntax, Me)
						Return typeBlockContext
					End If
					MyBase.Add(node)
					typeBlockContext = Me
					Return typeBlockContext
				End If
			Label4:
				MyBase.Add(node)
			ElseIf (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement) Then
				node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_InvOutsideProc)
				MyBase.Add(node)
			Else
				If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopStatement AndAlso kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStatement) Then
					If (Not SyntaxFacts.IsEndBlockLoopOrNextStatement(node.Kind)) Then
						node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_ExecutableAsDeclaration)
					End If
					MyBase.Add(node)
					typeBlockContext = Me
					Return typeBlockContext
				End If
				MyBase.Add(node)
			End If
			typeBlockContext = Me
			Return typeBlockContext
		End Function

		Friend Overrides Function RecoverFromMismatchedEnd(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_Syntax
			Dim kind As SyntaxKind = statement.Kind
			If (kind <= SyntaxKind.IncompleteMember) Then
				Select Case kind
					Case SyntaxKind.EmptyStatement
					Label2:
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnrecognizedEnd
						Exit Select
					Case SyntaxKind.List Or SyntaxKind.EmptyStatement
					Case 4
						Throw New ArgumentException("Statement must be an end block statement")
					Case SyntaxKind.EndIfStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_EndIfNoMatchingIf
						Exit Select
					Case SyntaxKind.EndUsingStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_EndUsingWithoutUsing
						Exit Select
					Case SyntaxKind.EndWithStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_EndWithWithoutWith
						Exit Select
					Case SyntaxKind.EndSelectStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_EndSelectNoSelect
						Exit Select
					Case SyntaxKind.EndStructureStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_EndStructureNoStructure
						Exit Select
					Case SyntaxKind.EndEnumStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidEndEnum
						Exit Select
					Case SyntaxKind.EndInterfaceStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidEndInterface
						Exit Select
					Case SyntaxKind.EndClassStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_EndClassNoClass
						Exit Select
					Case SyntaxKind.EndModuleStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_EndModuleNoModule
						Exit Select
					Case SyntaxKind.EndNamespaceStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_EndNamespaceNoNamespace
						Exit Select
					Case SyntaxKind.EndSubStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidEndSub
						Exit Select
					Case SyntaxKind.EndFunctionStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidEndFunction
						Exit Select
					Case SyntaxKind.EndGetStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidEndGet
						Exit Select
					Case SyntaxKind.EndSetStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidEndSet
						Exit Select
					Case SyntaxKind.EndPropertyStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidEndProperty
						Exit Select
					Case SyntaxKind.EndOperatorStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidEndOperator
						Exit Select
					Case SyntaxKind.EndEventStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidEndEvent
						Exit Select
					Case SyntaxKind.EndAddHandlerStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidEndAddHandler
						Exit Select
					Case SyntaxKind.EndRemoveHandlerStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidEndRemoveHandler
						Exit Select
					Case SyntaxKind.EndRaiseEventStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidEndRaiseEvent
						Exit Select
					Case SyntaxKind.EndWhileStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_EndWhileNoWhile
						Exit Select
					Case SyntaxKind.EndTryStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_EndTryNoTry
						Exit Select
					Case SyntaxKind.EndSyncLockStatement
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_EndSyncLockNoSyncLock
						Exit Select
					Case Else
						If (kind = SyntaxKind.IncompleteMember) Then
							GoTo Label2
						End If
						Throw New ArgumentException("Statement must be an end block statement")
				End Select
			ElseIf (kind = SyntaxKind.NextStatement) Then
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_NextNoMatchingFor
			Else
				If (CUShort(kind) - CUShort(SyntaxKind.SimpleLoopStatement) > CUShort(SyntaxKind.EmptyStatement)) Then
					Throw New ArgumentException("Statement must be an end block statement")
				End If
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_LoopNoMatchingDo
			End If
			statement = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(statement, eRRID)
			Return Me.ProcessSyntax(statement)
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			If (Not Me.KindEndsBlock(node.Kind)) Then
				Dim kind As SyntaxKind = node.Kind
				If (kind <= SyntaxKind.AttributesStatement) Then
					Select Case kind
						Case SyntaxKind.OptionStatement
						Case SyntaxKind.ImportsStatement
						Case SyntaxKind.NamespaceStatement
						Case SyntaxKind.InheritsStatement
						Case SyntaxKind.ImplementsStatement
						Case SyntaxKind.ModuleStatement
						Case SyntaxKind.StructureStatement
						Case SyntaxKind.InterfaceStatement
						Case SyntaxKind.ClassStatement
						Case SyntaxKind.EnumStatement
						Case SyntaxKind.SubStatement
						Case SyntaxKind.FunctionStatement
						Case SyntaxKind.SubNewStatement
						Case SyntaxKind.EventStatement
						Case SyntaxKind.OperatorStatement
						Case SyntaxKind.PropertyStatement
						Case SyntaxKind.GetAccessorStatement
						Case SyntaxKind.SetAccessorStatement
						Case SyntaxKind.AddHandlerAccessorStatement
						Case SyntaxKind.RemoveHandlerAccessorStatement
						Case SyntaxKind.RaiseEventAccessorStatement
							linkResult = MyBase.UseSyntax(node, newContext, False)
							Return linkResult
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
						Case SyntaxKind.GetAccessorBlock
						Case SyntaxKind.SetAccessorBlock
						Case SyntaxKind.AddHandlerAccessorBlock
						Case SyntaxKind.RemoveHandlerAccessorBlock
						Case SyntaxKind.RaiseEventAccessorBlock
						Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.PropertyBlock
						Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock
						Case SyntaxKind.ParameterList
						Case 100
						Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement
						Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement
						Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement
							Exit Select
						Case SyntaxKind.NamespaceBlock
						Case SyntaxKind.ModuleBlock
							newContext = Me
							linkResult = BlockContext.LinkResult.Crumble
							Return linkResult
						Case SyntaxKind.StructureBlock
						Case SyntaxKind.InterfaceBlock
						Case SyntaxKind.ClassBlock
							linkResult = MyBase.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockSyntax).EndBlockStatement.IsMissing)
							Return linkResult
						Case SyntaxKind.EnumBlock
							linkResult = MyBase.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax).EndEnumStatement.IsMissing)
							Return linkResult
						Case SyntaxKind.EnumMemberDeclaration
						Case SyntaxKind.DeclareSubStatement
						Case SyntaxKind.DeclareFunctionStatement
						Case SyntaxKind.DelegateSubStatement
						Case SyntaxKind.DelegateFunctionStatement
							linkResult = MyBase.UseSyntax(node, newContext, False)
							Return linkResult
						Case SyntaxKind.SubBlock
						Case SyntaxKind.FunctionBlock
						Case SyntaxKind.ConstructorBlock
							linkResult = MyBase.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax).[End].IsMissing)
							Return linkResult
						Case SyntaxKind.OperatorBlock
							If (MyBase.BlockKind <> SyntaxKind.ModuleBlock) Then
								linkResult = MyBase.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax).[End].IsMissing)
								Return linkResult
							Else
								newContext = Me
								linkResult = BlockContext.LinkResult.Crumble
								Return linkResult
							End If
						Case SyntaxKind.PropertyBlock
							linkResult = MyBase.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax).EndPropertyStatement.IsMissing)
							Return linkResult
						Case SyntaxKind.EventBlock
							linkResult = MyBase.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax).EndEventStatement.IsMissing)
							Return linkResult
						Case Else
							If (kind = SyntaxKind.FieldDeclaration) Then
								linkResult = MyBase.UseSyntax(node, newContext, False)
								Return linkResult
							End If
							If (kind = SyntaxKind.AttributesStatement) Then
								linkResult = MyBase.UseSyntax(node, newContext, False)
								Return linkResult
							End If
							Exit Select
					End Select
				ElseIf (kind = SyntaxKind.LocalDeclarationStatement) Then
					newContext = Me
					linkResult = BlockContext.LinkResult.Crumble
					Return linkResult
				ElseIf (kind = SyntaxKind.SingleLineIfStatement) Then
					newContext = Me
					linkResult = BlockContext.LinkResult.Crumble
					Return linkResult
				Else
					If (kind <> SyntaxKind.IfStatement) Then
						GoTo Label4
					End If
					node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_ExecutableAsDeclaration)
					linkResult = MyBase.TryUseStatement(node, newContext)
					Return linkResult
				End If
			Label4:
				newContext = Me
				linkResult = BlockContext.LinkResult.NotUsed
			Else
				linkResult = MyBase.UseSyntax(node, newContext, False)
			End If
			Return linkResult
		End Function
	End Class
End Namespace