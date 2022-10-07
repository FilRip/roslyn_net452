Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundTreeRewriter
		Inherits BoundTreeVisitor
		Protected Sub New()
			MyBase.New()
		End Sub

		Public Overrides Function VisitAddHandlerStatement(ByVal node As BoundAddHandlerStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.EventAccess), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Handler), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(boundExpression, boundExpression1)
		End Function

		Public Overrides Function VisitAddressOfOperator(ByVal node As BoundAddressOfOperator) As BoundNode
			Dim boundMethodGroup As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup = DirectCast(Me.Visit(node.MethodGroup), Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup)
			Me.VisitType(node.Type)
			Return node.Update(node.Binder, node.WithDependencies, boundMethodGroup)
		End Function

		Public Overrides Function VisitAggregateClause(ByVal node As BoundAggregateClause) As BoundNode
			Dim boundQueryClauseBase As Microsoft.CodeAnalysis.VisualBasic.BoundQueryClauseBase = DirectCast(Me.Visit(node.CapturedGroupOpt), Microsoft.CodeAnalysis.VisualBasic.BoundQueryClauseBase)
			Dim boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = DirectCast(Me.Visit(node.GroupPlaceholderOpt), Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.UnderlyingExpression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.CompoundVariableType)
			Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundQueryClauseBase, boundRValuePlaceholder, boundExpression, node.RangeVariables, typeSymbol, node.Binders, typeSymbol1)
		End Function

		Public Overrides Function VisitAnonymousTypeCreationExpression(ByVal node As BoundAnonymousTypeCreationExpression) As BoundNode
			Dim boundAnonymousTypePropertyAccesses As ImmutableArray(Of BoundAnonymousTypePropertyAccess) = Me.VisitList(Of BoundAnonymousTypePropertyAccess)(node.Declarations)
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.Arguments)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.BinderOpt, boundAnonymousTypePropertyAccesses, boundExpressions, typeSymbol)
		End Function

		Public Overrides Function VisitAnonymousTypeFieldInitializer(ByVal node As BoundAnonymousTypeFieldInitializer) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Binder, boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitAnonymousTypePropertyAccess(ByVal node As BoundAnonymousTypePropertyAccess) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Binder, node.PropertyIndex, typeSymbol)
		End Function

		Public Overrides Function VisitArrayAccess(ByVal node As BoundArrayAccess) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(node.Indices)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundExpressions, node.IsLValue, typeSymbol)
		End Function

		Public Overrides Function VisitArrayCreation(ByVal node As BoundArrayCreation) As BoundNode
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.Bounds)
			Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = DirectCast(Me.Visit(node.InitializerOpt), Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization)
			Dim arrayLiteralOpt As BoundArrayLiteral = node.ArrayLiteralOpt
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.IsParamArrayArgument, boundExpressions, boundArrayInitialization, arrayLiteralOpt, node.ArrayLiteralConversion, typeSymbol)
		End Function

		Public Overrides Function VisitArrayInitialization(ByVal node As BoundArrayInitialization) As BoundNode
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.Initializers)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpressions, typeSymbol)
		End Function

		Public Overrides Function VisitArrayLength(ByVal node As BoundArrayLength) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitArrayLiteral(ByVal node As BoundArrayLiteral) As BoundNode
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.Bounds)
			Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = DirectCast(Me.Visit(node.Initializer), Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization)
			Me.VisitType(node.Type)
			Return node.Update(node.HasDominantType, node.NumberOfCandidates, node.InferredType, boundExpressions, boundArrayInitialization, node.Binder)
		End Function

		Public Overrides Function VisitAsNewLocalDeclarations(ByVal node As BoundAsNewLocalDeclarations) As BoundNode
			Dim boundLocalDeclarations As ImmutableArray(Of BoundLocalDeclaration) = Me.VisitList(Of BoundLocalDeclaration)(node.LocalDeclarations)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Initializer), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(boundLocalDeclarations, boundExpression)
		End Function

		Public Overrides Function VisitAssignmentOperator(ByVal node As BoundAssignmentOperator) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Left), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundCompoundAssignmentTargetPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundCompoundAssignmentTargetPlaceholder = DirectCast(Me.Visit(node.LeftOnTheRightOpt), Microsoft.CodeAnalysis.VisualBasic.BoundCompoundAssignmentTargetPlaceholder)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Right), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundCompoundAssignmentTargetPlaceholder, boundExpression1, node.SuppressObjectClone, typeSymbol)
		End Function

		Public Overrides Function VisitAttribute(ByVal node As BoundAttribute) As BoundNode
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.ConstructorArguments)
			Dim boundExpressions1 As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.NamedArguments)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Constructor, boundExpressions, boundExpressions1, node.ResultKind, typeSymbol)
		End Function

		Public Overrides Function VisitAwaitOperator(ByVal node As BoundAwaitOperator) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Operand), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = DirectCast(Me.Visit(node.AwaitableInstancePlaceholder), Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.GetAwaiter), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundLValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundLValuePlaceholder = DirectCast(Me.Visit(node.AwaiterInstancePlaceholder), Microsoft.CodeAnalysis.VisualBasic.BoundLValuePlaceholder)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.IsCompleted), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.GetResult), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundRValuePlaceholder, boundExpression1, boundLValuePlaceholder, boundExpression2, boundExpression3, typeSymbol)
		End Function

		Public Overrides Function VisitBadExpression(ByVal node As BoundBadExpression) As BoundNode
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.ChildBoundNodes)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.ResultKind, node.Symbols, boundExpressions, typeSymbol)
		End Function

		Public Overrides Function VisitBadStatement(ByVal node As BoundBadStatement) As BoundNode
			Return node.Update(Me.VisitList(Of BoundNode)(node.ChildBoundNodes))
		End Function

		Public Overrides Function VisitBadVariable(ByVal node As BoundBadVariable) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.IsLValue, typeSymbol)
		End Function

		Public Overrides Function VisitBinaryConditionalExpression(ByVal node As BoundBinaryConditionalExpression) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.TestExpression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim convertedTestExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ConvertedTestExpression
			Dim testExpressionPlaceholder As BoundRValuePlaceholder = node.TestExpressionPlaceholder
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ElseExpression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, convertedTestExpression, testExpressionPlaceholder, boundExpression1, node.ConstantValueOpt, typeSymbol)
		End Function

		Public Overrides Function VisitBinaryOperator(ByVal node As BoundBinaryOperator) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Left), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Right), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.OperatorKind, boundExpression, boundExpression1, node.Checked, node.ConstantValueOpt, typeSymbol)
		End Function

		Public Overrides Function VisitBlock(ByVal node As BoundBlock) As BoundNode
			Dim boundStatements As ImmutableArray(Of BoundStatement) = Me.VisitList(Of BoundStatement)(node.Statements)
			Return node.Update(node.StatementListSyntax, node.Locals, boundStatements)
		End Function

		Public Overrides Function VisitByRefArgumentPlaceholder(ByVal node As BoundByRefArgumentPlaceholder) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.IsOut, typeSymbol)
		End Function

		Public Overrides Function VisitByRefArgumentWithCopyBack(ByVal node As BoundByRefArgumentWithCopyBack) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.OriginalArgument), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.InConversion), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundByRefArgumentPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundByRefArgumentPlaceholder = DirectCast(Me.Visit(node.InPlaceholder), Microsoft.CodeAnalysis.VisualBasic.BoundByRefArgumentPlaceholder)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.OutConversion), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = DirectCast(Me.Visit(node.OutPlaceholder), Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundExpression1, boundByRefArgumentPlaceholder, boundExpression2, boundRValuePlaceholder, typeSymbol)
		End Function

		Public Overrides Function VisitCall(ByVal node As BoundCall) As BoundNode
			Dim methodGroupOpt As BoundMethodGroup = node.MethodGroupOpt
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReceiverOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(node.Arguments)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Method, methodGroupOpt, boundExpression, boundExpressions, node.DefaultArguments, node.ConstantValueOpt, node.IsLValue, node.SuppressObjectClone, typeSymbol)
		End Function

		Public Overrides Function VisitCaseBlock(ByVal node As BoundCaseBlock) As BoundNode
			Dim boundCaseStatement As Microsoft.CodeAnalysis.VisualBasic.BoundCaseStatement = DirectCast(Me.Visit(node.CaseStatement), Microsoft.CodeAnalysis.VisualBasic.BoundCaseStatement)
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			Return node.Update(boundCaseStatement, boundBlock)
		End Function

		Public Overrides Function VisitCaseStatement(ByVal node As BoundCaseStatement) As BoundNode
			Dim boundCaseClauses As ImmutableArray(Of BoundCaseClause) = Me.VisitList(Of BoundCaseClause)(node.CaseClauses)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ConditionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(boundCaseClauses, boundExpression)
		End Function

		Public Overrides Function VisitCatchBlock(ByVal node As BoundCatchBlock) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ExceptionSourceOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ErrorLineNumberOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ExceptionFilterOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			Return node.Update(node.LocalOpt, boundExpression, boundExpression1, boundExpression2, boundBlock, node.IsSynthesizedAsyncCatchAll)
		End Function

		Public Overrides Function VisitCollectionInitializerExpression(ByVal node As BoundCollectionInitializerExpression) As BoundNode
			Dim boundWithLValueExpressionPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundWithLValueExpressionPlaceholder = DirectCast(Me.Visit(node.PlaceholderOpt), Microsoft.CodeAnalysis.VisualBasic.BoundWithLValueExpressionPlaceholder)
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.Initializers)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundWithLValueExpressionPlaceholder, boundExpressions, typeSymbol)
		End Function

		Public Overrides Function VisitComplexConditionalAccessReceiver(ByVal node As BoundComplexConditionalAccessReceiver) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ValueTypeReceiver), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReferenceTypeReceiver), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundExpression1, typeSymbol)
		End Function

		Public Overrides Function VisitCompoundAssignmentTargetPlaceholder(ByVal node As BoundCompoundAssignmentTargetPlaceholder) As BoundNode
			Return node.Update(Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitConditionalAccess(ByVal node As BoundConditionalAccess) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Receiver), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = DirectCast(Me.Visit(node.Placeholder), Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.AccessExpression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundRValuePlaceholder, boundExpression1, typeSymbol)
		End Function

		Public Overrides Function VisitConditionalAccessReceiverPlaceholder(ByVal node As BoundConditionalAccessReceiverPlaceholder) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.PlaceholderId, typeSymbol)
		End Function

		Public Overrides Function VisitConditionalGoto(ByVal node As BoundConditionalGoto) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(boundExpression, node.JumpIfTrue, node.Label)
		End Function

		Public Overrides Function VisitContinueStatement(ByVal node As BoundContinueStatement) As BoundNode
			Return node
		End Function

		Public Overrides Function VisitConversion(ByVal node As BoundConversion) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Operand), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExtendedConversionInfo As Microsoft.CodeAnalysis.VisualBasic.BoundExtendedConversionInfo = DirectCast(Me.Visit(node.ExtendedInfoOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExtendedConversionInfo)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.ConversionKind, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, boundExtendedConversionInfo, typeSymbol)
		End Function

		Public Overrides Function VisitConvertedTupleElements(ByVal node As BoundConvertedTupleElements) As BoundNode
			Dim boundRValuePlaceholders As ImmutableArray(Of BoundRValuePlaceholder) = Me.VisitList(Of BoundRValuePlaceholder)(node.ElementPlaceholders)
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.ConvertedElements)
			Return node.Update(boundRValuePlaceholders, boundExpressions)
		End Function

		Public Overrides Function VisitConvertedTupleLiteral(ByVal node As BoundConvertedTupleLiteral) As BoundNode
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.Arguments)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.NaturalTypeOpt)
			Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(typeSymbol, boundExpressions, typeSymbol1)
		End Function

		Public Overrides Function VisitDelegateCreationExpression(ByVal node As BoundDelegateCreationExpression) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReceiverOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = DirectCast(Me.Visit(node.RelaxationLambdaOpt), Microsoft.CodeAnalysis.VisualBasic.BoundLambda)
			Dim boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = DirectCast(Me.Visit(node.RelaxationReceiverPlaceholderOpt), Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder)
			Dim methodGroupOpt As BoundMethodGroup = node.MethodGroupOpt
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.Method, boundLambda, boundRValuePlaceholder, methodGroupOpt, typeSymbol)
		End Function

		Public Overrides Function VisitDimStatement(ByVal node As BoundDimStatement) As BoundNode
			Dim boundLocalDeclarationBases As ImmutableArray(Of BoundLocalDeclarationBase) = Me.VisitList(Of BoundLocalDeclarationBase)(node.LocalDeclarations)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.InitializerOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(boundLocalDeclarationBases, boundExpression)
		End Function

		Public Overrides Function VisitDirectCast(ByVal node As BoundDirectCast) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Operand), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = DirectCast(Me.Visit(node.RelaxationLambdaOpt), Microsoft.CodeAnalysis.VisualBasic.BoundLambda)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.ConversionKind, node.SuppressVirtualCalls, node.ConstantValueOpt, boundLambda, typeSymbol)
		End Function

		Public Overrides Function VisitDoLoopStatement(ByVal node As BoundDoLoopStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.TopConditionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.BottomConditionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Return node.Update(boundExpression, boundExpression1, node.TopConditionIsUntil, node.BottomConditionIsUntil, boundStatement, node.ContinueLabel, node.ExitLabel)
		End Function

		Public Overrides Function VisitDup(ByVal node As BoundDup) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.IsReference, typeSymbol)
		End Function

		Public Overrides Function VisitEndStatement(ByVal node As BoundEndStatement) As BoundNode
			Return node
		End Function

		Public Overrides Function VisitEraseStatement(ByVal node As BoundEraseStatement) As BoundNode
			Return node.Update(Me.VisitList(Of BoundAssignmentOperator)(node.Clauses))
		End Function

		Public Overrides Function VisitEventAccess(ByVal node As BoundEventAccess) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReceiverOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.EventSymbol, typeSymbol)
		End Function

		Public Overrides Function VisitExitStatement(ByVal node As BoundExitStatement) As BoundNode
			Return node
		End Function

		Public Overrides Function VisitExpressionStatement(ByVal node As BoundExpressionStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(boundExpression)
		End Function

		Public Overrides Function VisitFieldAccess(ByVal node As BoundFieldAccess) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReceiverOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.FieldSymbol, node.IsLValue, node.SuppressVirtualCalls, node.ConstantsInProgressOpt, typeSymbol)
		End Function

		Public Overrides Function VisitFieldInfo(ByVal node As BoundFieldInfo) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Field, typeSymbol)
		End Function

		Public Overrides Function VisitFieldInitializer(ByVal node As BoundFieldInitializer) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.MemberAccessExpressionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.InitialValue), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(node.InitializedFields, boundExpression, boundExpression1)
		End Function

		Public Overrides Function VisitForEachStatement(ByVal node As BoundForEachStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Collection), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ControlVariable), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(node.NextVariablesOpt)
			Return node.Update(boundExpression, node.EnumeratorInfo, node.DeclaredOrInferredLocalOpt, boundExpression1, boundStatement, boundExpressions, node.ContinueLabel, node.ExitLabel)
		End Function

		Public Overrides Function VisitForToStatement(ByVal node As BoundForToStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.InitialValue), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.LimitValue), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.StepValue), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundForToUserDefinedOperator As BoundForToUserDefinedOperators = DirectCast(Me.Visit(node.OperatorsOpt), BoundForToUserDefinedOperators)
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ControlVariable), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(node.NextVariablesOpt)
			Return node.Update(boundExpression, boundExpression1, boundExpression2, node.Checked, boundForToUserDefinedOperator, node.DeclaredOrInferredLocalOpt, boundExpression3, boundStatement, boundExpressions, node.ContinueLabel, node.ExitLabel)
		End Function

		Public Overrides Function VisitForToUserDefinedOperators(ByVal node As BoundForToUserDefinedOperators) As BoundNode
			Dim boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = DirectCast(Me.Visit(node.LeftOperandPlaceholder), Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder)
			Dim boundRValuePlaceholder1 As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = DirectCast(Me.Visit(node.RightOperandPlaceholder), Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder)
			Dim boundUserDefinedBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator = DirectCast(Me.Visit(node.Addition), Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator)
			Dim boundUserDefinedBinaryOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator = DirectCast(Me.Visit(node.Subtraction), Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.LessThanOrEqual), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.GreaterThanOrEqual), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(boundRValuePlaceholder, boundRValuePlaceholder1, boundUserDefinedBinaryOperator, boundUserDefinedBinaryOperator1, boundExpression, boundExpression1)
		End Function

		Public Overrides Function VisitGetType(ByVal node As BoundGetType) As BoundNode
			Dim boundTypeExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression = DirectCast(Me.Visit(node.SourceType), Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundTypeExpression, typeSymbol)
		End Function

		Public Overrides Function VisitGlobalStatementInitializer(ByVal node As BoundGlobalStatementInitializer) As BoundNode
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.Statement), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Return node.Update(boundStatement)
		End Function

		Public Overrides Function VisitGotoStatement(ByVal node As BoundGotoStatement) As BoundNode
			Dim boundLabel As Microsoft.CodeAnalysis.VisualBasic.BoundLabel = DirectCast(Me.Visit(node.LabelExpressionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundLabel)
			Return node.Update(node.Label, boundLabel)
		End Function

		Public Overrides Function VisitGroupAggregation(ByVal node As BoundGroupAggregation) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Group), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitGroupTypeInferenceLambda(ByVal node As GroupTypeInferenceLambda) As BoundNode
			Me.VisitType(node.Type)
			Return node.Update(node.Binder, node.Parameters, node.Compilation)
		End Function

		Public Overrides Function VisitHostObjectMemberReference(ByVal node As BoundHostObjectMemberReference) As BoundNode
			Return node.Update(Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitIfStatement(ByVal node As BoundIfStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.Consequence), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.AlternativeOpt), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Return node.Update(boundExpression, boundStatement, boundStatement1)
		End Function

		Public Overrides Function VisitInitializer(ByVal node As BoundInitializer) As BoundNode
			Return node
		End Function

		Public Overrides Function VisitInstrumentationPayloadRoot(ByVal node As BoundInstrumentationPayloadRoot) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.AnalysisKind, node.IsLValue, typeSymbol)
		End Function

		Public Overrides Function VisitInterpolatedStringExpression(ByVal node As BoundInterpolatedStringExpression) As BoundNode
			Dim boundNodes As ImmutableArray(Of BoundNode) = Me.VisitList(Of BoundNode)(node.Contents)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundNodes, node.Binder, typeSymbol)
		End Function

		Public Overrides Function VisitInterpolation(ByVal node As BoundInterpolation) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.AlignmentOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = DirectCast(Me.Visit(node.FormatStringOpt), Microsoft.CodeAnalysis.VisualBasic.BoundLiteral)
			Return node.Update(boundExpression, boundExpression1, boundLiteral)
		End Function

		Public Overrides Function VisitLabel(ByVal node As BoundLabel) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Label, typeSymbol)
		End Function

		Public Overrides Function VisitLabelStatement(ByVal node As BoundLabelStatement) As BoundNode
			Return node
		End Function

		Public Overrides Function VisitLambda(ByVal node As BoundLambda) As BoundNode
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			Me.VisitType(node.Type)
			Return node.Update(node.LambdaSymbol, boundBlock, node.Diagnostics, node.LambdaBinderOpt, node.DelegateRelaxation, node.MethodConversionKind)
		End Function

		Public Overrides Function VisitLateAddressOfOperator(ByVal node As BoundLateAddressOfOperator) As BoundNode
			Dim boundLateMemberAccess As Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess = DirectCast(Me.Visit(node.MemberAccess), Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Binder, boundLateMemberAccess, typeSymbol)
		End Function

		Public Overrides Function VisitLateBoundArgumentSupportingAssignmentWithCapture(ByVal node As BoundLateBoundArgumentSupportingAssignmentWithCapture) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.OriginalArgument), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.LocalSymbol, typeSymbol)
		End Function

		Public Overrides Function VisitLateInvocation(ByVal node As BoundLateInvocation) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Member), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(node.ArgumentsOpt)
			Dim methodOrPropertyGroupOpt As BoundMethodOrPropertyGroup = node.MethodOrPropertyGroupOpt
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundExpressions, node.ArgumentNamesOpt, node.AccessKind, methodOrPropertyGroupOpt, typeSymbol)
		End Function

		Public Overrides Function VisitLateMemberAccess(ByVal node As BoundLateMemberAccess) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReceiverOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundTypeArgument As BoundTypeArguments = DirectCast(Me.Visit(node.TypeArgumentsOpt), BoundTypeArguments)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.ContainerTypeOpt)
			Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.NameOpt, typeSymbol, boundExpression, boundTypeArgument, node.AccessKind, typeSymbol1)
		End Function

		Public Overridable Function VisitList(Of T As Microsoft.CodeAnalysis.VisualBasic.BoundNode)(ByVal list As ImmutableArray(Of T)) As ImmutableArray(Of T)
			Dim instance As ArrayBuilder(Of T) = Nothing
			Dim num As Integer = 0
			Dim num1 As Integer = If(list.IsDefault, 0, list.Length)
			While num < num1
				Dim item As T = list(num)
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Me.Visit(DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.BoundNode))
				If (item <> boundNode AndAlso instance Is Nothing) Then
					instance = ArrayBuilder(Of T).GetInstance()
					If (num > 0) Then
						instance.AddRange(list, num)
					End If
				End If
				If (instance IsNot Nothing AndAlso boundNode IsNot Nothing) Then
					instance.Add(DirectCast(boundNode, T))
				End If
				num = num + 1
			End While
			Return If(instance Is Nothing, list, instance.ToImmutableAndFree())
		End Function

		Public Sub VisitList(Of T As BoundNode)(ByVal list As ImmutableArray(Of T), ByVal results As ArrayBuilder(Of T))
			Dim enumerator As ImmutableArray(Of T).Enumerator = list.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As T = enumerator.Current
				results.Add(DirectCast(Me.Visit(DirectCast(current, BoundNode)), T))
			End While
		End Sub

		Public Overrides Function VisitLiteral(ByVal node As BoundLiteral) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Value, typeSymbol)
		End Function

		Public Overrides Function VisitLocal(ByVal node As BoundLocal) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.LocalSymbol, node.IsLValue, typeSymbol)
		End Function

		Public Overrides Function VisitLocalDeclaration(ByVal node As BoundLocalDeclaration) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.DeclarationInitializerOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundArrayCreation As Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation = DirectCast(Me.Visit(node.IdentifierInitializerOpt), Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation)
			Return node.Update(node.LocalSymbol, boundExpression, boundArrayCreation, node.InitializedByAsNew)
		End Function

		Public Overrides Function VisitLoweredConditionalAccess(ByVal node As BoundLoweredConditionalAccess) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReceiverOrCondition), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.WhenNotNull), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.WhenNullOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.CaptureReceiver, node.PlaceholderId, boundExpression1, boundExpression2, typeSymbol)
		End Function

		Public Overrides Function VisitLValuePlaceholder(ByVal node As BoundLValuePlaceholder) As BoundNode
			Return node.Update(Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitLValueToRValueWrapper(ByVal node As BoundLValueToRValueWrapper) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.UnderlyingLValue), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitMaximumMethodDefIndex(ByVal node As BoundMaximumMethodDefIndex) As BoundNode
			Return node.Update(Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitMeReference(ByVal node As BoundMeReference) As BoundNode
			Return node.Update(Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitMethodDefIndex(ByVal node As BoundMethodDefIndex) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Method, typeSymbol)
		End Function

		Public Overrides Function VisitMethodGroup(ByVal node As BoundMethodGroup) As BoundNode
			Dim boundTypeArgument As BoundTypeArguments = DirectCast(Me.Visit(node.TypeArgumentsOpt), BoundTypeArguments)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReceiverOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Me.VisitType(node.Type)
			Return node.Update(boundTypeArgument, node.Methods, node.PendingExtensionMethodsOpt, node.ResultKind, boundExpression, node.QualificationKind)
		End Function

		Public Overrides Function VisitMethodInfo(ByVal node As BoundMethodInfo) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Method, typeSymbol)
		End Function

		Public Overrides Function VisitMidResult(ByVal node As BoundMidResult) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Original), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Start), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.LengthOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Source), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundExpression1, boundExpression2, boundExpression3, typeSymbol)
		End Function

		Public Overrides Function VisitModuleVersionId(ByVal node As BoundModuleVersionId) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.IsLValue, typeSymbol)
		End Function

		Public Overrides Function VisitModuleVersionIdString(ByVal node As BoundModuleVersionIdString) As BoundNode
			Return node.Update(Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitMyBaseReference(ByVal node As BoundMyBaseReference) As BoundNode
			Return node.Update(Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitMyClassReference(ByVal node As BoundMyClassReference) As BoundNode
			Return node.Update(Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitNameOfOperator(ByVal node As BoundNameOfOperator) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Argument), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.ConstantValueOpt, typeSymbol)
		End Function

		Public Overrides Function VisitNamespaceExpression(ByVal node As BoundNamespaceExpression) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.UnevaluatedReceiverOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.AliasOpt, node.NamespaceSymbol)
		End Function

		Public Overrides Function VisitNewT(ByVal node As BoundNewT) As BoundNode
			Dim boundObjectInitializerExpressionBase As Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpressionBase = DirectCast(Me.Visit(node.InitializerOpt), Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpressionBase)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundObjectInitializerExpressionBase, typeSymbol)
		End Function

		Public Overrides Function VisitNoOpStatement(ByVal node As BoundNoOpStatement) As BoundNode
			Return node
		End Function

		Public Overrides Function VisitNoPiaObjectCreationExpression(ByVal node As BoundNoPiaObjectCreationExpression) As BoundNode
			Dim boundObjectInitializerExpressionBase As Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpressionBase = DirectCast(Me.Visit(node.InitializerOpt), Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpressionBase)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.GuidString, boundObjectInitializerExpressionBase, typeSymbol)
		End Function

		Public Overrides Function VisitNullableIsTrueOperator(ByVal node As BoundNullableIsTrueOperator) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Operand), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitObjectCreationExpression(ByVal node As BoundObjectCreationExpression) As BoundNode
			Dim methodGroupOpt As BoundMethodGroup = node.MethodGroupOpt
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.Arguments)
			Dim boundObjectInitializerExpressionBase As Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpressionBase = DirectCast(Me.Visit(node.InitializerOpt), Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpressionBase)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.ConstructorOpt, methodGroupOpt, boundExpressions, node.DefaultArguments, boundObjectInitializerExpressionBase, typeSymbol)
		End Function

		Public Overrides Function VisitObjectInitializerExpression(ByVal node As BoundObjectInitializerExpression) As BoundNode
			Dim boundWithLValueExpressionPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundWithLValueExpressionPlaceholder = DirectCast(Me.Visit(node.PlaceholderOpt), Microsoft.CodeAnalysis.VisualBasic.BoundWithLValueExpressionPlaceholder)
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.Initializers)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.CreateTemporaryLocalForInitialization, node.Binder, boundWithLValueExpressionPlaceholder, boundExpressions, typeSymbol)
		End Function

		Public Overrides Function VisitOmittedArgument(ByVal node As BoundOmittedArgument) As BoundNode
			Return node.Update(Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitOnErrorStatement(ByVal node As BoundOnErrorStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.LabelExpressionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(node.OnErrorKind, node.LabelOpt, boundExpression)
		End Function

		Public Overrides Function VisitOrdering(ByVal node As BoundOrdering) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.UnderlyingExpression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitParameter(ByVal node As BoundParameter) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.ParameterSymbol, node.IsLValue, node.SuppressVirtualCalls, typeSymbol)
		End Function

		Public Overrides Function VisitParameterEqualsValue(ByVal node As BoundParameterEqualsValue) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(node.Parameter, boundExpression)
		End Function

		Public Overrides Function VisitParenthesized(ByVal node As BoundParenthesized) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitPreviousSubmissionReference(ByVal node As BoundPreviousSubmissionReference) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.SourceType, typeSymbol)
		End Function

		Public Overrides Function VisitPropertyAccess(ByVal node As BoundPropertyAccess) As BoundNode
			Dim propertyGroupOpt As BoundPropertyGroup = node.PropertyGroupOpt
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReceiverOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(node.Arguments)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.PropertySymbol, propertyGroupOpt, node.AccessKind, node.IsWriteable, node.IsLValue, boundExpression, boundExpressions, node.DefaultArguments, typeSymbol)
		End Function

		Public Overrides Function VisitPropertyGroup(ByVal node As BoundPropertyGroup) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReceiverOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Me.VisitType(node.Type)
			Return node.Update(node.Properties, node.ResultKind, boundExpression, node.QualificationKind)
		End Function

		Public Overrides Function VisitPropertyInitializer(ByVal node As BoundPropertyInitializer) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.MemberAccessExpressionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.InitialValue), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(node.InitializedProperties, boundExpression, boundExpression1)
		End Function

		Public Overrides Function VisitPseudoVariable(ByVal node As BoundPseudoVariable) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.LocalSymbol, node.IsLValue, node.EmitExpressions, typeSymbol)
		End Function

		Public Overrides Function VisitQueryableSource(ByVal node As BoundQueryableSource) As BoundNode
			Dim boundQueryPart As Microsoft.CodeAnalysis.VisualBasic.BoundQueryPart = DirectCast(Me.Visit(node.Source), Microsoft.CodeAnalysis.VisualBasic.BoundQueryPart)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.CompoundVariableType)
			Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundQueryPart, node.RangeVariableOpt, node.RangeVariables, typeSymbol, node.Binders, typeSymbol1)
		End Function

		Public Overrides Function VisitQueryClause(ByVal node As BoundQueryClause) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.UnderlyingExpression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.CompoundVariableType)
			Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.RangeVariables, typeSymbol, node.Binders, typeSymbol1)
		End Function

		Public Overrides Function VisitQueryExpression(ByVal node As BoundQueryExpression) As BoundNode
			Dim boundQueryClauseBase As Microsoft.CodeAnalysis.VisualBasic.BoundQueryClauseBase = DirectCast(Me.Visit(node.LastOperator), Microsoft.CodeAnalysis.VisualBasic.BoundQueryClauseBase)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundQueryClauseBase, typeSymbol)
		End Function

		Public Overrides Function VisitQueryLambda(ByVal node As BoundQueryLambda) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Me.VisitType(node.Type)
			Return node.Update(node.LambdaSymbol, node.RangeVariables, boundExpression, node.ExprIsOperandOfConditionalBranch)
		End Function

		Public Overrides Function VisitQuerySource(ByVal node As BoundQuerySource) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitRaiseEventStatement(ByVal node As BoundRaiseEventStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.EventInvocation), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(node.EventSymbol, boundExpression)
		End Function

		Public Overrides Function VisitRangeCaseClause(ByVal node As BoundRangeCaseClause) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.LowerBoundOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.UpperBoundOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.LowerBoundConditionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.UpperBoundConditionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(boundExpression, boundExpression1, boundExpression2, boundExpression3)
		End Function

		Public Overrides Function VisitRangeVariable(ByVal node As BoundRangeVariable) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.RangeVariable, typeSymbol)
		End Function

		Public Overrides Function VisitRangeVariableAssignment(ByVal node As BoundRangeVariableAssignment) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.RangeVariable, boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitRedimClause(ByVal node As BoundRedimClause) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Operand), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(node.Indices)
			Return node.Update(boundExpression, boundExpressions, node.ArrayTypeOpt, node.Preserve)
		End Function

		Public Overrides Function VisitRedimStatement(ByVal node As BoundRedimStatement) As BoundNode
			Return node.Update(Me.VisitList(Of BoundRedimClause)(node.Clauses))
		End Function

		Public Overrides Function VisitReferenceAssignment(ByVal node As BoundReferenceAssignment) As BoundNode
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = DirectCast(Me.Visit(node.ByRefLocal), Microsoft.CodeAnalysis.VisualBasic.BoundLocal)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.LValue), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundLocal, boundExpression, node.IsLValue, typeSymbol)
		End Function

		Public Overrides Function VisitRelationalCaseClause(ByVal node As BoundRelationalCaseClause) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ValueOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ConditionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(node.OperatorKind, boundExpression, boundExpression1)
		End Function

		Public Overrides Function VisitRelaxationLambda(ByVal node As BoundRelaxationLambda) As BoundNode
			Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = DirectCast(Me.Visit(node.Lambda), Microsoft.CodeAnalysis.VisualBasic.BoundLambda)
			Dim boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = DirectCast(Me.Visit(node.ReceiverPlaceholderOpt), Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder)
			Return node.Update(boundLambda, boundRValuePlaceholder)
		End Function

		Public Overrides Function VisitRemoveHandlerStatement(ByVal node As BoundRemoveHandlerStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.EventAccess), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Handler), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(boundExpression, boundExpression1)
		End Function

		Public Overrides Function VisitResumeStatement(ByVal node As BoundResumeStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.LabelExpressionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(node.ResumeKind, node.LabelOpt, boundExpression)
		End Function

		Public Overrides Function VisitReturnStatement(ByVal node As BoundReturnStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ExpressionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(boundExpression, node.FunctionLocalOpt, node.ExitLabelOpt)
		End Function

		Public Overrides Function VisitRValuePlaceholder(ByVal node As BoundRValuePlaceholder) As BoundNode
			Return node.Update(Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitSelectStatement(ByVal node As BoundSelectStatement) As BoundNode
			Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = DirectCast(Me.Visit(node.ExpressionStatement), Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement)
			Dim boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = DirectCast(Me.Visit(node.ExprPlaceholderOpt), Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder)
			Dim boundCaseBlocks As ImmutableArray(Of BoundCaseBlock) = Me.VisitList(Of BoundCaseBlock)(node.CaseBlocks)
			Return node.Update(boundExpressionStatement, boundRValuePlaceholder, boundCaseBlocks, node.RecommendSwitchTable, node.ExitLabel)
		End Function

		Public Overrides Function VisitSequence(ByVal node As BoundSequence) As BoundNode
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(node.SideEffects)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ValueOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Locals, boundExpressions, boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitSequencePoint(ByVal node As BoundSequencePoint) As BoundNode
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.StatementOpt), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Return node.Update(boundStatement)
		End Function

		Public Overrides Function VisitSequencePointExpression(ByVal node As BoundSequencePointExpression) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitSequencePointWithSpan(ByVal node As BoundSequencePointWithSpan) As BoundNode
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.StatementOpt), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Return node.Update(boundStatement, node.Span)
		End Function

		Public Overrides Function VisitSimpleCaseClause(ByVal node As BoundSimpleCaseClause) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ValueOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ConditionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(boundExpression, boundExpression1)
		End Function

		Public Overrides Function VisitSourceDocumentIndex(ByVal node As BoundSourceDocumentIndex) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Document, typeSymbol)
		End Function

		Public Overrides Function VisitSpillSequence(ByVal node As BoundSpillSequence) As BoundNode
			Dim boundStatements As ImmutableArray(Of BoundStatement) = Me.VisitList(Of BoundStatement)(node.Statements)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ValueOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Locals, node.SpillFields, boundStatements, boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitStateMachineScope(ByVal node As BoundStateMachineScope) As BoundNode
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.Statement), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Return node.Update(node.Fields, boundStatement)
		End Function

		Public Overrides Function VisitStatementList(ByVal node As BoundStatementList) As BoundNode
			Return node.Update(Me.VisitList(Of BoundStatement)(node.Statements))
		End Function

		Public Overrides Function VisitStopStatement(ByVal node As BoundStopStatement) As BoundNode
			Return node
		End Function

		Public Overrides Function VisitSyncLockStatement(ByVal node As BoundSyncLockStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.LockExpression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			Return node.Update(boundExpression, boundBlock)
		End Function

		Public Overrides Function VisitTernaryConditionalExpression(ByVal node As BoundTernaryConditionalExpression) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.WhenTrue), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.WhenFalse), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundExpression1, boundExpression2, node.ConstantValueOpt, typeSymbol)
		End Function

		Public Overrides Function VisitThrowStatement(ByVal node As BoundThrowStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ExpressionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(boundExpression)
		End Function

		Public Overrides Function VisitToQueryableCollectionConversion(ByVal node As BoundToQueryableCollectionConversion) As BoundNode
			Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = DirectCast(Me.Visit(node.ConversionCall), Microsoft.CodeAnalysis.VisualBasic.BoundCall)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundCall, typeSymbol)
		End Function

		Public Overrides Function VisitTryCast(ByVal node As BoundTryCast) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Operand), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = DirectCast(Me.Visit(node.RelaxationLambdaOpt), Microsoft.CodeAnalysis.VisualBasic.BoundLambda)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.ConversionKind, node.ConstantValueOpt, boundLambda, typeSymbol)
		End Function

		Public Overrides Function VisitTryStatement(ByVal node As BoundTryStatement) As BoundNode
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.TryBlock), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			Dim boundCatchBlocks As ImmutableArray(Of BoundCatchBlock) = Me.VisitList(Of BoundCatchBlock)(node.CatchBlocks)
			Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.FinallyBlockOpt), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			Return node.Update(boundBlock, boundCatchBlocks, boundBlock1, node.ExitLabelOpt)
		End Function

		Public Overrides Function VisitTupleLiteral(ByVal node As BoundTupleLiteral) As BoundNode
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.Arguments)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.InferredType, node.ArgumentNamesOpt, node.InferredNamesOpt, boundExpressions, typeSymbol)
		End Function

		Public Overridable Function VisitType(ByVal type As TypeSymbol) As TypeSymbol
			Return type
		End Function

		Public Overrides Function VisitTypeArguments(ByVal node As BoundTypeArguments) As BoundNode
			Me.VisitType(node.Type)
			Return node.Update(node.Arguments)
		End Function

		Public Overrides Function VisitTypeAsValueExpression(ByVal node As BoundTypeAsValueExpression) As BoundNode
			Dim boundTypeExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundTypeExpression, typeSymbol)
		End Function

		Public Overrides Function VisitTypeExpression(ByVal node As BoundTypeExpression) As BoundNode
			Dim unevaluatedReceiverOpt As BoundExpression = node.UnevaluatedReceiverOpt
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(unevaluatedReceiverOpt, node.AliasOpt, typeSymbol)
		End Function

		Public Overrides Function VisitTypeOf(ByVal node As BoundTypeOf) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Operand), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.TargetType)
			Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.IsTypeOfIsNotExpression, typeSymbol, typeSymbol1)
		End Function

		Public Overrides Function VisitTypeOrValueExpression(ByVal node As BoundTypeOrValueExpression) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.Data, typeSymbol)
		End Function

		Public Overrides Function VisitUnaryOperator(ByVal node As BoundUnaryOperator) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Operand), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.OperatorKind, boundExpression, node.Checked, node.ConstantValueOpt, typeSymbol)
		End Function

		Public Overrides Function VisitUnboundLambda(ByVal node As UnboundLambda) As BoundNode
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.ReturnType)
			Me.VisitType(node.Type)
			Return node.Update(node.Binder, node.Flags, node.Parameters, typeSymbol, node.BindingCache)
		End Function

		Public Overrides Function VisitUnstructuredExceptionHandlingCatchFilter(ByVal node As BoundUnstructuredExceptionHandlingCatchFilter) As BoundNode
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = DirectCast(Me.Visit(node.ActiveHandlerLocal), Microsoft.CodeAnalysis.VisualBasic.BoundLocal)
			Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = DirectCast(Me.Visit(node.ResumeTargetLocal), Microsoft.CodeAnalysis.VisualBasic.BoundLocal)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundLocal, boundLocal1, typeSymbol)
		End Function

		Public Overrides Function VisitUnstructuredExceptionHandlingStatement(ByVal node As BoundUnstructuredExceptionHandlingStatement) As BoundNode
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			Return node.Update(node.ContainsOnError, node.ContainsResume, node.ResumeWithoutLabelOpt, node.TrackLineNumber, boundBlock)
		End Function

		Public Overrides Function VisitUnstructuredExceptionOnErrorSwitch(ByVal node As BoundUnstructuredExceptionOnErrorSwitch) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundGotoStatements As ImmutableArray(Of BoundGotoStatement) = Me.VisitList(Of BoundGotoStatement)(node.Jumps)
			Return node.Update(boundExpression, boundGotoStatements)
		End Function

		Public Overrides Function VisitUnstructuredExceptionResumeSwitch(ByVal node As BoundUnstructuredExceptionResumeSwitch) As BoundNode
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = DirectCast(Me.Visit(node.ResumeTargetTemporary), Microsoft.CodeAnalysis.VisualBasic.BoundLocal)
			Dim boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = DirectCast(Me.Visit(node.ResumeLabel), Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement)
			Dim boundLabelStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = DirectCast(Me.Visit(node.ResumeNextLabel), Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement)
			Dim boundGotoStatements As ImmutableArray(Of BoundGotoStatement) = Me.VisitList(Of BoundGotoStatement)(node.Jumps)
			Return node.Update(boundLocal, boundLabelStatement, boundLabelStatement1, boundGotoStatements)
		End Function

		Public Overrides Function VisitUserDefinedBinaryOperator(ByVal node As BoundUserDefinedBinaryOperator) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.UnderlyingExpression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.OperatorKind, boundExpression, node.Checked, typeSymbol)
		End Function

		Public Overrides Function VisitUserDefinedConversion(ByVal node As BoundUserDefinedConversion) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.UnderlyingExpression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, node.InOutConversionFlags, typeSymbol)
		End Function

		Public Overrides Function VisitUserDefinedShortCircuitingOperator(ByVal node As BoundUserDefinedShortCircuitingOperator) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.LeftOperand), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = DirectCast(Me.Visit(node.LeftOperandPlaceholder), Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.LeftTest), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundUserDefinedBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator = DirectCast(Me.Visit(node.BitwiseOperator), Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundRValuePlaceholder, boundExpression1, boundUserDefinedBinaryOperator, typeSymbol)
		End Function

		Public Overrides Function VisitUserDefinedUnaryOperator(ByVal node As BoundUserDefinedUnaryOperator) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.UnderlyingExpression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(node.OperatorKind, boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitUsingStatement(ByVal node As BoundUsingStatement) As BoundNode
			Dim boundLocalDeclarationBases As ImmutableArray(Of BoundLocalDeclarationBase) = Me.VisitList(Of BoundLocalDeclarationBase)(node.ResourceList)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ResourceExpressionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			Return node.Update(boundLocalDeclarationBases, boundExpression, boundBlock, node.UsingInfo, node.Locals)
		End Function

		Public Overrides Function VisitValueTypeMeReference(ByVal node As BoundValueTypeMeReference) As BoundNode
			Return node.Update(Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitWhileStatement(ByVal node As BoundWhileStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Return node.Update(boundExpression, boundStatement, node.ContinueLabel, node.ExitLabel)
		End Function

		Public Overrides Function VisitWithLValueExpressionPlaceholder(ByVal node As BoundWithLValueExpressionPlaceholder) As BoundNode
			Return node.Update(Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitWithRValueExpressionPlaceholder(ByVal node As BoundWithRValueExpressionPlaceholder) As BoundNode
			Return node.Update(Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitWithStatement(ByVal node As BoundWithStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.OriginalExpression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			Return node.Update(boundExpression, boundBlock, node.Binder)
		End Function

		Public Overrides Function VisitXmlAttribute(ByVal node As BoundXmlAttribute) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim objectCreation As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ObjectCreation
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundExpression1, node.MatchesImport, objectCreation, typeSymbol)
		End Function

		Public Overrides Function VisitXmlCData(ByVal node As BoundXmlCData) As BoundNode
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.BoundLiteral)
			Dim objectCreation As BoundExpression = node.ObjectCreation
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundLiteral, objectCreation, typeSymbol)
		End Function

		Public Overrides Function VisitXmlComment(ByVal node As BoundXmlComment) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim objectCreation As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ObjectCreation
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, objectCreation, typeSymbol)
		End Function

		Public Overrides Function VisitXmlDeclaration(ByVal node As BoundXmlDeclaration) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Version), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Encoding), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Standalone), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim objectCreation As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ObjectCreation
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundExpression1, boundExpression2, objectCreation, typeSymbol)
		End Function

		Public Overrides Function VisitXmlDocument(ByVal node As BoundXmlDocument) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Declaration), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(node.ChildNodes)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundExpressions, node.RewriterInfo, typeSymbol)
		End Function

		Public Overrides Function VisitXmlElement(ByVal node As BoundXmlElement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Argument), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(node.ChildNodes)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundExpressions, node.RewriterInfo, typeSymbol)
		End Function

		Public Overrides Function VisitXmlEmbeddedExpression(ByVal node As BoundXmlEmbeddedExpression) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitXmlMemberAccess(ByVal node As BoundXmlMemberAccess) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.MemberAccess), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, typeSymbol)
		End Function

		Public Overrides Function VisitXmlName(ByVal node As BoundXmlName) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.XmlNamespace), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.LocalName), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim objectCreation As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ObjectCreation
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundExpression1, objectCreation, typeSymbol)
		End Function

		Public Overrides Function VisitXmlNamespace(ByVal node As BoundXmlNamespace) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.XmlNamespace), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim objectCreation As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ObjectCreation
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, objectCreation, typeSymbol)
		End Function

		Public Overrides Function VisitXmlProcessingInstruction(ByVal node As BoundXmlProcessingInstruction) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Target), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Data), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim objectCreation As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ObjectCreation
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			Return node.Update(boundExpression, boundExpression1, objectCreation, typeSymbol)
		End Function

		Public Overrides Function VisitYieldStatement(ByVal node As BoundYieldStatement) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Expression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(boundExpression)
		End Function
	End Class
End Namespace