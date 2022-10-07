Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Reflection

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class BlockContext
		Implements ISyntaxFactoryContext
		Private _beginStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax

		Protected _parser As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser

		Protected _statements As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)

		Private ReadOnly _kind As SyntaxKind

		Private ReadOnly _endKind As SyntaxKind

		Private ReadOnly _prev As BlockContext

		Private ReadOnly _isWithinMultiLineLambda As Boolean

		Private ReadOnly _isWithinSingleLineLambda As Boolean

		Private ReadOnly _isWithinAsyncMethodOrLambda As Boolean

		Private ReadOnly _isWithinIteratorMethodOrLambdaOrProperty As Boolean

		Private ReadOnly _level As Integer

		Private ReadOnly _syntaxFactory As ContextAwareSyntaxFactory

		Friend ReadOnly Property BeginStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Get
				Return Me._beginStatement
			End Get
		End Property

		Friend ReadOnly Property BlockKind As SyntaxKind
			Get
				Return Me._kind
			End Get
		End Property

		Friend Overridable ReadOnly Property IsLambda As Boolean
			Get
				Return False
			End Get
		End Property

		Friend ReadOnly Property IsLineIf As Boolean
			Get
				If (Me._kind = SyntaxKind.SingleLineIfStatement) Then
					Return True
				End If
				Return Me._kind = SyntaxKind.SingleLineElseClause
			End Get
		End Property

		Friend MustOverride ReadOnly Property IsSingleLine As Boolean

		Friend Overridable ReadOnly Property IsWithinAsyncMethodOrLambda As Boolean Implements ISyntaxFactoryContext.IsWithinAsyncMethodOrLambda
			Get
				Return Me._isWithinAsyncMethodOrLambda
			End Get
		End Property

		Friend Overridable ReadOnly Property IsWithinIteratorContext As Boolean Implements ISyntaxFactoryContext.IsWithinIteratorContext
			Get
				Return Me._isWithinIteratorMethodOrLambdaOrProperty
			End Get
		End Property

		Friend ReadOnly Property IsWithinIteratorMethodOrLambdaOrProperty As Boolean
			Get
				Return Me._isWithinIteratorMethodOrLambdaOrProperty
			End Get
		End Property

		Friend ReadOnly Property IsWithinLambda As Boolean
			Get
				Return Me._isWithinMultiLineLambda Or Me._isWithinSingleLineLambda
			End Get
		End Property

		Friend ReadOnly Property IsWithinSingleLineLambda As Boolean
			Get
				Return Me._isWithinSingleLineLambda
			End Get
		End Property

		Friend ReadOnly Property Level As Integer
			Get
				Return Me._level
			End Get
		End Property

		Friend Property Parser As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser
			Get
				Return Me._parser
			End Get
			Set(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser)
				Me._parser = value
			End Set
		End Property

		Friend ReadOnly Property PrevBlock As BlockContext
			Get
				Return Me._prev
			End Get
		End Property

		Friend ReadOnly Property Statements As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Get
				Return Me._statements
			End Get
		End Property

		Friend ReadOnly Property SyntaxFactory As ContextAwareSyntaxFactory
			Get
				Return Me._syntaxFactory
			End Get
		End Property

		Protected Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prev As BlockContext)
			MyBase.New()
			Dim modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)
			Dim num As Integer
			Me._beginStatement = statement
			Me._kind = kind
			Me._prev = prev
			Me._syntaxFactory = New ContextAwareSyntaxFactory(Me)
			If (prev IsNot Nothing) Then
				Me._isWithinSingleLineLambda = prev._isWithinSingleLineLambda
				Me._isWithinMultiLineLambda = prev._isWithinMultiLineLambda
			End If
			If (Not Me._isWithinSingleLineLambda) Then
				Me._isWithinSingleLineLambda = SyntaxFacts.IsSingleLineLambdaExpression(Me._kind)
			End If
			If (Not Me._isWithinMultiLineLambda) Then
				Me._isWithinMultiLineLambda = SyntaxFacts.IsMultiLineLambdaExpression(Me._kind)
			End If
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me._kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock) Then
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					modifiers = DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax).Modifiers
					Me._isWithinAsyncMethodOrLambda = modifiers.Any(630)
					modifiers = DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax).Modifiers
					Me._isWithinIteratorMethodOrLambdaOrProperty = modifiers.Any(632)
					Me._endKind = BlockContext.GetEndKind(kind)
					num = If(prev IsNot Nothing, prev.Level + 1, 0)
					Me._level = num
					If (prev IsNot Nothing) Then
						Me._parser = prev.Parser
						Me._statements = Me._parser._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)()
					End If
					Return
				Else
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						If (Me._prev IsNot Nothing) Then
							Me._isWithinAsyncMethodOrLambda = Me._prev.IsWithinAsyncMethodOrLambda
							Me._isWithinIteratorMethodOrLambdaOrProperty = Me._prev.IsWithinIteratorMethodOrLambdaOrProperty
						End If
						Me._endKind = BlockContext.GetEndKind(kind)
						num = If(prev IsNot Nothing, prev.Level + 1, 0)
						Me._level = num
						If (prev IsNot Nothing) Then
							Me._parser = prev.Parser
							Me._statements = Me._parser._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)()
						End If
						Return
					End If
					Me._isWithinIteratorMethodOrLambdaOrProperty = Me._prev.IsWithinIteratorMethodOrLambdaOrProperty
					Me._endKind = BlockContext.GetEndKind(kind)
					num = If(prev IsNot Nothing, prev.Level + 1, 0)
					Me._level = num
					If (prev IsNot Nothing) Then
						Me._parser = prev.Parser
						Me._statements = Me._parser._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)()
					End If
					Return
				End If
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock) Then
				modifiers = DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax).Modifiers
				Me._isWithinIteratorMethodOrLambdaOrProperty = modifiers.Any(632)
				Me._endKind = BlockContext.GetEndKind(kind)
				num = If(prev IsNot Nothing, prev.Level + 1, 0)
				Me._level = num
				If (prev IsNot Nothing) Then
					Me._parser = prev.Parser
					Me._statements = Me._parser._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)()
				End If
				Return
			Else
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression AndAlso CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
					If (Me._prev IsNot Nothing) Then
						Me._isWithinAsyncMethodOrLambda = Me._prev.IsWithinAsyncMethodOrLambda
						Me._isWithinIteratorMethodOrLambdaOrProperty = Me._prev.IsWithinIteratorMethodOrLambdaOrProperty
					End If
					Me._endKind = BlockContext.GetEndKind(kind)
					num = If(prev IsNot Nothing, prev.Level + 1, 0)
					Me._level = num
					If (prev IsNot Nothing) Then
						Me._parser = prev.Parser
						Me._statements = Me._parser._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)()
					End If
					Return
				End If
				modifiers = DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax).Modifiers
				Me._isWithinAsyncMethodOrLambda = modifiers.Any(630)
				modifiers = DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax).Modifiers
				Me._isWithinIteratorMethodOrLambdaOrProperty = modifiers.Any(632)
				Me._endKind = BlockContext.GetEndKind(kind)
				num = If(prev IsNot Nothing, prev.Level + 1, 0)
				Me._level = num
				If (prev IsNot Nothing) Then
					Me._parser = prev.Parser
					Me._statements = Me._parser._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)()
				End If
				Return
			End If
			If (Me._prev IsNot Nothing) Then
				Me._isWithinAsyncMethodOrLambda = Me._prev.IsWithinAsyncMethodOrLambda
				Me._isWithinIteratorMethodOrLambdaOrProperty = Me._prev.IsWithinIteratorMethodOrLambdaOrProperty
			End If
			Me._endKind = BlockContext.GetEndKind(kind)
			num = If(prev IsNot Nothing, prev.Level + 1, 0)
			Me._level = num
			If (prev IsNot Nothing) Then
				Me._parser = prev.Parser
				Me._statements = Me._parser._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)()
			End If
		End Sub

		Friend Sub Add(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Me._statements.Add(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax))
		End Sub

		Friend Function BaseDeclarations(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsOrImplementsStatementSyntax)() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of T)
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of T) = Me._statements.ToList(Of T)()
			Me._statements.Clear()
			Return list
		End Function

		Friend Function Body() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = Me._statements.ToList()
			Me._statements.Clear()
			Return list
		End Function

		Friend Function Body(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of T)
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of T) = Me._statements.ToList(Of T)()
			Me._statements.Clear()
			Return list
		End Function

		Friend Function BodyWithWeakChildren() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			If (Not BlockContext.IsLargeEnoughNonEmptyStatementList(Me._statements)) Then
				syntaxList = Me.Body()
			Else
				Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList.List(Me._statements.ToArray()))
				Me._statements.Clear()
				syntaxList = syntaxList1
			End If
			Return syntaxList
		End Function

		Friend MustOverride Function CreateBlockSyntax(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode

		Private Function CreateMissingEnd(ByRef errorId As ERRID) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Return Me.CreateMissingEnd(Me.BlockKind, errorId)
		End Function

		Private Function CreateMissingEnd(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByRef errorId As ERRID) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndKeyword)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseBlock) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock) Then
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock
							statementSyntax = Me.SyntaxFactory.EndWhileStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword))
							errorId = ERRID.ERR_ExpectedEndWhile
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeTarget Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock
							Throw ExceptionUtilities.UnexpectedValue(kind)
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock
							statementSyntax = Me.SyntaxFactory.EndUsingStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword))
							errorId = ERRID.ERR_ExpectedEndUsing
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock
							statementSyntax = Me.SyntaxFactory.EndSyncLockStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword))
							errorId = ERRID.ERR_ExpectedEndSyncLock
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithBlock
							statementSyntax = Me.SyntaxFactory.EndWithStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword))
							errorId = ERRID.ERR_ExpectedEndWith
							Exit Select
						Case Else
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
								statementSyntax = Me.SyntaxFactory.EndIfStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfKeyword))
								errorId = ERRID.ERR_ExpectedEndIf
								Exit Select
							Else
								Throw ExceptionUtilities.UnexpectedValue(kind)
							End If
					End Select
				Else
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock
							statementSyntax = Me.SyntaxFactory.EndNamespaceStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceKeyword))
							errorId = ERRID.ERR_ExpectedEndNamespace
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement
							Throw ExceptionUtilities.UnexpectedValue(kind)
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock
							statementSyntax = Me.SyntaxFactory.EndModuleStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleKeyword))
							errorId = ERRID.ERR_ExpectedEndModule
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock
							statementSyntax = Me.SyntaxFactory.EndStructureStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword))
							errorId = ERRID.ERR_ExpectedEndStructure
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock
							statementSyntax = Me.SyntaxFactory.EndInterfaceStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceKeyword))
							errorId = ERRID.ERR_MissingEndInterface
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock
							statementSyntax = Me.SyntaxFactory.EndClassStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword))
							errorId = ERRID.ERR_ExpectedEndClass
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock
							statementSyntax = Me.SyntaxFactory.EndEnumStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumKeyword))
							errorId = ERRID.ERR_MissingEndEnum
							Exit Select
						Case Else
							Select Case syntaxKind
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock
									statementSyntax = Me.SyntaxFactory.EndSubStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword))
									errorId = ERRID.ERR_EndSubExpected

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock
									statementSyntax = Me.SyntaxFactory.EndFunctionStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword))
									errorId = ERRID.ERR_EndFunctionExpected

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock
									statementSyntax = Me.SyntaxFactory.EndOperatorStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword))
									errorId = ERRID.ERR_EndOperatorExpected

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock
									statementSyntax = Me.SyntaxFactory.EndGetStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetKeyword))
									errorId = ERRID.ERR_MissingEndGet

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock
									statementSyntax = Me.SyntaxFactory.EndSetStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword))
									errorId = ERRID.ERR_MissingEndSet

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock
									statementSyntax = Me.SyntaxFactory.EndAddHandlerStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerKeyword))
									errorId = ERRID.ERR_MissingEndAddHandler

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock
									statementSyntax = Me.SyntaxFactory.EndRemoveHandlerStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword))
									errorId = ERRID.ERR_MissingEndRemoveHandler

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock
									statementSyntax = Me.SyntaxFactory.EndRaiseEventStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword))
									errorId = ERRID.ERR_MissingEndRaiseEvent

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock
									statementSyntax = Me.SyntaxFactory.EndPropertyStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword))
									errorId = ERRID.ERR_EndProp

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock
									statementSyntax = Me.SyntaxFactory.EndEventStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventKeyword))
									errorId = ERRID.ERR_MissingEndEvent

								Case Else
									Throw ExceptionUtilities.UnexpectedValue(kind)
							End Select

					End Select
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForEachBlock) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryBlock) Then
					statementSyntax = Me.SyntaxFactory.EndTryStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword))
					errorId = ERRID.ERR_ExpectedEndTry
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectBlock) Then
					statementSyntax = Me.SyntaxFactory.EndSelectStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword))
					errorId = ERRID.ERR_ExpectedEndSelect
				Else
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						Throw ExceptionUtilities.UnexpectedValue(kind)
					End If
					statementSyntax = Me.SyntaxFactory.NextStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextKeyword), New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)())
					errorId = ERRID.ERR_ExpectedNext
				End If
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineFunctionLambdaExpression) Then
				statementSyntax = Me.SyntaxFactory.EndFunctionStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword))
				errorId = ERRID.ERR_MultilineLambdaMissingFunction
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineSubLambdaExpression) Then
				statementSyntax = Me.SyntaxFactory.EndSubStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword))
				errorId = ERRID.ERR_MultilineLambdaMissingSub
			Else
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleDoLoopBlock) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					Throw ExceptionUtilities.UnexpectedValue(kind)
				End If
				statementSyntax = Me.SyntaxFactory.SimpleLoopStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LoopKeyword), Nothing)
				errorId = ERRID.ERR_ExpectedLoop
			End If
			Return statementSyntax
		End Function

		Friend MustOverride Function EndBlock(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext

		Friend Sub FreeStatements()
			Me._parser._pool.Free(Me._statements)
		End Sub

		Friend Sub GetBeginEndStatements(Of T1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, T2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(ByRef beginStmt As T1, ByRef endStmt As T2)
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = 0
			beginStmt = DirectCast(Me.BeginStatement, T1)
			If (endStmt Is Nothing) Then
				endStmt = DirectCast(Me.CreateMissingEnd(eRRID), T2)
				If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
					beginStmt = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of T1)(beginStmt, eRRID)
				End If
			End If
		End Sub

		Private Shared Function GetEndKind(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyBlock) Then
				If (syntaxKind1 > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock) Then
					Select Case syntaxKind1
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeTarget Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock
							Throw ExceptionUtilities.UnexpectedValue(kind)
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithBlock
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement
							Exit Select
						Case Else
							Select Case syntaxKind1
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause
									syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfPart
									Throw ExceptionUtilities.UnexpectedValue(kind)
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock
								Label5:
									syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement

								Case Else
									Select Case syntaxKind1
										Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfBlock
										Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseBlock
											GoTo Label5
										Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfStatement
										Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfStatement
										Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseStatement
										Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextLabel Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseStatement
											Throw ExceptionUtilities.UnexpectedValue(kind)
										Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryBlock
										Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchBlock
										Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyBlock
											syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement

										Case Else
											Throw ExceptionUtilities.UnexpectedValue(kind)
									End Select

							End Select

					End Select
				Else
					If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
						Return syntaxKind
					End If
					Select Case syntaxKind1
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement
							Throw ExceptionUtilities.UnexpectedValue(kind)
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement
							Exit Select
						Case Else
							Select Case syntaxKind1
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock
									syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement
									Return syntaxKind
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock
									syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement
									Return syntaxKind
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock
									syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock
									syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock
									syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock
									syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock
									syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock
									syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock
									syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement

								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock
									syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement

								Case Else
									Throw ExceptionUtilities.UnexpectedValue(kind)
							End Select

					End Select
				End If
			ElseIf (syntaxKind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseElseBlock) Then
				If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectBlock AndAlso syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseBlock AndAlso syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseElseBlock) Then
					Throw ExceptionUtilities.UnexpectedValue(kind)
				End If
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement
			ElseIf (CUShort(syntaxKind1) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextStatement
			Else
				Select Case syntaxKind1
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
						Return syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfExpression
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CallStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryConditionalExpression
						Throw ExceptionUtilities.UnexpectedValue(kind)
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineFunctionLambdaExpression
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement
						Return syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineSubLambdaExpression
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement
						Return syntaxKind
					Case Else
						If (CUShort(syntaxKind1) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleDoLoopBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleLoopStatement
							Exit Select
						Else
							Throw ExceptionUtilities.UnexpectedValue(kind)
						End If
				End Select
			End If
			Return syntaxKind
		End Function

		Private Sub HandleAnyUnexpectedTokens(ByVal currentStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal unexpected As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken))
			Dim count As Integer
			Dim item As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			If (unexpected.Node IsNot Nothing) Then
				If (Me._statements.Count <> 0) Then
					count = Me._statements.Count - 1
					item = Me._statements(count)
				Else
					count = -1
					item = Me._beginStatement
				End If
				item = If(currentStmt.ContainsDiagnostics OrElse unexpected.ContainsDiagnostics(), item.AddTrailingSyntax(unexpected), item.AddTrailingSyntax(unexpected, ERRID.ERR_ExpectedEOS))
				If (count = -1) Then
					Me._beginStatement = item
					Return
				End If
				Me._statements(count) = item
			End If
		End Sub

		Private Shared Function IsLargeEnoughNonEmptyStatementList(ByVal statements As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)) As Boolean
			Dim flag As Boolean
			If (statements.Count <> 0) Then
				flag = If(statements.Count > 2, True, statements(0).Width > 60)
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Overridable Function KindEndsBlock(ByVal kind As SyntaxKind) As Boolean
			Return Me._endKind = kind
		End Function

		Friend Function LinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim kind As SyntaxKind = node.Kind
			Dim prevBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = Me
			While prevBlock IsNot Nothing
				If (prevBlock.KindEndsBlock(kind)) Then
					Dim blockContext1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = Me.FindNearestLambdaOrSingleLineIf(prevBlock)
					If (blockContext1 Is Nothing) Then
						If (prevBlock <> Me) Then
							Me.RecoverFromMissingEnd(prevBlock)
						End If
						blockContext = prevBlock.EndBlock(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax))
						Return blockContext
					Else
						If (blockContext1.IsLambda) Then
							Exit While
						End If
						node = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_BogusWithinLineIf)
						blockContext = Me.ProcessSyntax(node)
						Return blockContext
					End If
				ElseIf (Not SyntaxFacts.IsEndBlockLoopOrNextStatement(kind)) Then
					blockContext = Me.ProcessSyntax(node)
					Return blockContext
				Else
					prevBlock = prevBlock.PrevBlock
				End If
			End While
			blockContext = Me.RecoverFromMismatchedEnd(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax))
			Return blockContext
		End Function

		Friend Function OptionalBody() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = Me.SingleStatementOrDefault()
			syntaxList = If(statementSyntax Is Nothing OrElse statementSyntax.Kind <> SyntaxKind.EmptyStatement OrElse statementSyntax.FullWidth <> 0, Me.Body(), New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)())
			Return syntaxList
		End Function

		Friend MustOverride Function Parse() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax

		Friend MustOverride Function ProcessStatementTerminator(ByVal lambdaContext As BlockContext) As BlockContext

		Friend MustOverride Function ProcessSyntax(ByVal syntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As BlockContext

		Friend MustOverride Function RecoverFromMismatchedEnd(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As BlockContext

		Friend Overridable Function ResyncAndProcessStatementTerminator(ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal lambdaContext As BlockContext) As BlockContext
			Me.HandleAnyUnexpectedTokens(statement, Me.Parser.ResyncAt())
			Return Me.ProcessStatementTerminator(lambdaContext)
		End Function

		Friend Function SingleStatementOrDefault() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			If (Me._statements.Count <> 1) Then
				Return Nothing
			End If
			Return Me._statements(0)
		End Function

		Friend Function TryLinkStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			newContext = Nothing
			Dim kind As SyntaxKind = node.Kind
			If (kind <= SyntaxKind.TryBlock) Then
				If (kind <= SyntaxKind.SingleLineIfStatement) Then
					Select Case kind
						Case SyntaxKind.WhileBlock
							linkResult = Me.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax).EndWhileStatement.IsMissing)
							Exit Select
						Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement
						Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock
							linkResult = Me.TryUseStatement(node, newContext)
							Return linkResult
						Case SyntaxKind.UsingBlock
							linkResult = Me.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax).EndUsingStatement.IsMissing)
							Exit Select
						Case SyntaxKind.SyncLockBlock
							linkResult = Me.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax).EndSyncLockStatement.IsMissing)
							Exit Select
						Case SyntaxKind.WithBlock
							linkResult = Me.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax).EndWithStatement.IsMissing)
							Exit Select
						Case Else
							If (kind = SyntaxKind.SingleLineIfStatement) Then
								linkResult = Me.UseSyntax(node, newContext, False)
								Exit Select
							Else
								linkResult = Me.TryUseStatement(node, newContext)
								Return linkResult
							End If
					End Select
				ElseIf (kind = SyntaxKind.MultiLineIfBlock) Then
					linkResult = Me.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax).EndIfStatement.IsMissing)
				Else
					If (kind <> SyntaxKind.TryBlock) Then
						linkResult = Me.TryUseStatement(node, newContext)
						Return linkResult
					End If
					linkResult = Me.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax).EndTryStatement.IsMissing)
				End If
			ElseIf (kind <= SyntaxKind.ForEachBlock) Then
				If (kind = SyntaxKind.SelectBlock) Then
					linkResult = Me.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax).EndSelectStatement.IsMissing)
				Else
					If (CUShort(kind) - CUShort(SyntaxKind.ForBlock) > CUShort(SyntaxKind.List)) Then
						linkResult = Me.TryUseStatement(node, newContext)
						Return linkResult
					End If
					newContext = Me
					linkResult = BlockContext.LinkResult.Crumble
				End If
			ElseIf (kind = SyntaxKind.NextStatement) Then
				newContext = Me
				linkResult = BlockContext.LinkResult.NotUsed
			Else
				If (CUShort(kind) - CUShort(SyntaxKind.SimpleDoLoopBlock) > 4) Then
					linkResult = Me.TryUseStatement(node, newContext)
					Return linkResult
				End If
				linkResult = Me.UseSyntax(node, newContext, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax).LoopStatement.IsMissing)
			End If
			Return linkResult
		End Function

		Friend MustOverride Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult

		Friend Function TryProcessExecutableStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim statementBlockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim kind As SyntaxKind = node.Kind
			If (kind > SyntaxKind.SelectStatement) Then
				If (kind <= SyntaxKind.SyncLockStatement) Then
					If (kind = SyntaxKind.CaseStatement) Then
						Me.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_CaseNoSelect))
						statementBlockContext = Me
						Return statementBlockContext
					Else
						If (kind <> SyntaxKind.CaseElseStatement) Then
							If (kind <> SyntaxKind.SyncLockStatement) Then
								If (Not TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax) Then
									statementBlockContext = Nothing
									Return statementBlockContext
								End If
								Me.Add(node)
								statementBlockContext = Me
								Return statementBlockContext
							End If
							statementBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementBlockContext(SyntaxKind.SyncLockBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
							Return statementBlockContext
						End If
						Me.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_CaseElseNoSelect))
						statementBlockContext = Me
						Return statementBlockContext
					End If
				ElseIf (kind > SyntaxKind.WithStatement) Then
					If (CUShort(kind) - CUShort(SyntaxKind.SimpleDoLoopBlock) <= 4) Then
						Me.Add(node)
						statementBlockContext = Me
						Return statementBlockContext
					End If
					If (CUShort(kind) - CUShort(SyntaxKind.SimpleDoStatement) > CUShort(SyntaxKind.EmptyStatement)) Then
						If (Not TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax) Then
							statementBlockContext = Nothing
							Return statementBlockContext
						End If
						Me.Add(node)
						statementBlockContext = Me
						Return statementBlockContext
					End If
					statementBlockContext = New DoLoopBlockContext(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
				Else
					Select Case kind
						Case SyntaxKind.WhileStatement
							statementBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementBlockContext(SyntaxKind.WhileBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
							Exit Select
						Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement
						Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NewConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.Attribute Or SyntaxKind.PrintStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.SelectStatement
						Case SyntaxKind.ForStepClause
						Case SyntaxKind.NextStatement
							If (Not TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax) Then
								statementBlockContext = Nothing
								Return statementBlockContext
							End If
							Me.Add(node)
							statementBlockContext = Me
							Return statementBlockContext
						Case SyntaxKind.ForBlock
						Case SyntaxKind.ForEachBlock
							Me.Add(node)
							statementBlockContext = Me
							Return statementBlockContext
						Case SyntaxKind.ForStatement
						Case SyntaxKind.ForEachStatement
							statementBlockContext = New ForBlockContext(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
							Exit Select
						Case SyntaxKind.UsingStatement
							statementBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementBlockContext(SyntaxKind.UsingBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
							Exit Select
						Case Else
							If (kind = SyntaxKind.WithStatement) Then
								statementBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementBlockContext(SyntaxKind.WithBlock, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
								Exit Select
							Else
								If (Not TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax) Then
									statementBlockContext = Nothing
									Return statementBlockContext
								End If
								Me.Add(node)
								statementBlockContext = Me
								Return statementBlockContext
							End If
					End Select
				End If
			ElseIf (kind <= SyntaxKind.SingleLineIfStatement) Then
				If (kind = SyntaxKind.WhileBlock OrElse CUShort(kind) - CUShort(SyntaxKind.UsingBlock) <= CUShort(SyntaxKind.EmptyStatement)) Then
					Me.Add(node)
					statementBlockContext = Me
					Return statementBlockContext
				End If
				If (kind <> SyntaxKind.SingleLineIfStatement) Then
					If (Not TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax) Then
						statementBlockContext = Nothing
						Return statementBlockContext
					End If
					Me.Add(node)
					statementBlockContext = Me
					Return statementBlockContext
				End If
				Me.Add(node)
				statementBlockContext = Me
				Return statementBlockContext
			ElseIf (kind > SyntaxKind.FinallyStatement) Then
				If (kind = SyntaxKind.SelectBlock) Then
					Me.Add(node)
					statementBlockContext = Me
					Return statementBlockContext
				End If
				If (kind <> SyntaxKind.SelectStatement) Then
					If (Not TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax) Then
						statementBlockContext = Nothing
						Return statementBlockContext
					End If
					Me.Add(node)
					statementBlockContext = Me
					Return statementBlockContext
				End If
				statementBlockContext = New SelectBlockContext(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
			ElseIf (kind <> SyntaxKind.MultiLineIfBlock) Then
				Select Case kind
					Case SyntaxKind.IfStatement
						Dim ifStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax)
						If (ifStatementSyntax.ThenKeyword Is Nothing OrElse SyntaxFacts.IsTerminator(Me.Parser.CurrentToken.Kind)) Then
							statementBlockContext = New IfBlockContext(ifStatementSyntax, Me)
							Exit Select
						Else
							statementBlockContext = New SingleLineIfBlockContext(ifStatementSyntax, Me)
							Exit Select
						End If
					Case SyntaxKind.ElseIfStatement
						Me.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_ElseIfNoMatchingIf))
						statementBlockContext = Me
						Return statementBlockContext
					Case SyntaxKind.ElseStatement
						Me.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, ERRID.ERR_ElseNoMatchingIf))
						statementBlockContext = Me
						Return statementBlockContext
					Case SyntaxKind.TryBlock
						Me.Add(node)
						statementBlockContext = Me
						Return statementBlockContext
					Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.ModuleBlock Or SyntaxKind.ImplementsStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.NextLabel Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.ElseStatement
					Case SyntaxKind.CatchBlock
					Case SyntaxKind.FinallyBlock
					Case SyntaxKind.CatchFilterClause
					Case 192
					Case SyntaxKind.List Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue
						If (Not TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax) Then
							statementBlockContext = Nothing
							Return statementBlockContext
						End If
						Me.Add(node)
						statementBlockContext = Me
						Return statementBlockContext
					Case SyntaxKind.TryStatement
						statementBlockContext = New TryBlockContext(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax), Me)
						Exit Select
					Case SyntaxKind.CatchStatement
					Case SyntaxKind.FinallyStatement
						Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = Me.FindNearestInSameMethodScope(New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("3CA1F78E51457D6E319328B7035A5913D87AC0FD929B24ED0EAC5A89C9A77B97").FieldHandle })
						If (blockContext Is Nothing) Then
							Me.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, If(node.Kind = SyntaxKind.CatchStatement, ERRID.ERR_CatchNoMatchingTry, ERRID.ERR_FinallyNoMatchingTry)))
							statementBlockContext = Me
							Return statementBlockContext
						Else
							Me.RecoverFromMissingEnd(blockContext)
							statementBlockContext = blockContext.ProcessSyntax(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax))
							Exit Select
						End If
					Case Else
						If (Not TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax) Then
							statementBlockContext = Nothing
							Return statementBlockContext
						End If
						Me.Add(node)
						statementBlockContext = Me
						Return statementBlockContext
				End Select
			Else
				Me.Add(node)
				statementBlockContext = Me
				Return statementBlockContext
			End If
			Return statementBlockContext
		End Function

		Friend Function TryUseStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			linkResult = If(statementSyntax Is Nothing, BlockContext.LinkResult.NotUsed, Me.UseSyntax(statementSyntax, newContext, False))
			Return linkResult
		End Function

		Friend Function UseSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext, Optional ByVal AddMissingTerminator As Boolean = False) As BlockContext.LinkResult
			Me.Parser.GetNextSyntaxNode()
			newContext = Me.LinkSyntax(node)
			Return If(Not AddMissingTerminator, BlockContext.LinkResult.Used, BlockContext.LinkResult.Used Or BlockContext.LinkResult.MissingTerminator)
		End Function

		<Flags>
		Friend Enum LinkResult
			NotUsed = 0
			Used = 1
			SkipTerminator = 2
			MissingTerminator = 4
			TerminatorFlags = 6
			Crumble = 8
		End Enum
	End Class
End Namespace