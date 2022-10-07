Imports Microsoft.CodeAnalysis
Imports System
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundTreeDumperNodeProducer
		Inherits BoundTreeVisitor(Of Object, TreeDumperNode)
		Private Sub New()
			MyBase.New()
		End Sub

		Public Shared Function MakeTree(ByVal node As BoundNode) As TreeDumperNode
			Return (New BoundTreeDumperNodeProducer()).Visit(node, Nothing)
		End Function

		Public Overrides Function VisitAddHandlerStatement(ByVal node As BoundAddHandlerStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("addHandlerStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("eventAccess", Nothing, New TreeDumperNode() { Me.Visit(node.EventAccess, Nothing) }), New TreeDumperNode("handler", Nothing, New TreeDumperNode() { Me.Visit(node.Handler, Nothing) }) })
		End Function

		Public Overrides Function VisitAddressOfOperator(ByVal node As BoundAddressOfOperator, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("binder", node.Binder, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("withDependencies", node.WithDependencies, Nothing), Nothing, Nothing }
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("methodGroup", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.MethodGroup, Nothing) })
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("addressOfOperator", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitAggregateClause(ByVal node As BoundAggregateClause, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("aggregateClause", Nothing, New TreeDumperNode() { New TreeDumperNode("capturedGroupOpt", Nothing, New TreeDumperNode() { Me.Visit(node.CapturedGroupOpt, Nothing) }), New TreeDumperNode("groupPlaceholderOpt", Nothing, New TreeDumperNode() { Me.Visit(node.GroupPlaceholderOpt, Nothing) }), New TreeDumperNode("underlyingExpression", Nothing, New TreeDumperNode() { Me.Visit(node.UnderlyingExpression, Nothing) }), New TreeDumperNode("rangeVariables", node.RangeVariables, Nothing), New TreeDumperNode("compoundVariableType", node.CompoundVariableType, Nothing), New TreeDumperNode("binders", node.Binders, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitAnonymousTypeCreationExpression(ByVal node As BoundAnonymousTypeCreationExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("anonymousTypeCreationExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("binderOpt", node.BinderOpt, Nothing), New TreeDumperNode("declarations", Nothing, node.Declarations.[Select](Of TreeDumperNode)(Function(x As BoundAnonymousTypePropertyAccess) Me.Visit(x, Nothing))), New TreeDumperNode("arguments", Nothing, node.Arguments.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitAnonymousTypeFieldInitializer(ByVal node As BoundAnonymousTypeFieldInitializer, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("binder", node.Binder, Nothing), Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("value", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Value, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("anonymousTypeFieldInitializer", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitAnonymousTypePropertyAccess(ByVal node As BoundAnonymousTypePropertyAccess, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("anonymousTypePropertyAccess", Nothing, New TreeDumperNode() { New TreeDumperNode("binder", node.Binder, Nothing), New TreeDumperNode("propertyIndex", node.PropertyIndex, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitArrayAccess(ByVal node As BoundArrayAccess, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("arrayAccess", Nothing, New TreeDumperNode() { New TreeDumperNode("expression", Nothing, New TreeDumperNode() { Me.Visit(node.Expression, Nothing) }), New TreeDumperNode("indices", Nothing, node.Indices.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("isLValue", node.IsLValue, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitArrayCreation(ByVal node As BoundArrayCreation, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("isParamArrayArgument", node.IsParamArrayArgument, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("bounds", Nothing, node.Bounds.[Select](Of Microsoft.CodeAnalysis.TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), Nothing, Nothing, Nothing, Nothing }
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("initializerOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.InitializerOpt, Nothing) })
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("arrayLiteralOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ArrayLiteralOpt, Nothing) })
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("arrayLiteralConversion", node.ArrayLiteralConversion, Nothing)
			treeDumperNode(5) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("arrayCreation", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitArrayInitialization(ByVal node As BoundArrayInitialization, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("arrayInitialization", Nothing, New TreeDumperNode() { New TreeDumperNode("initializers", Nothing, node.Initializers.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitArrayLength(ByVal node As BoundArrayLength, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("arrayLength", Nothing, New TreeDumperNode() { New TreeDumperNode("expression", Nothing, New TreeDumperNode() { Me.Visit(node.Expression, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitArrayLiteral(ByVal node As BoundArrayLiteral, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("hasDominantType", node.HasDominantType, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("numberOfCandidates", node.NumberOfCandidates, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("inferredType", node.InferredType, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("bounds", Nothing, node.Bounds.[Select](Of Microsoft.CodeAnalysis.TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), Nothing, Nothing, Nothing }
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("initializer", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Initializer, Nothing) })
			treeDumperNode(5) = New Microsoft.CodeAnalysis.TreeDumperNode("binder", node.Binder, Nothing)
			treeDumperNode(6) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("arrayLiteral", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitAsNewLocalDeclarations(ByVal node As BoundAsNewLocalDeclarations, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("localDeclarations", Nothing, node.LocalDeclarations.[Select](Of Microsoft.CodeAnalysis.TreeDumperNode)(Function(x As BoundLocalDeclaration) Me.Visit(x, Nothing))), Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("initializer", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Initializer, Nothing) })
			Return New Microsoft.CodeAnalysis.TreeDumperNode("asNewLocalDeclarations", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitAssignmentOperator(ByVal node As BoundAssignmentOperator, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("assignmentOperator", Nothing, New TreeDumperNode() { New TreeDumperNode("left", Nothing, New TreeDumperNode() { Me.Visit(node.Left, Nothing) }), New TreeDumperNode("leftOnTheRightOpt", Nothing, New TreeDumperNode() { Me.Visit(node.LeftOnTheRightOpt, Nothing) }), New TreeDumperNode("right", Nothing, New TreeDumperNode() { Me.Visit(node.Right, Nothing) }), New TreeDumperNode("suppressObjectClone", node.SuppressObjectClone, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitAttribute(ByVal node As BoundAttribute, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("attribute", Nothing, New TreeDumperNode() { New TreeDumperNode("constructor", node.Constructor, Nothing), New TreeDumperNode("constructorArguments", Nothing, node.ConstructorArguments.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("namedArguments", Nothing, node.NamedArguments.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("resultKind", node.ResultKind, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitAwaitOperator(ByVal node As BoundAwaitOperator, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("awaitOperator", Nothing, New TreeDumperNode() { New TreeDumperNode("operand", Nothing, New TreeDumperNode() { Me.Visit(node.Operand, Nothing) }), New TreeDumperNode("awaitableInstancePlaceholder", Nothing, New TreeDumperNode() { Me.Visit(node.AwaitableInstancePlaceholder, Nothing) }), New TreeDumperNode("getAwaiter", Nothing, New TreeDumperNode() { Me.Visit(node.GetAwaiter, Nothing) }), New TreeDumperNode("awaiterInstancePlaceholder", Nothing, New TreeDumperNode() { Me.Visit(node.AwaiterInstancePlaceholder, Nothing) }), New TreeDumperNode("isCompleted", Nothing, New TreeDumperNode() { Me.Visit(node.IsCompleted, Nothing) }), New TreeDumperNode("getResult", Nothing, New TreeDumperNode() { Me.Visit(node.GetResult, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitBadExpression(ByVal node As BoundBadExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("badExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("resultKind", node.ResultKind, Nothing), New TreeDumperNode("symbols", node.Symbols, Nothing), New TreeDumperNode("childBoundNodes", Nothing, node.ChildBoundNodes.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitBadStatement(ByVal node As BoundBadStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("badStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("childBoundNodes", Nothing, node.ChildBoundNodes.[Select](Of TreeDumperNode)(Function(x As BoundNode) Me.Visit(x, Nothing))) })
		End Function

		Public Overrides Function VisitBadVariable(ByVal node As BoundBadVariable, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("badVariable", Nothing, New TreeDumperNode() { New TreeDumperNode("expression", Nothing, New TreeDumperNode() { Me.Visit(node.Expression, Nothing) }), New TreeDumperNode("isLValue", node.IsLValue, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitBinaryConditionalExpression(ByVal node As BoundBinaryConditionalExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("binaryConditionalExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("testExpression", Nothing, New TreeDumperNode() { Me.Visit(node.TestExpression, Nothing) }), New TreeDumperNode("convertedTestExpression", Nothing, New TreeDumperNode() { Me.Visit(node.ConvertedTestExpression, Nothing) }), New TreeDumperNode("testExpressionPlaceholder", Nothing, New TreeDumperNode() { Me.Visit(node.TestExpressionPlaceholder, Nothing) }), New TreeDumperNode("elseExpression", Nothing, New TreeDumperNode() { Me.Visit(node.ElseExpression, Nothing) }), New TreeDumperNode("constantValueOpt", node.ConstantValueOpt, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitBinaryOperator(ByVal node As BoundBinaryOperator, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("operatorKind", node.OperatorKind, Nothing), Nothing, Nothing, Nothing, Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("left", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Left, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("right", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Right, Nothing) })
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("checked", node.Checked, Nothing)
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("constantValueOpt", node.ConstantValueOpt, Nothing)
			treeDumperNode(5) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("binaryOperator", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitBlock(ByVal node As BoundBlock, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("block", Nothing, New TreeDumperNode() { New TreeDumperNode("statementListSyntax", node.StatementListSyntax, Nothing), New TreeDumperNode("locals", node.Locals, Nothing), New TreeDumperNode("statements", Nothing, node.Statements.[Select](Of TreeDumperNode)(Function(x As BoundStatement) Me.Visit(x, Nothing))) })
		End Function

		Public Overrides Function VisitByRefArgumentPlaceholder(ByVal node As BoundByRefArgumentPlaceholder, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("byRefArgumentPlaceholder", Nothing, New TreeDumperNode() { New TreeDumperNode("isOut", node.IsOut, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitByRefArgumentWithCopyBack(ByVal node As BoundByRefArgumentWithCopyBack, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("byRefArgumentWithCopyBack", Nothing, New TreeDumperNode() { New TreeDumperNode("originalArgument", Nothing, New TreeDumperNode() { Me.Visit(node.OriginalArgument, Nothing) }), New TreeDumperNode("inConversion", Nothing, New TreeDumperNode() { Me.Visit(node.InConversion, Nothing) }), New TreeDumperNode("inPlaceholder", Nothing, New TreeDumperNode() { Me.Visit(node.InPlaceholder, Nothing) }), New TreeDumperNode("outConversion", Nothing, New TreeDumperNode() { Me.Visit(node.OutConversion, Nothing) }), New TreeDumperNode("outPlaceholder", Nothing, New TreeDumperNode() { Me.Visit(node.OutPlaceholder, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitCall(ByVal node As BoundCall, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("method", node.Method, Nothing), Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("methodGroupOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.MethodGroupOpt, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("receiverOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ReceiverOpt, Nothing) })
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("arguments", Nothing, node.Arguments.[Select](Of Microsoft.CodeAnalysis.TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing)))
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("defaultArguments", node.DefaultArguments, Nothing)
			treeDumperNode(5) = New Microsoft.CodeAnalysis.TreeDumperNode("constantValueOpt", node.ConstantValueOpt, Nothing)
			treeDumperNode(6) = New Microsoft.CodeAnalysis.TreeDumperNode("isLValue", node.IsLValue, Nothing)
			treeDumperNode(7) = New Microsoft.CodeAnalysis.TreeDumperNode("suppressObjectClone", node.SuppressObjectClone, Nothing)
			treeDumperNode(8) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("[call]", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitCaseBlock(ByVal node As BoundCaseBlock, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("caseBlock", Nothing, New TreeDumperNode() { New TreeDumperNode("caseStatement", Nothing, New TreeDumperNode() { Me.Visit(node.CaseStatement, Nothing) }), New TreeDumperNode("body", Nothing, New TreeDumperNode() { Me.Visit(node.Body, Nothing) }) })
		End Function

		Public Overrides Function VisitCaseStatement(ByVal node As BoundCaseStatement, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("caseClauses", Nothing, node.CaseClauses.[Select](Of Microsoft.CodeAnalysis.TreeDumperNode)(Function(x As BoundCaseClause) Me.Visit(x, Nothing))), Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("conditionOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ConditionOpt, Nothing) })
			Return New Microsoft.CodeAnalysis.TreeDumperNode("caseStatement", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitCatchBlock(ByVal node As BoundCatchBlock, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("localOpt", node.LocalOpt, Nothing), Nothing, Nothing, Nothing, Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("exceptionSourceOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ExceptionSourceOpt, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("errorLineNumberOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ErrorLineNumberOpt, Nothing) })
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("exceptionFilterOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ExceptionFilterOpt, Nothing) })
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("body", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Body, Nothing) })
			treeDumperNode(5) = New Microsoft.CodeAnalysis.TreeDumperNode("isSynthesizedAsyncCatchAll", node.IsSynthesizedAsyncCatchAll, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("catchBlock", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitCollectionInitializerExpression(ByVal node As BoundCollectionInitializerExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("collectionInitializerExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("placeholderOpt", Nothing, New TreeDumperNode() { Me.Visit(node.PlaceholderOpt, Nothing) }), New TreeDumperNode("initializers", Nothing, node.Initializers.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitComplexConditionalAccessReceiver(ByVal node As BoundComplexConditionalAccessReceiver, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("complexConditionalAccessReceiver", Nothing, New TreeDumperNode() { New TreeDumperNode("valueTypeReceiver", Nothing, New TreeDumperNode() { Me.Visit(node.ValueTypeReceiver, Nothing) }), New TreeDumperNode("referenceTypeReceiver", Nothing, New TreeDumperNode() { Me.Visit(node.ReferenceTypeReceiver, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitCompoundAssignmentTargetPlaceholder(ByVal node As BoundCompoundAssignmentTargetPlaceholder, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("compoundAssignmentTargetPlaceholder", Nothing, New TreeDumperNode() { New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitConditionalAccess(ByVal node As BoundConditionalAccess, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("conditionalAccess", Nothing, New TreeDumperNode() { New TreeDumperNode("receiver", Nothing, New TreeDumperNode() { Me.Visit(node.Receiver, Nothing) }), New TreeDumperNode("placeholder", Nothing, New TreeDumperNode() { Me.Visit(node.Placeholder, Nothing) }), New TreeDumperNode("accessExpression", Nothing, New TreeDumperNode() { Me.Visit(node.AccessExpression, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitConditionalAccessReceiverPlaceholder(ByVal node As BoundConditionalAccessReceiverPlaceholder, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("conditionalAccessReceiverPlaceholder", Nothing, New TreeDumperNode() { New TreeDumperNode("placeholderId", node.PlaceholderId, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitConditionalGoto(ByVal node As BoundConditionalGoto, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("conditionalGoto", Nothing, New TreeDumperNode() { New TreeDumperNode("condition", Nothing, New TreeDumperNode() { Me.Visit(node.Condition, Nothing) }), New TreeDumperNode("jumpIfTrue", node.JumpIfTrue, Nothing), New TreeDumperNode("label", node.Label, Nothing) })
		End Function

		Public Overrides Function VisitContinueStatement(ByVal node As BoundContinueStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("continueStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("label", node.Label, Nothing) })
		End Function

		Public Overrides Function VisitConversion(ByVal node As BoundConversion, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("conversion", Nothing, New TreeDumperNode() { New TreeDumperNode("operand", Nothing, New TreeDumperNode() { Me.Visit(node.Operand, Nothing) }), New TreeDumperNode("conversionKind", node.ConversionKind, Nothing), New TreeDumperNode("checked", node.Checked, Nothing), New TreeDumperNode("explicitCastInCode", node.ExplicitCastInCode, Nothing), New TreeDumperNode("constantValueOpt", node.ConstantValueOpt, Nothing), New TreeDumperNode("extendedInfoOpt", Nothing, New TreeDumperNode() { Me.Visit(node.ExtendedInfoOpt, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitConvertedTupleElements(ByVal node As BoundConvertedTupleElements, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("convertedTupleElements", Nothing, New TreeDumperNode() { New TreeDumperNode("elementPlaceholders", Nothing, node.ElementPlaceholders.[Select](Of TreeDumperNode)(Function(x As BoundRValuePlaceholder) Me.Visit(x, Nothing))), New TreeDumperNode("convertedElements", Nothing, node.ConvertedElements.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))) })
		End Function

		Public Overrides Function VisitConvertedTupleLiteral(ByVal node As BoundConvertedTupleLiteral, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("convertedTupleLiteral", Nothing, New TreeDumperNode() { New TreeDumperNode("naturalTypeOpt", node.NaturalTypeOpt, Nothing), New TreeDumperNode("arguments", Nothing, node.Arguments.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitDelegateCreationExpression(ByVal node As BoundDelegateCreationExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("delegateCreationExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("receiverOpt", Nothing, New TreeDumperNode() { Me.Visit(node.ReceiverOpt, Nothing) }), New TreeDumperNode("method", node.Method, Nothing), New TreeDumperNode("relaxationLambdaOpt", Nothing, New TreeDumperNode() { Me.Visit(node.RelaxationLambdaOpt, Nothing) }), New TreeDumperNode("relaxationReceiverPlaceholderOpt", Nothing, New TreeDumperNode() { Me.Visit(node.RelaxationReceiverPlaceholderOpt, Nothing) }), New TreeDumperNode("methodGroupOpt", Nothing, New TreeDumperNode() { Me.Visit(node.MethodGroupOpt, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitDimStatement(ByVal node As BoundDimStatement, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("localDeclarations", Nothing, node.LocalDeclarations.[Select](Of Microsoft.CodeAnalysis.TreeDumperNode)(Function(x As BoundLocalDeclarationBase) Me.Visit(x, Nothing))), Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("initializerOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.InitializerOpt, Nothing) })
			Return New Microsoft.CodeAnalysis.TreeDumperNode("dimStatement", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitDirectCast(ByVal node As BoundDirectCast, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("[directCast]", Nothing, New TreeDumperNode() { New TreeDumperNode("operand", Nothing, New TreeDumperNode() { Me.Visit(node.Operand, Nothing) }), New TreeDumperNode("conversionKind", node.ConversionKind, Nothing), New TreeDumperNode("suppressVirtualCalls", node.SuppressVirtualCalls, Nothing), New TreeDumperNode("constantValueOpt", node.ConstantValueOpt, Nothing), New TreeDumperNode("relaxationLambdaOpt", Nothing, New TreeDumperNode() { Me.Visit(node.RelaxationLambdaOpt, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitDoLoopStatement(ByVal node As BoundDoLoopStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("doLoopStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("topConditionOpt", Nothing, New TreeDumperNode() { Me.Visit(node.TopConditionOpt, Nothing) }), New TreeDumperNode("bottomConditionOpt", Nothing, New TreeDumperNode() { Me.Visit(node.BottomConditionOpt, Nothing) }), New TreeDumperNode("topConditionIsUntil", node.TopConditionIsUntil, Nothing), New TreeDumperNode("bottomConditionIsUntil", node.BottomConditionIsUntil, Nothing), New TreeDumperNode("body", Nothing, New TreeDumperNode() { Me.Visit(node.Body, Nothing) }), New TreeDumperNode("continueLabel", node.ContinueLabel, Nothing), New TreeDumperNode("exitLabel", node.ExitLabel, Nothing) })
		End Function

		Public Overrides Function VisitDup(ByVal node As BoundDup, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("dup", Nothing, New TreeDumperNode() { New TreeDumperNode("isReference", node.IsReference, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitEndStatement(ByVal node As BoundEndStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("endStatement", Nothing, Array.Empty(Of TreeDumperNode)())
		End Function

		Public Overrides Function VisitEraseStatement(ByVal node As BoundEraseStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("eraseStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("clauses", Nothing, node.Clauses.[Select](Of TreeDumperNode)(Function(x As BoundAssignmentOperator) Me.Visit(x, Nothing))) })
		End Function

		Public Overrides Function VisitEventAccess(ByVal node As BoundEventAccess, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("eventAccess", Nothing, New TreeDumperNode() { New TreeDumperNode("receiverOpt", Nothing, New TreeDumperNode() { Me.Visit(node.ReceiverOpt, Nothing) }), New TreeDumperNode("eventSymbol", node.EventSymbol, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitExitStatement(ByVal node As BoundExitStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("exitStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("label", node.Label, Nothing) })
		End Function

		Public Overrides Function VisitExpressionStatement(ByVal node As BoundExpressionStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("expressionStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("expression", Nothing, New TreeDumperNode() { Me.Visit(node.Expression, Nothing) }) })
		End Function

		Public Overrides Function VisitFieldAccess(ByVal node As BoundFieldAccess, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("fieldAccess", Nothing, New TreeDumperNode() { New TreeDumperNode("receiverOpt", Nothing, New TreeDumperNode() { Me.Visit(node.ReceiverOpt, Nothing) }), New TreeDumperNode("fieldSymbol", node.FieldSymbol, Nothing), New TreeDumperNode("isLValue", node.IsLValue, Nothing), New TreeDumperNode("suppressVirtualCalls", node.SuppressVirtualCalls, Nothing), New TreeDumperNode("constantsInProgressOpt", node.ConstantsInProgressOpt, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitFieldInfo(ByVal node As BoundFieldInfo, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("fieldInfo", Nothing, New TreeDumperNode() { New TreeDumperNode("field", node.Field, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitFieldInitializer(ByVal node As BoundFieldInitializer, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("initializedFields", node.InitializedFields, Nothing), Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("memberAccessExpressionOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.MemberAccessExpressionOpt, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("initialValue", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.InitialValue, Nothing) })
			Return New Microsoft.CodeAnalysis.TreeDumperNode("fieldInitializer", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitForEachStatement(ByVal node As BoundForEachStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("forEachStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("collection", Nothing, New TreeDumperNode() { Me.Visit(node.Collection, Nothing) }), New TreeDumperNode("enumeratorInfo", node.EnumeratorInfo, Nothing), New TreeDumperNode("declaredOrInferredLocalOpt", node.DeclaredOrInferredLocalOpt, Nothing), New TreeDumperNode("controlVariable", Nothing, New TreeDumperNode() { Me.Visit(node.ControlVariable, Nothing) }), New TreeDumperNode("body", Nothing, New TreeDumperNode() { Me.Visit(node.Body, Nothing) }), New TreeDumperNode("nextVariablesOpt", Nothing, node.NextVariablesOpt.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("continueLabel", node.ContinueLabel, Nothing), New TreeDumperNode("exitLabel", node.ExitLabel, Nothing) })
		End Function

		Public Overrides Function VisitForToStatement(ByVal node As BoundForToStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("forToStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("initialValue", Nothing, New TreeDumperNode() { Me.Visit(node.InitialValue, Nothing) }), New TreeDumperNode("limitValue", Nothing, New TreeDumperNode() { Me.Visit(node.LimitValue, Nothing) }), New TreeDumperNode("stepValue", Nothing, New TreeDumperNode() { Me.Visit(node.StepValue, Nothing) }), New TreeDumperNode("checked", node.Checked, Nothing), New TreeDumperNode("operatorsOpt", Nothing, New TreeDumperNode() { Me.Visit(node.OperatorsOpt, Nothing) }), New TreeDumperNode("declaredOrInferredLocalOpt", node.DeclaredOrInferredLocalOpt, Nothing), New TreeDumperNode("controlVariable", Nothing, New TreeDumperNode() { Me.Visit(node.ControlVariable, Nothing) }), New TreeDumperNode("body", Nothing, New TreeDumperNode() { Me.Visit(node.Body, Nothing) }), New TreeDumperNode("nextVariablesOpt", Nothing, node.NextVariablesOpt.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("continueLabel", node.ContinueLabel, Nothing), New TreeDumperNode("exitLabel", node.ExitLabel, Nothing) })
		End Function

		Public Overrides Function VisitForToUserDefinedOperators(ByVal node As BoundForToUserDefinedOperators, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("forToUserDefinedOperators", Nothing, New TreeDumperNode() { New TreeDumperNode("leftOperandPlaceholder", Nothing, New TreeDumperNode() { Me.Visit(node.LeftOperandPlaceholder, Nothing) }), New TreeDumperNode("rightOperandPlaceholder", Nothing, New TreeDumperNode() { Me.Visit(node.RightOperandPlaceholder, Nothing) }), New TreeDumperNode("addition", Nothing, New TreeDumperNode() { Me.Visit(node.Addition, Nothing) }), New TreeDumperNode("subtraction", Nothing, New TreeDumperNode() { Me.Visit(node.Subtraction, Nothing) }), New TreeDumperNode("lessThanOrEqual", Nothing, New TreeDumperNode() { Me.Visit(node.LessThanOrEqual, Nothing) }), New TreeDumperNode("greaterThanOrEqual", Nothing, New TreeDumperNode() { Me.Visit(node.GreaterThanOrEqual, Nothing) }) })
		End Function

		Public Overrides Function VisitGetType(ByVal node As BoundGetType, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("[getType]", Nothing, New TreeDumperNode() { New TreeDumperNode("sourceType", Nothing, New TreeDumperNode() { Me.Visit(node.SourceType, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitGlobalStatementInitializer(ByVal node As BoundGlobalStatementInitializer, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("globalStatementInitializer", Nothing, New TreeDumperNode() { New TreeDumperNode("statement", Nothing, New TreeDumperNode() { Me.Visit(node.Statement, Nothing) }) })
		End Function

		Public Overrides Function VisitGotoStatement(ByVal node As BoundGotoStatement, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("label", node.Label, Nothing), Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("labelExpressionOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.LabelExpressionOpt, Nothing) })
			Return New Microsoft.CodeAnalysis.TreeDumperNode("gotoStatement", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitGroupAggregation(ByVal node As BoundGroupAggregation, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("groupAggregation", Nothing, New TreeDumperNode() { New TreeDumperNode("group", Nothing, New TreeDumperNode() { Me.Visit(node.Group, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitGroupTypeInferenceLambda(ByVal node As GroupTypeInferenceLambda, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("groupTypeInferenceLambda", Nothing, New TreeDumperNode() { New TreeDumperNode("binder", node.Binder, Nothing), New TreeDumperNode("parameters", node.Parameters, Nothing), New TreeDumperNode("compilation", node.Compilation, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitHostObjectMemberReference(ByVal node As BoundHostObjectMemberReference, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("hostObjectMemberReference", Nothing, New TreeDumperNode() { New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitIfStatement(ByVal node As BoundIfStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("ifStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("condition", Nothing, New TreeDumperNode() { Me.Visit(node.Condition, Nothing) }), New TreeDumperNode("consequence", Nothing, New TreeDumperNode() { Me.Visit(node.Consequence, Nothing) }), New TreeDumperNode("alternativeOpt", Nothing, New TreeDumperNode() { Me.Visit(node.AlternativeOpt, Nothing) }) })
		End Function

		Public Overrides Function VisitInitializer(ByVal node As BoundInitializer, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("initializer", Nothing, Array.Empty(Of TreeDumperNode)())
		End Function

		Public Overrides Function VisitInstrumentationPayloadRoot(ByVal node As BoundInstrumentationPayloadRoot, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("instrumentationPayloadRoot", Nothing, New TreeDumperNode() { New TreeDumperNode("analysisKind", node.AnalysisKind, Nothing), New TreeDumperNode("isLValue", node.IsLValue, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitInterpolatedStringExpression(ByVal node As BoundInterpolatedStringExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("interpolatedStringExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("contents", Nothing, node.Contents.[Select](Of TreeDumperNode)(Function(x As BoundNode) Me.Visit(x, Nothing))), New TreeDumperNode("binder", node.Binder, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitInterpolation(ByVal node As BoundInterpolation, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("interpolation", Nothing, New TreeDumperNode() { New TreeDumperNode("expression", Nothing, New TreeDumperNode() { Me.Visit(node.Expression, Nothing) }), New TreeDumperNode("alignmentOpt", Nothing, New TreeDumperNode() { Me.Visit(node.AlignmentOpt, Nothing) }), New TreeDumperNode("formatStringOpt", Nothing, New TreeDumperNode() { Me.Visit(node.FormatStringOpt, Nothing) }) })
		End Function

		Public Overrides Function VisitLabel(ByVal node As BoundLabel, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("label", Nothing, New TreeDumperNode() { New TreeDumperNode("label", node.Label, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitLabelStatement(ByVal node As BoundLabelStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("labelStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("label", node.Label, Nothing) })
		End Function

		Public Overrides Function VisitLambda(ByVal node As BoundLambda, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("lambdaSymbol", node.LambdaSymbol, Nothing), Nothing, Nothing, Nothing, Nothing, Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("body", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Body, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("diagnostics", node.Diagnostics, Nothing)
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("lambdaBinderOpt", node.LambdaBinderOpt, Nothing)
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("delegateRelaxation", node.DelegateRelaxation, Nothing)
			treeDumperNode(5) = New Microsoft.CodeAnalysis.TreeDumperNode("methodConversionKind", node.MethodConversionKind, Nothing)
			treeDumperNode(6) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("lambda", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitLateAddressOfOperator(ByVal node As BoundLateAddressOfOperator, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("binder", node.Binder, Nothing), Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("memberAccess", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.MemberAccess, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("lateAddressOfOperator", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitLateBoundArgumentSupportingAssignmentWithCapture(ByVal node As BoundLateBoundArgumentSupportingAssignmentWithCapture, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("lateBoundArgumentSupportingAssignmentWithCapture", Nothing, New TreeDumperNode() { New TreeDumperNode("originalArgument", Nothing, New TreeDumperNode() { Me.Visit(node.OriginalArgument, Nothing) }), New TreeDumperNode("localSymbol", node.LocalSymbol, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitLateInvocation(ByVal node As BoundLateInvocation, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("lateInvocation", Nothing, New TreeDumperNode() { New TreeDumperNode("member", Nothing, New TreeDumperNode() { Me.Visit(node.Member, Nothing) }), New TreeDumperNode("argumentsOpt", Nothing, node.ArgumentsOpt.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, Nothing), New TreeDumperNode("accessKind", node.AccessKind, Nothing), New TreeDumperNode("methodOrPropertyGroupOpt", Nothing, New TreeDumperNode() { Me.Visit(node.MethodOrPropertyGroupOpt, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitLateMemberAccess(ByVal node As BoundLateMemberAccess, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("nameOpt", node.NameOpt, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("containerTypeOpt", node.ContainerTypeOpt, Nothing), Nothing, Nothing, Nothing, Nothing }
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("receiverOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ReceiverOpt, Nothing) })
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("typeArgumentsOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.TypeArgumentsOpt, Nothing) })
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("accessKind", node.AccessKind, Nothing)
			treeDumperNode(5) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("lateMemberAccess", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitLiteral(ByVal node As BoundLiteral, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("literal", Nothing, New TreeDumperNode() { New TreeDumperNode("value", node.Value, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitLocal(ByVal node As BoundLocal, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("local", Nothing, New TreeDumperNode() { New TreeDumperNode("localSymbol", node.LocalSymbol, Nothing), New TreeDumperNode("isLValue", node.IsLValue, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitLocalDeclaration(ByVal node As BoundLocalDeclaration, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("localSymbol", node.LocalSymbol, Nothing), Nothing, Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("declarationInitializerOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.DeclarationInitializerOpt, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("identifierInitializerOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.IdentifierInitializerOpt, Nothing) })
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("initializedByAsNew", node.InitializedByAsNew, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("localDeclaration", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitLoweredConditionalAccess(ByVal node As BoundLoweredConditionalAccess, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("loweredConditionalAccess", Nothing, New TreeDumperNode() { New TreeDumperNode("receiverOrCondition", Nothing, New TreeDumperNode() { Me.Visit(node.ReceiverOrCondition, Nothing) }), New TreeDumperNode("captureReceiver", node.CaptureReceiver, Nothing), New TreeDumperNode("placeholderId", node.PlaceholderId, Nothing), New TreeDumperNode("whenNotNull", Nothing, New TreeDumperNode() { Me.Visit(node.WhenNotNull, Nothing) }), New TreeDumperNode("whenNullOpt", Nothing, New TreeDumperNode() { Me.Visit(node.WhenNullOpt, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitLValuePlaceholder(ByVal node As BoundLValuePlaceholder, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("lValuePlaceholder", Nothing, New TreeDumperNode() { New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitLValueToRValueWrapper(ByVal node As BoundLValueToRValueWrapper, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("lValueToRValueWrapper", Nothing, New TreeDumperNode() { New TreeDumperNode("underlyingLValue", Nothing, New TreeDumperNode() { Me.Visit(node.UnderlyingLValue, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitMaximumMethodDefIndex(ByVal node As BoundMaximumMethodDefIndex, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("maximumMethodDefIndex", Nothing, New TreeDumperNode() { New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitMeReference(ByVal node As BoundMeReference, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("meReference", Nothing, New TreeDumperNode() { New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitMethodDefIndex(ByVal node As BoundMethodDefIndex, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("methodDefIndex", Nothing, New TreeDumperNode() { New TreeDumperNode("method", node.Method, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitMethodGroup(ByVal node As BoundMethodGroup, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("methodGroup", Nothing, New TreeDumperNode() { New TreeDumperNode("typeArgumentsOpt", Nothing, New TreeDumperNode() { Me.Visit(node.TypeArgumentsOpt, Nothing) }), New TreeDumperNode("methods", node.Methods, Nothing), New TreeDumperNode("pendingExtensionMethodsOpt", node.PendingExtensionMethodsOpt, Nothing), New TreeDumperNode("resultKind", node.ResultKind, Nothing), New TreeDumperNode("receiverOpt", Nothing, New TreeDumperNode() { Me.Visit(node.ReceiverOpt, Nothing) }), New TreeDumperNode("qualificationKind", node.QualificationKind, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitMethodInfo(ByVal node As BoundMethodInfo, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("methodInfo", Nothing, New TreeDumperNode() { New TreeDumperNode("method", node.Method, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitMidResult(ByVal node As BoundMidResult, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("midResult", Nothing, New TreeDumperNode() { New TreeDumperNode("original", Nothing, New TreeDumperNode() { Me.Visit(node.Original, Nothing) }), New TreeDumperNode("start", Nothing, New TreeDumperNode() { Me.Visit(node.Start, Nothing) }), New TreeDumperNode("lengthOpt", Nothing, New TreeDumperNode() { Me.Visit(node.LengthOpt, Nothing) }), New TreeDumperNode("source", Nothing, New TreeDumperNode() { Me.Visit(node.Source, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitModuleVersionId(ByVal node As BoundModuleVersionId, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("moduleVersionId", Nothing, New TreeDumperNode() { New TreeDumperNode("isLValue", node.IsLValue, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitModuleVersionIdString(ByVal node As BoundModuleVersionIdString, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("moduleVersionIdString", Nothing, New TreeDumperNode() { New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitMyBaseReference(ByVal node As BoundMyBaseReference, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("myBaseReference", Nothing, New TreeDumperNode() { New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitMyClassReference(ByVal node As BoundMyClassReference, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("myClassReference", Nothing, New TreeDumperNode() { New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitNameOfOperator(ByVal node As BoundNameOfOperator, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("nameOfOperator", Nothing, New TreeDumperNode() { New TreeDumperNode("argument", Nothing, New TreeDumperNode() { Me.Visit(node.Argument, Nothing) }), New TreeDumperNode("constantValueOpt", node.ConstantValueOpt, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitNamespaceExpression(ByVal node As BoundNamespaceExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("namespaceExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("unevaluatedReceiverOpt", Nothing, New TreeDumperNode() { Me.Visit(node.UnevaluatedReceiverOpt, Nothing) }), New TreeDumperNode("aliasOpt", node.AliasOpt, Nothing), New TreeDumperNode("namespaceSymbol", node.NamespaceSymbol, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitNewT(ByVal node As BoundNewT, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("newT", Nothing, New TreeDumperNode() { New TreeDumperNode("initializerOpt", Nothing, New TreeDumperNode() { Me.Visit(node.InitializerOpt, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitNoOpStatement(ByVal node As BoundNoOpStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("noOpStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("flavor", node.Flavor, Nothing) })
		End Function

		Public Overrides Function VisitNoPiaObjectCreationExpression(ByVal node As BoundNoPiaObjectCreationExpression, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("guidString", node.GuidString, Nothing), Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("initializerOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.InitializerOpt, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("noPiaObjectCreationExpression", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitNullableIsTrueOperator(ByVal node As BoundNullableIsTrueOperator, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("nullableIsTrueOperator", Nothing, New TreeDumperNode() { New TreeDumperNode("operand", Nothing, New TreeDumperNode() { Me.Visit(node.Operand, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitObjectCreationExpression(ByVal node As BoundObjectCreationExpression, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("constructorOpt", node.ConstructorOpt, Nothing), Nothing, Nothing, Nothing, Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("methodGroupOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.MethodGroupOpt, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("arguments", Nothing, node.Arguments.[Select](Of Microsoft.CodeAnalysis.TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing)))
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("defaultArguments", node.DefaultArguments, Nothing)
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("initializerOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.InitializerOpt, Nothing) })
			treeDumperNode(5) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("objectCreationExpression", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitObjectInitializerExpression(ByVal node As BoundObjectInitializerExpression, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("createTemporaryLocalForInitialization", node.CreateTemporaryLocalForInitialization, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("binder", node.Binder, Nothing), Nothing, Nothing, Nothing }
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("placeholderOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.PlaceholderOpt, Nothing) })
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("initializers", Nothing, node.Initializers.[Select](Of Microsoft.CodeAnalysis.TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing)))
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("objectInitializerExpression", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitOmittedArgument(ByVal node As BoundOmittedArgument, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("omittedArgument", Nothing, New TreeDumperNode() { New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitOnErrorStatement(ByVal node As BoundOnErrorStatement, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("onErrorKind", node.OnErrorKind, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("labelOpt", node.LabelOpt, Nothing), Nothing }
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("labelExpressionOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.LabelExpressionOpt, Nothing) })
			Return New Microsoft.CodeAnalysis.TreeDumperNode("onErrorStatement", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitOrdering(ByVal node As BoundOrdering, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("ordering", Nothing, New TreeDumperNode() { New TreeDumperNode("underlyingExpression", Nothing, New TreeDumperNode() { Me.Visit(node.UnderlyingExpression, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitParameter(ByVal node As BoundParameter, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("parameter", Nothing, New TreeDumperNode() { New TreeDumperNode("parameterSymbol", node.ParameterSymbol, Nothing), New TreeDumperNode("isLValue", node.IsLValue, Nothing), New TreeDumperNode("suppressVirtualCalls", node.SuppressVirtualCalls, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitParameterEqualsValue(ByVal node As BoundParameterEqualsValue, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("parameter", node.Parameter, Nothing), Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("value", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Value, Nothing) })
			Return New Microsoft.CodeAnalysis.TreeDumperNode("parameterEqualsValue", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitParenthesized(ByVal node As BoundParenthesized, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("parenthesized", Nothing, New TreeDumperNode() { New TreeDumperNode("expression", Nothing, New TreeDumperNode() { Me.Visit(node.Expression, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitPreviousSubmissionReference(ByVal node As BoundPreviousSubmissionReference, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("previousSubmissionReference", Nothing, New TreeDumperNode() { New TreeDumperNode("sourceType", node.SourceType, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitPropertyAccess(ByVal node As BoundPropertyAccess, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("propertySymbol", node.PropertySymbol, Nothing), Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("propertyGroupOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.PropertyGroupOpt, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("accessKind", node.AccessKind, Nothing)
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("isWriteable", node.IsWriteable, Nothing)
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("isLValue", node.IsLValue, Nothing)
			treeDumperNode(5) = New Microsoft.CodeAnalysis.TreeDumperNode("receiverOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ReceiverOpt, Nothing) })
			treeDumperNode(6) = New Microsoft.CodeAnalysis.TreeDumperNode("arguments", Nothing, node.Arguments.[Select](Of Microsoft.CodeAnalysis.TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing)))
			treeDumperNode(7) = New Microsoft.CodeAnalysis.TreeDumperNode("defaultArguments", node.DefaultArguments, Nothing)
			treeDumperNode(8) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("propertyAccess", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitPropertyGroup(ByVal node As BoundPropertyGroup, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("properties", node.Properties, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("resultKind", node.ResultKind, Nothing), Nothing, Nothing, Nothing }
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("receiverOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ReceiverOpt, Nothing) })
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("qualificationKind", node.QualificationKind, Nothing)
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("propertyGroup", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitPropertyInitializer(ByVal node As BoundPropertyInitializer, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("initializedProperties", node.InitializedProperties, Nothing), Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("memberAccessExpressionOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.MemberAccessExpressionOpt, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("initialValue", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.InitialValue, Nothing) })
			Return New Microsoft.CodeAnalysis.TreeDumperNode("propertyInitializer", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitPseudoVariable(ByVal node As BoundPseudoVariable, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("pseudoVariable", Nothing, New TreeDumperNode() { New TreeDumperNode("localSymbol", node.LocalSymbol, Nothing), New TreeDumperNode("isLValue", node.IsLValue, Nothing), New TreeDumperNode("emitExpressions", node.EmitExpressions, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitQueryableSource(ByVal node As BoundQueryableSource, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("queryableSource", Nothing, New TreeDumperNode() { New TreeDumperNode("source", Nothing, New TreeDumperNode() { Me.Visit(node.Source, Nothing) }), New TreeDumperNode("rangeVariableOpt", node.RangeVariableOpt, Nothing), New TreeDumperNode("rangeVariables", node.RangeVariables, Nothing), New TreeDumperNode("compoundVariableType", node.CompoundVariableType, Nothing), New TreeDumperNode("binders", node.Binders, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitQueryClause(ByVal node As BoundQueryClause, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("queryClause", Nothing, New TreeDumperNode() { New TreeDumperNode("underlyingExpression", Nothing, New TreeDumperNode() { Me.Visit(node.UnderlyingExpression, Nothing) }), New TreeDumperNode("rangeVariables", node.RangeVariables, Nothing), New TreeDumperNode("compoundVariableType", node.CompoundVariableType, Nothing), New TreeDumperNode("binders", node.Binders, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitQueryExpression(ByVal node As BoundQueryExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("queryExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("lastOperator", Nothing, New TreeDumperNode() { Me.Visit(node.LastOperator, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitQueryLambda(ByVal node As BoundQueryLambda, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("lambdaSymbol", node.LambdaSymbol, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("rangeVariables", node.RangeVariables, Nothing), Nothing, Nothing, Nothing }
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("expression", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Expression, Nothing) })
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("exprIsOperandOfConditionalBranch", node.ExprIsOperandOfConditionalBranch, Nothing)
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("queryLambda", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitQuerySource(ByVal node As BoundQuerySource, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("querySource", Nothing, New TreeDumperNode() { New TreeDumperNode("expression", Nothing, New TreeDumperNode() { Me.Visit(node.Expression, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitRaiseEventStatement(ByVal node As BoundRaiseEventStatement, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("eventSymbol", node.EventSymbol, Nothing), Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("eventInvocation", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.EventInvocation, Nothing) })
			Return New Microsoft.CodeAnalysis.TreeDumperNode("raiseEventStatement", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitRangeCaseClause(ByVal node As BoundRangeCaseClause, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("rangeCaseClause", Nothing, New TreeDumperNode() { New TreeDumperNode("lowerBoundOpt", Nothing, New TreeDumperNode() { Me.Visit(node.LowerBoundOpt, Nothing) }), New TreeDumperNode("upperBoundOpt", Nothing, New TreeDumperNode() { Me.Visit(node.UpperBoundOpt, Nothing) }), New TreeDumperNode("lowerBoundConditionOpt", Nothing, New TreeDumperNode() { Me.Visit(node.LowerBoundConditionOpt, Nothing) }), New TreeDumperNode("upperBoundConditionOpt", Nothing, New TreeDumperNode() { Me.Visit(node.UpperBoundConditionOpt, Nothing) }) })
		End Function

		Public Overrides Function VisitRangeVariable(ByVal node As BoundRangeVariable, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("rangeVariable", Nothing, New TreeDumperNode() { New TreeDumperNode("rangeVariable", node.RangeVariable, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitRangeVariableAssignment(ByVal node As BoundRangeVariableAssignment, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("rangeVariable", node.RangeVariable, Nothing), Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("value", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Value, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("rangeVariableAssignment", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitRedimClause(ByVal node As BoundRedimClause, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("redimClause", Nothing, New TreeDumperNode() { New TreeDumperNode("operand", Nothing, New TreeDumperNode() { Me.Visit(node.Operand, Nothing) }), New TreeDumperNode("indices", Nothing, node.Indices.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("arrayTypeOpt", node.ArrayTypeOpt, Nothing), New TreeDumperNode("preserve", node.Preserve, Nothing) })
		End Function

		Public Overrides Function VisitRedimStatement(ByVal node As BoundRedimStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("redimStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("clauses", Nothing, node.Clauses.[Select](Of TreeDumperNode)(Function(x As BoundRedimClause) Me.Visit(x, Nothing))) })
		End Function

		Public Overrides Function VisitReferenceAssignment(ByVal node As BoundReferenceAssignment, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("referenceAssignment", Nothing, New TreeDumperNode() { New TreeDumperNode("byRefLocal", Nothing, New TreeDumperNode() { Me.Visit(node.ByRefLocal, Nothing) }), New TreeDumperNode("lValue", Nothing, New TreeDumperNode() { Me.Visit(node.LValue, Nothing) }), New TreeDumperNode("isLValue", node.IsLValue, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitRelationalCaseClause(ByVal node As BoundRelationalCaseClause, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("operatorKind", node.OperatorKind, Nothing), Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("valueOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ValueOpt, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("conditionOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ConditionOpt, Nothing) })
			Return New Microsoft.CodeAnalysis.TreeDumperNode("relationalCaseClause", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitRelaxationLambda(ByVal node As BoundRelaxationLambda, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("relaxationLambda", Nothing, New TreeDumperNode() { New TreeDumperNode("lambda", Nothing, New TreeDumperNode() { Me.Visit(node.Lambda, Nothing) }), New TreeDumperNode("receiverPlaceholderOpt", Nothing, New TreeDumperNode() { Me.Visit(node.ReceiverPlaceholderOpt, Nothing) }) })
		End Function

		Public Overrides Function VisitRemoveHandlerStatement(ByVal node As BoundRemoveHandlerStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("removeHandlerStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("eventAccess", Nothing, New TreeDumperNode() { Me.Visit(node.EventAccess, Nothing) }), New TreeDumperNode("handler", Nothing, New TreeDumperNode() { Me.Visit(node.Handler, Nothing) }) })
		End Function

		Public Overrides Function VisitResumeStatement(ByVal node As BoundResumeStatement, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("resumeKind", node.ResumeKind, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("labelOpt", node.LabelOpt, Nothing), Nothing }
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("labelExpressionOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.LabelExpressionOpt, Nothing) })
			Return New Microsoft.CodeAnalysis.TreeDumperNode("resumeStatement", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitReturnStatement(ByVal node As BoundReturnStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("returnStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("expressionOpt", Nothing, New TreeDumperNode() { Me.Visit(node.ExpressionOpt, Nothing) }), New TreeDumperNode("functionLocalOpt", node.FunctionLocalOpt, Nothing), New TreeDumperNode("exitLabelOpt", node.ExitLabelOpt, Nothing) })
		End Function

		Public Overrides Function VisitRValuePlaceholder(ByVal node As BoundRValuePlaceholder, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("rValuePlaceholder", Nothing, New TreeDumperNode() { New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitSelectStatement(ByVal node As BoundSelectStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("selectStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("expressionStatement", Nothing, New TreeDumperNode() { Me.Visit(node.ExpressionStatement, Nothing) }), New TreeDumperNode("exprPlaceholderOpt", Nothing, New TreeDumperNode() { Me.Visit(node.ExprPlaceholderOpt, Nothing) }), New TreeDumperNode("caseBlocks", Nothing, node.CaseBlocks.[Select](Of TreeDumperNode)(Function(x As BoundCaseBlock) Me.Visit(x, Nothing))), New TreeDumperNode("recommendSwitchTable", node.RecommendSwitchTable, Nothing), New TreeDumperNode("exitLabel", node.ExitLabel, Nothing) })
		End Function

		Public Overrides Function VisitSequence(ByVal node As BoundSequence, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("locals", node.Locals, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("sideEffects", Nothing, node.SideEffects.[Select](Of Microsoft.CodeAnalysis.TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), Nothing, Nothing }
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("valueOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ValueOpt, Nothing) })
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("sequence", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitSequencePoint(ByVal node As BoundSequencePoint, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("sequencePoint", Nothing, New TreeDumperNode() { New TreeDumperNode("statementOpt", Nothing, New TreeDumperNode() { Me.Visit(node.StatementOpt, Nothing) }) })
		End Function

		Public Overrides Function VisitSequencePointExpression(ByVal node As BoundSequencePointExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("sequencePointExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("expression", Nothing, New TreeDumperNode() { Me.Visit(node.Expression, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitSequencePointWithSpan(ByVal node As BoundSequencePointWithSpan, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("sequencePointWithSpan", Nothing, New TreeDumperNode() { New TreeDumperNode("statementOpt", Nothing, New TreeDumperNode() { Me.Visit(node.StatementOpt, Nothing) }), New TreeDumperNode("span", node.Span, Nothing) })
		End Function

		Public Overrides Function VisitSimpleCaseClause(ByVal node As BoundSimpleCaseClause, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("simpleCaseClause", Nothing, New TreeDumperNode() { New TreeDumperNode("valueOpt", Nothing, New TreeDumperNode() { Me.Visit(node.ValueOpt, Nothing) }), New TreeDumperNode("conditionOpt", Nothing, New TreeDumperNode() { Me.Visit(node.ConditionOpt, Nothing) }) })
		End Function

		Public Overrides Function VisitSourceDocumentIndex(ByVal node As BoundSourceDocumentIndex, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("sourceDocumentIndex", Nothing, New TreeDumperNode() { New TreeDumperNode("document", node.Document, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitSpillSequence(ByVal node As BoundSpillSequence, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("locals", node.Locals, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("spillFields", node.SpillFields, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("statements", Nothing, node.Statements.[Select](Of Microsoft.CodeAnalysis.TreeDumperNode)(Function(x As BoundStatement) Me.Visit(x, Nothing))), Nothing, Nothing }
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("valueOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ValueOpt, Nothing) })
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("spillSequence", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitStateMachineScope(ByVal node As BoundStateMachineScope, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("fields", node.Fields, Nothing), Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("statement", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Statement, Nothing) })
			Return New Microsoft.CodeAnalysis.TreeDumperNode("stateMachineScope", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitStatementList(ByVal node As BoundStatementList, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("statementList", Nothing, New TreeDumperNode() { New TreeDumperNode("statements", Nothing, node.Statements.[Select](Of TreeDumperNode)(Function(x As BoundStatement) Me.Visit(x, Nothing))) })
		End Function

		Public Overrides Function VisitStopStatement(ByVal node As BoundStopStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("stopStatement", Nothing, Array.Empty(Of TreeDumperNode)())
		End Function

		Public Overrides Function VisitSyncLockStatement(ByVal node As BoundSyncLockStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("syncLockStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("lockExpression", Nothing, New TreeDumperNode() { Me.Visit(node.LockExpression, Nothing) }), New TreeDumperNode("body", Nothing, New TreeDumperNode() { Me.Visit(node.Body, Nothing) }) })
		End Function

		Public Overrides Function VisitTernaryConditionalExpression(ByVal node As BoundTernaryConditionalExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("ternaryConditionalExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("condition", Nothing, New TreeDumperNode() { Me.Visit(node.Condition, Nothing) }), New TreeDumperNode("whenTrue", Nothing, New TreeDumperNode() { Me.Visit(node.WhenTrue, Nothing) }), New TreeDumperNode("whenFalse", Nothing, New TreeDumperNode() { Me.Visit(node.WhenFalse, Nothing) }), New TreeDumperNode("constantValueOpt", node.ConstantValueOpt, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitThrowStatement(ByVal node As BoundThrowStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("throwStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("expressionOpt", Nothing, New TreeDumperNode() { Me.Visit(node.ExpressionOpt, Nothing) }) })
		End Function

		Public Overrides Function VisitToQueryableCollectionConversion(ByVal node As BoundToQueryableCollectionConversion, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("toQueryableCollectionConversion", Nothing, New TreeDumperNode() { New TreeDumperNode("conversionCall", Nothing, New TreeDumperNode() { Me.Visit(node.ConversionCall, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitTryCast(ByVal node As BoundTryCast, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("[tryCast]", Nothing, New TreeDumperNode() { New TreeDumperNode("operand", Nothing, New TreeDumperNode() { Me.Visit(node.Operand, Nothing) }), New TreeDumperNode("conversionKind", node.ConversionKind, Nothing), New TreeDumperNode("constantValueOpt", node.ConstantValueOpt, Nothing), New TreeDumperNode("relaxationLambdaOpt", Nothing, New TreeDumperNode() { Me.Visit(node.RelaxationLambdaOpt, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitTryStatement(ByVal node As BoundTryStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("tryStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("tryBlock", Nothing, New TreeDumperNode() { Me.Visit(node.TryBlock, Nothing) }), New TreeDumperNode("catchBlocks", Nothing, node.CatchBlocks.[Select](Of TreeDumperNode)(Function(x As BoundCatchBlock) Me.Visit(x, Nothing))), New TreeDumperNode("finallyBlockOpt", Nothing, New TreeDumperNode() { Me.Visit(node.FinallyBlockOpt, Nothing) }), New TreeDumperNode("exitLabelOpt", node.ExitLabelOpt, Nothing) })
		End Function

		Public Overrides Function VisitTupleLiteral(ByVal node As BoundTupleLiteral, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("tupleLiteral", Nothing, New TreeDumperNode() { New TreeDumperNode("inferredType", node.InferredType, Nothing), New TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, Nothing), New TreeDumperNode("inferredNamesOpt", node.InferredNamesOpt, Nothing), New TreeDumperNode("arguments", Nothing, node.Arguments.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitTypeArguments(ByVal node As BoundTypeArguments, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("typeArguments", Nothing, New TreeDumperNode() { New TreeDumperNode("arguments", node.Arguments, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitTypeAsValueExpression(ByVal node As BoundTypeAsValueExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("typeAsValueExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("expression", Nothing, New TreeDumperNode() { Me.Visit(node.Expression, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitTypeExpression(ByVal node As BoundTypeExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("typeExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("unevaluatedReceiverOpt", Nothing, New TreeDumperNode() { Me.Visit(node.UnevaluatedReceiverOpt, Nothing) }), New TreeDumperNode("aliasOpt", node.AliasOpt, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitTypeOf(ByVal node As BoundTypeOf, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("[typeOf]", Nothing, New TreeDumperNode() { New TreeDumperNode("operand", Nothing, New TreeDumperNode() { Me.Visit(node.Operand, Nothing) }), New TreeDumperNode("isTypeOfIsNotExpression", node.IsTypeOfIsNotExpression, Nothing), New TreeDumperNode("targetType", node.TargetType, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitTypeOrValueExpression(ByVal node As BoundTypeOrValueExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("typeOrValueExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("data", node.Data, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitUnaryOperator(ByVal node As BoundUnaryOperator, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("operatorKind", node.OperatorKind, Nothing), Nothing, Nothing, Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("operand", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Operand, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("checked", node.Checked, Nothing)
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("constantValueOpt", node.ConstantValueOpt, Nothing)
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("unaryOperator", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitUnboundLambda(ByVal node As UnboundLambda, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("unboundLambda", Nothing, New TreeDumperNode() { New TreeDumperNode("binder", node.Binder, Nothing), New TreeDumperNode("flags", node.Flags, Nothing), New TreeDumperNode("parameters", node.Parameters, Nothing), New TreeDumperNode("returnType", node.ReturnType, Nothing), New TreeDumperNode("bindingCache", node.BindingCache, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitUnstructuredExceptionHandlingCatchFilter(ByVal node As BoundUnstructuredExceptionHandlingCatchFilter, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("unstructuredExceptionHandlingCatchFilter", Nothing, New TreeDumperNode() { New TreeDumperNode("activeHandlerLocal", Nothing, New TreeDumperNode() { Me.Visit(node.ActiveHandlerLocal, Nothing) }), New TreeDumperNode("resumeTargetLocal", Nothing, New TreeDumperNode() { Me.Visit(node.ResumeTargetLocal, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitUnstructuredExceptionHandlingStatement(ByVal node As BoundUnstructuredExceptionHandlingStatement, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("containsOnError", node.ContainsOnError, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("containsResume", node.ContainsResume, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("resumeWithoutLabelOpt", node.ResumeWithoutLabelOpt, Nothing), New Microsoft.CodeAnalysis.TreeDumperNode("trackLineNumber", node.TrackLineNumber, Nothing), Nothing }
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("body", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Body, Nothing) })
			Return New Microsoft.CodeAnalysis.TreeDumperNode("unstructuredExceptionHandlingStatement", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitUnstructuredExceptionOnErrorSwitch(ByVal node As BoundUnstructuredExceptionOnErrorSwitch, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("unstructuredExceptionOnErrorSwitch", Nothing, New TreeDumperNode() { New TreeDumperNode("value", Nothing, New TreeDumperNode() { Me.Visit(node.Value, Nothing) }), New TreeDumperNode("jumps", Nothing, node.Jumps.[Select](Of TreeDumperNode)(Function(x As BoundGotoStatement) Me.Visit(x, Nothing))) })
		End Function

		Public Overrides Function VisitUnstructuredExceptionResumeSwitch(ByVal node As BoundUnstructuredExceptionResumeSwitch, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("unstructuredExceptionResumeSwitch", Nothing, New TreeDumperNode() { New TreeDumperNode("resumeTargetTemporary", Nothing, New TreeDumperNode() { Me.Visit(node.ResumeTargetTemporary, Nothing) }), New TreeDumperNode("resumeLabel", Nothing, New TreeDumperNode() { Me.Visit(node.ResumeLabel, Nothing) }), New TreeDumperNode("resumeNextLabel", Nothing, New TreeDumperNode() { Me.Visit(node.ResumeNextLabel, Nothing) }), New TreeDumperNode("jumps", Nothing, node.Jumps.[Select](Of TreeDumperNode)(Function(x As BoundGotoStatement) Me.Visit(x, Nothing))) })
		End Function

		Public Overrides Function VisitUserDefinedBinaryOperator(ByVal node As BoundUserDefinedBinaryOperator, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("operatorKind", node.OperatorKind, Nothing), Nothing, Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("underlyingExpression", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.UnderlyingExpression, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("checked", node.Checked, Nothing)
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("userDefinedBinaryOperator", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitUserDefinedConversion(ByVal node As BoundUserDefinedConversion, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("userDefinedConversion", Nothing, New TreeDumperNode() { New TreeDumperNode("underlyingExpression", Nothing, New TreeDumperNode() { Me.Visit(node.UnderlyingExpression, Nothing) }), New TreeDumperNode("inOutConversionFlags", node.InOutConversionFlags, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitUserDefinedShortCircuitingOperator(ByVal node As BoundUserDefinedShortCircuitingOperator, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("userDefinedShortCircuitingOperator", Nothing, New TreeDumperNode() { New TreeDumperNode("leftOperand", Nothing, New TreeDumperNode() { Me.Visit(node.LeftOperand, Nothing) }), New TreeDumperNode("leftOperandPlaceholder", Nothing, New TreeDumperNode() { Me.Visit(node.LeftOperandPlaceholder, Nothing) }), New TreeDumperNode("leftTest", Nothing, New TreeDumperNode() { Me.Visit(node.LeftTest, Nothing) }), New TreeDumperNode("bitwiseOperator", Nothing, New TreeDumperNode() { Me.Visit(node.BitwiseOperator, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitUserDefinedUnaryOperator(ByVal node As BoundUserDefinedUnaryOperator, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("operatorKind", node.OperatorKind, Nothing), Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("underlyingExpression", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.UnderlyingExpression, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("type", node.Type, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("userDefinedUnaryOperator", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitUsingStatement(ByVal node As BoundUsingStatement, ByVal arg As Object) As Microsoft.CodeAnalysis.TreeDumperNode
			Dim treeDumperNode() As Microsoft.CodeAnalysis.TreeDumperNode = { New Microsoft.CodeAnalysis.TreeDumperNode("resourceList", Nothing, node.ResourceList.[Select](Of Microsoft.CodeAnalysis.TreeDumperNode)(Function(x As BoundLocalDeclarationBase) Me.Visit(x, Nothing))), Nothing, Nothing, Nothing, Nothing }
			treeDumperNode(1) = New Microsoft.CodeAnalysis.TreeDumperNode("resourceExpressionOpt", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.ResourceExpressionOpt, Nothing) })
			treeDumperNode(2) = New Microsoft.CodeAnalysis.TreeDumperNode("body", Nothing, New Microsoft.CodeAnalysis.TreeDumperNode() { Me.Visit(node.Body, Nothing) })
			treeDumperNode(3) = New Microsoft.CodeAnalysis.TreeDumperNode("usingInfo", node.UsingInfo, Nothing)
			treeDumperNode(4) = New Microsoft.CodeAnalysis.TreeDumperNode("locals", node.Locals, Nothing)
			Return New Microsoft.CodeAnalysis.TreeDumperNode("usingStatement", Nothing, treeDumperNode)
		End Function

		Public Overrides Function VisitValueTypeMeReference(ByVal node As BoundValueTypeMeReference, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("valueTypeMeReference", Nothing, New TreeDumperNode() { New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitWhileStatement(ByVal node As BoundWhileStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("whileStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("condition", Nothing, New TreeDumperNode() { Me.Visit(node.Condition, Nothing) }), New TreeDumperNode("body", Nothing, New TreeDumperNode() { Me.Visit(node.Body, Nothing) }), New TreeDumperNode("continueLabel", node.ContinueLabel, Nothing), New TreeDumperNode("exitLabel", node.ExitLabel, Nothing) })
		End Function

		Public Overrides Function VisitWithLValueExpressionPlaceholder(ByVal node As BoundWithLValueExpressionPlaceholder, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("withLValueExpressionPlaceholder", Nothing, New TreeDumperNode() { New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitWithRValueExpressionPlaceholder(ByVal node As BoundWithRValueExpressionPlaceholder, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("withRValueExpressionPlaceholder", Nothing, New TreeDumperNode() { New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitWithStatement(ByVal node As BoundWithStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("withStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("originalExpression", Nothing, New TreeDumperNode() { Me.Visit(node.OriginalExpression, Nothing) }), New TreeDumperNode("body", Nothing, New TreeDumperNode() { Me.Visit(node.Body, Nothing) }), New TreeDumperNode("binder", node.Binder, Nothing) })
		End Function

		Public Overrides Function VisitXmlAttribute(ByVal node As BoundXmlAttribute, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("xmlAttribute", Nothing, New TreeDumperNode() { New TreeDumperNode("name", Nothing, New TreeDumperNode() { Me.Visit(node.Name, Nothing) }), New TreeDumperNode("value", Nothing, New TreeDumperNode() { Me.Visit(node.Value, Nothing) }), New TreeDumperNode("matchesImport", node.MatchesImport, Nothing), New TreeDumperNode("objectCreation", Nothing, New TreeDumperNode() { Me.Visit(node.ObjectCreation, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitXmlCData(ByVal node As BoundXmlCData, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("xmlCData", Nothing, New TreeDumperNode() { New TreeDumperNode("value", Nothing, New TreeDumperNode() { Me.Visit(node.Value, Nothing) }), New TreeDumperNode("objectCreation", Nothing, New TreeDumperNode() { Me.Visit(node.ObjectCreation, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitXmlComment(ByVal node As BoundXmlComment, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("xmlComment", Nothing, New TreeDumperNode() { New TreeDumperNode("value", Nothing, New TreeDumperNode() { Me.Visit(node.Value, Nothing) }), New TreeDumperNode("objectCreation", Nothing, New TreeDumperNode() { Me.Visit(node.ObjectCreation, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitXmlDeclaration(ByVal node As BoundXmlDeclaration, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("xmlDeclaration", Nothing, New TreeDumperNode() { New TreeDumperNode("version", Nothing, New TreeDumperNode() { Me.Visit(node.Version, Nothing) }), New TreeDumperNode("encoding", Nothing, New TreeDumperNode() { Me.Visit(node.Encoding, Nothing) }), New TreeDumperNode("standalone", Nothing, New TreeDumperNode() { Me.Visit(node.Standalone, Nothing) }), New TreeDumperNode("objectCreation", Nothing, New TreeDumperNode() { Me.Visit(node.ObjectCreation, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitXmlDocument(ByVal node As BoundXmlDocument, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("xmlDocument", Nothing, New TreeDumperNode() { New TreeDumperNode("declaration", Nothing, New TreeDumperNode() { Me.Visit(node.Declaration, Nothing) }), New TreeDumperNode("childNodes", Nothing, node.ChildNodes.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("rewriterInfo", node.RewriterInfo, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitXmlElement(ByVal node As BoundXmlElement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("xmlElement", Nothing, New TreeDumperNode() { New TreeDumperNode("argument", Nothing, New TreeDumperNode() { Me.Visit(node.Argument, Nothing) }), New TreeDumperNode("childNodes", Nothing, node.ChildNodes.[Select](Of TreeDumperNode)(Function(x As BoundExpression) Me.Visit(x, Nothing))), New TreeDumperNode("rewriterInfo", node.RewriterInfo, Nothing), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitXmlEmbeddedExpression(ByVal node As BoundXmlEmbeddedExpression, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("xmlEmbeddedExpression", Nothing, New TreeDumperNode() { New TreeDumperNode("expression", Nothing, New TreeDumperNode() { Me.Visit(node.Expression, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitXmlMemberAccess(ByVal node As BoundXmlMemberAccess, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("xmlMemberAccess", Nothing, New TreeDumperNode() { New TreeDumperNode("memberAccess", Nothing, New TreeDumperNode() { Me.Visit(node.MemberAccess, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitXmlName(ByVal node As BoundXmlName, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("xmlName", Nothing, New TreeDumperNode() { New TreeDumperNode("xmlNamespace", Nothing, New TreeDumperNode() { Me.Visit(node.XmlNamespace, Nothing) }), New TreeDumperNode("localName", Nothing, New TreeDumperNode() { Me.Visit(node.LocalName, Nothing) }), New TreeDumperNode("objectCreation", Nothing, New TreeDumperNode() { Me.Visit(node.ObjectCreation, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitXmlNamespace(ByVal node As BoundXmlNamespace, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("xmlNamespace", Nothing, New TreeDumperNode() { New TreeDumperNode("xmlNamespace", Nothing, New TreeDumperNode() { Me.Visit(node.XmlNamespace, Nothing) }), New TreeDumperNode("objectCreation", Nothing, New TreeDumperNode() { Me.Visit(node.ObjectCreation, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitXmlProcessingInstruction(ByVal node As BoundXmlProcessingInstruction, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("xmlProcessingInstruction", Nothing, New TreeDumperNode() { New TreeDumperNode("target", Nothing, New TreeDumperNode() { Me.Visit(node.Target, Nothing) }), New TreeDumperNode("data", Nothing, New TreeDumperNode() { Me.Visit(node.Data, Nothing) }), New TreeDumperNode("objectCreation", Nothing, New TreeDumperNode() { Me.Visit(node.ObjectCreation, Nothing) }), New TreeDumperNode("type", node.Type, Nothing) })
		End Function

		Public Overrides Function VisitYieldStatement(ByVal node As BoundYieldStatement, ByVal arg As Object) As TreeDumperNode
			Return New TreeDumperNode("yieldStatement", Nothing, New TreeDumperNode() { New TreeDumperNode("expression", Nothing, New TreeDumperNode() { Me.Visit(node.Expression, Nothing) }) })
		End Function
	End Class
End Namespace