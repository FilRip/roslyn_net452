Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module InitializerRewriter
		Friend Function BuildConstructorBody(ByVal compilationState As TypeCompilationState, ByVal constructorMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal constructorInitializerOpt As BoundStatement, ByVal processedInitializers As Binder.ProcessedFieldOrPropertyInitializers, ByVal block As Microsoft.CodeAnalysis.VisualBasic.BoundBlock) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim num As Integer
			Dim flag As Boolean = False
			Dim containingType As NamedTypeSymbol = constructorMethod.ContainingType
			If (Not InitializerRewriter.HasExplicitMeConstructorCall(block, containingType, flag) OrElse flag) Then
				If (processedInitializers.InitializerStatements.IsDefault) Then
					processedInitializers.InitializerStatements = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of BoundInitializer, BoundStatement)(processedInitializers.BoundInitializers, New Func(Of BoundInitializer, BoundStatement)(AddressOf InitializerRewriter.RewriteInitializerAsStatement))
				End If
				Dim initializerStatements As ImmutableArray(Of BoundStatement) = processedInitializers.InitializerStatements
				Dim statements As ImmutableArray(Of BoundStatement) = block.Statements
				Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
				If (constructorInitializerOpt IsNot Nothing) Then
					instance.Add(constructorInitializerOpt)
				ElseIf (flag) Then
					instance.Add(statements(0))
				ElseIf (Not constructorMethod.IsShared AndAlso containingType.IsValueType) Then
					Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = block.Syntax
					instance.Add(New BoundExpressionStatement(syntax, New BoundAssignmentOperator(syntax, New BoundValueTypeMeReference(syntax, containingType), New BoundConversion(syntax, New BoundLiteral(syntax, ConstantValue.Null, Nothing), ConversionKind.WideningNothingLiteral, False, False, containingType, False), True, containingType, False), False))
				End If
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = containingType.GetMembers().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (current.Kind <> SymbolKind.Method) Then
						Continue While
					End If
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					Dim handledEvents As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.HandledEvent) = methodSymbol.HandledEvents
					If (handledEvents.IsEmpty) Then
						Continue While
					End If
					If (methodSymbol.IsPartial()) Then
						Dim partialImplementationPart As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = methodSymbol.PartialImplementationPart
						If (partialImplementationPart Is Nothing) Then
							Continue While
						End If
						handledEvents = Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of Microsoft.CodeAnalysis.VisualBasic.HandledEvent)(handledEvents, partialImplementationPart.HandledEvents)
					End If
					Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.HandledEvent).Enumerator = handledEvents.GetEnumerator()
					While enumerator1.MoveNext()
						Dim handledEvent As Microsoft.CodeAnalysis.VisualBasic.HandledEvent = enumerator1.Current
						If (handledEvent.hookupMethod.MethodKind <> constructorMethod.MethodKind) Then
							Continue While
						End If
						Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = DirectCast(handledEvent.EventSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol)
						Dim addMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = eventSymbol.AddMethod
						Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = handledEvent.delegateCreation
						Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = boundExpression.Syntax
						Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
						If (Not addMethod.IsShared) Then
							Dim meParameter As ParameterSymbol = constructorMethod.MeParameter
							If (Not TypeSymbol.Equals(addMethod.ContainingType, containingType, TypeCompareKind.ConsiderEverything)) Then
								boundExpression1 = (New BoundMyBaseReference(syntaxNode, meParameter.Type)).MakeCompilerGenerated()
							Else
								boundExpression1 = (New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(syntaxNode, meParameter.Type)).MakeCompilerGenerated()
							End If
						End If
						instance.Add((New BoundAddHandlerStatement(syntaxNode, (New BoundEventAccess(syntaxNode, boundExpression1, eventSymbol, eventSymbol.Type, False)).MakeCompilerGenerated(), boundExpression, False)).MakeCompilerGenerated())
					End While
				End While
				instance.AddRange(initializerStatements)
				If (Not constructorMethod.IsShared AndAlso compilationState.InitializeComponentOpt IsNot Nothing AndAlso constructorMethod.IsImplicitlyDeclared) Then
					Dim syntax1 As Microsoft.CodeAnalysis.SyntaxNode = constructorMethod.Syntax
					Dim initializeComponentOpt As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = compilationState.InitializeComponentOpt
					Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(constructorMethod.Syntax, compilationState.InitializeComponentOpt.ContainingType)
					Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty
					Dim returnType As TypeSymbol = compilationState.InitializeComponentOpt.ReturnType
					Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
					instance.Add((New BoundCall(syntax1, initializeComponentOpt, Nothing, boundMeReference, empty, Nothing, returnType, False, False, bitVector)).MakeCompilerGenerated().ToStatement().MakeCompilerGenerated())
				End If
				If (instance.Count <> 0) Then
					num = If(flag, 1, 0)
					Dim length As Integer = statements.Length - 1
					Dim num1 As Integer = num
					Do
						instance.Add(statements(num1))
						num1 = num1 + 1
					Loop While num1 <= length
					boundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(block.Syntax, block.StatementListSyntax, block.Locals, instance.ToImmutableAndFree(), block.HasErrors)
				Else
					instance.Free()
					boundBlock = block
				End If
			Else
				boundBlock = block
			End If
			Return boundBlock
		End Function

		Friend Function BuildScriptInitializerBody(ByVal initializerMethod As SynthesizedInteractiveInitializerMethod, ByVal processedInitializers As Binder.ProcessedFieldOrPropertyInitializers, ByVal block As BoundBlock) As BoundBlock
			Dim boundStatements As ImmutableArray(Of BoundStatement) = InitializerRewriter.RewriteInitializersAsStatements(initializerMethod, processedInitializers.BoundInitializers)
			processedInitializers.InitializerStatements = boundStatements
			Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
			instance.AddRange(boundStatements)
			instance.AddRange(block.Statements)
			Return New BoundBlock(block.Syntax, block.StatementListSyntax, block.Locals, instance.ToImmutableAndFree(), block.HasErrors)
		End Function

		Friend Function HasExplicitMeConstructorCall(ByVal block As BoundBlock, ByVal container As TypeSymbol, <Out> ByRef isMyBaseConstructorCall As Boolean) As Boolean
			Dim flag As Boolean
			isMyBaseConstructorCall = False
			If (System.Linq.ImmutableArrayExtensions.Any(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(block.Statements)) Then
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = block.Statements.First()
				If (boundStatement.Kind = BoundKind.ExpressionStatement) Then
					Dim expression As BoundExpression = DirectCast(boundStatement, BoundExpressionStatement).Expression
					If (expression.Kind = BoundKind.[Call]) Then
						Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = DirectCast(expression, Microsoft.CodeAnalysis.VisualBasic.BoundCall)
						Dim receiverOpt As BoundExpression = boundCall.ReceiverOpt
						If (receiverOpt IsNot Nothing AndAlso receiverOpt.IsInstanceReference()) Then
							Dim method As MethodSymbol = boundCall.Method
							If (method.MethodKind <> MethodKind.Constructor) Then
								flag = False
								Return flag
							End If
							isMyBaseConstructorCall = receiverOpt.IsMyBaseReference()
							flag = TypeSymbol.Equals(method.ContainingType, container, TypeCompareKind.ConsiderEverything)
							Return flag
						End If
					End If
				End If
			End If
			flag = False
			Return flag
		End Function

		Private Function RewriteInitializerAsStatement(ByVal initializer As BoundInitializer) As BoundStatement
			Dim statement As BoundStatement
			Dim kind As BoundKind = initializer.Kind
			If (CByte(kind) - CByte(BoundKind.FieldInitializer) <= CByte(BoundKind.OmittedArgument)) Then
				statement = initializer
			Else
				If (kind <> BoundKind.GlobalStatementInitializer) Then
					Throw ExceptionUtilities.UnexpectedValue(initializer.Kind)
				End If
				statement = DirectCast(initializer, BoundGlobalStatementInitializer).Statement
			End If
			Return statement
		End Function

		Private Function RewriteInitializersAsStatements(ByVal method As SynthesizedInteractiveInitializerMethod, ByVal boundInitializers As ImmutableArray(Of BoundInitializer)) As ImmutableArray(Of BoundStatement)
			Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance(boundInitializers.Length)
			Dim resultType As TypeSymbol = method.ResultType
			Dim boundLiteral As BoundExpression = Nothing
			Dim enumerator As ImmutableArray(Of BoundInitializer).Enumerator = boundInitializers.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As BoundInitializer = enumerator.Current
				If (resultType IsNot Nothing AndAlso current = boundInitializers.Last() AndAlso current.Kind = BoundKind.GlobalStatementInitializer) Then
					Dim statement As BoundStatement = DirectCast(current, BoundGlobalStatementInitializer).Statement
					If (statement.Kind = BoundKind.ExpressionStatement) Then
						Dim expression As BoundExpression = DirectCast(statement, BoundExpressionStatement).Expression
						If (expression.Type.SpecialType <> SpecialType.System_Void) Then
							boundLiteral = expression
							Continue While
						End If
					End If
				End If
				instance.Add(InitializerRewriter.RewriteInitializerAsStatement(current))
			End While
			If (resultType IsNot Nothing) Then
				If (boundLiteral Is Nothing) Then
					boundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(method.Syntax, ConstantValue.[Nothing], resultType)
				End If
				instance.Add(New BoundReturnStatement(boundLiteral.Syntax, boundLiteral, method.FunctionLocal, method.ExitLabel, False))
			End If
			Return instance.ToImmutableAndFree()
		End Function
	End Module
End Namespace