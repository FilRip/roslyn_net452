Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.Operations
	Friend NotInheritable Class VisualBasicOperationFactory
		Private _lazyPlaceholderToParentMap As ConcurrentDictionary(Of BoundValuePlaceholderBase, BoundNode)

		Private ReadOnly _semanticModel As SemanticModel

		Public Sub New(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel)
			MyBase.New()
			Me._lazyPlaceholderToParentMap = Nothing
			Me._semanticModel = semanticModel
		End Sub

		Public Function Create(ByVal boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As IOperation
			Dim rValueWrapper As IOperation
			Dim constantValueOpt As Microsoft.CodeAnalysis.ConstantValue
			If (boundNode IsNot Nothing) Then
				Select Case boundNode.Kind
					Case BoundKind.TypeArguments
						rValueWrapper = Me.CreateBoundTypeArgumentsOperation(DirectCast(boundNode, BoundTypeArguments))
						Exit Select
					Case BoundKind.OmittedArgument
						rValueWrapper = Me.CreateBoundOmittedArgumentOperation(DirectCast(boundNode, BoundOmittedArgument))
						Exit Select
					Case BoundKind.LValueToRValueWrapper
						rValueWrapper = Me.CreateBoundLValueToRValueWrapper(DirectCast(boundNode, BoundLValueToRValueWrapper))
						Exit Select
					Case BoundKind.WithLValueExpressionPlaceholder
						rValueWrapper = Me.CreateBoundWithLValueExpressionPlaceholder(DirectCast(boundNode, BoundWithLValueExpressionPlaceholder))
						Exit Select
					Case BoundKind.WithRValueExpressionPlaceholder
						rValueWrapper = Me.CreateBoundWithRValueExpressionPlaceholder(DirectCast(boundNode, BoundWithRValueExpressionPlaceholder))
						Exit Select
					Case BoundKind.RValuePlaceholder
						rValueWrapper = Me.CreateBoundRValuePlaceholderOperation(DirectCast(boundNode, BoundRValuePlaceholder))
						Exit Select
					Case BoundKind.LValuePlaceholder
					Case BoundKind.Dup
					Case BoundKind.ArrayLength
					Case BoundKind.FieldInfo
					Case BoundKind.MethodInfo
					Case BoundKind.MethodDefIndex
					Case BoundKind.MaximumMethodDefIndex
					Case BoundKind.InstrumentationPayloadRoot
					Case BoundKind.ModuleVersionId
					Case BoundKind.ModuleVersionIdString
					Case BoundKind.SourceDocumentIndex
					Case BoundKind.ReferenceAssignment
					Case BoundKind.RelaxationLambda
					Case BoundKind.ConvertedTupleElements
					Case BoundKind.UserDefinedConversion
					Case BoundKind.SequencePoint
					Case BoundKind.SequencePointExpression
					Case BoundKind.SequencePointWithSpan
					Case BoundKind.StateMachineScope
					Case BoundKind.AsNewLocalDeclarations
					Case BoundKind.Initializer
					Case BoundKind.GlobalStatementInitializer
					Case BoundKind.Sequence
					Case BoundKind.CaseStatement
					Case BoundKind.ForToUserDefinedOperators
					Case BoundKind.ValueTypeMeReference
					Case BoundKind.PreviousSubmissionReference
					Case BoundKind.HostObjectMemberReference
					Case BoundKind.PseudoVariable
					Case BoundKind.ByRefArgumentPlaceholder
					Case BoundKind.LateBoundArgumentSupportingAssignmentWithCapture
					Case BoundKind.StatementList
					Case BoundKind.ConditionalGoto
					Case BoundKind.GroupTypeInferenceLambda
					Case BoundKind.XmlName
					Case BoundKind.XmlDeclaration
					Case BoundKind.XmlAttribute
					Case BoundKind.UnstructuredExceptionHandlingCatchFilter
					Case BoundKind.UnstructuredExceptionOnErrorSwitch
					Case BoundKind.UnstructuredExceptionResumeSwitch
					Case BoundKind.SpillSequence
					Case BoundKind.LoweredConditionalAccess
					Case BoundKind.ComplexConditionalAccessReceiver
						Throw ExceptionUtilities.UnexpectedValue(boundNode.Kind)
					Case BoundKind.BadExpression
						rValueWrapper = Me.CreateBoundBadExpressionOperation(DirectCast(boundNode, BoundBadExpression))
						Exit Select
					Case BoundKind.BadStatement
						rValueWrapper = Me.CreateBoundBadStatementOperation(DirectCast(boundNode, BoundBadStatement))
						Exit Select
					Case BoundKind.Parenthesized
						rValueWrapper = Me.CreateBoundParenthesizedOperation(DirectCast(boundNode, BoundParenthesized))
						Exit Select
					Case BoundKind.BadVariable
						rValueWrapper = Me.Create(DirectCast(boundNode, BoundBadVariable).Expression)
						Exit Select
					Case BoundKind.ArrayAccess
						rValueWrapper = Me.CreateBoundArrayAccessOperation(DirectCast(boundNode, BoundArrayAccess))
						Exit Select
					Case BoundKind.[GetType]
						rValueWrapper = Me.CreateBoundGetTypeOperation(DirectCast(boundNode, BoundGetType))
						Exit Select
					Case BoundKind.TypeExpression
					Case BoundKind.TypeOrValueExpression
					Case BoundKind.NamespaceExpression
					Case BoundKind.CompoundAssignmentTargetPlaceholder
					Case BoundKind.AddressOfOperator
					Case BoundKind.MethodGroup
					Case BoundKind.PropertyGroup
					Case BoundKind.EraseStatement
					Case BoundKind.Attribute
					Case BoundKind.LateAddressOfOperator
					Case BoundKind.ArrayLiteral
					Case BoundKind.ByRefArgumentWithCopyBack
					Case BoundKind.Label
					Case BoundKind.UnboundLambda
					Case BoundKind.RangeVariable
					Case BoundKind.XmlNamespace
					Case BoundKind.XmlDocument
					Case BoundKind.XmlProcessingInstruction
					Case BoundKind.XmlComment
					Case BoundKind.XmlElement
					Case BoundKind.XmlMemberAccess
					Case BoundKind.XmlEmbeddedExpression
					Case BoundKind.XmlCData
					Case BoundKind.ResumeStatement
					Case BoundKind.OnErrorStatement
					Case BoundKind.UnstructuredExceptionHandlingStatement
					Case BoundKind.MidResult
					Case BoundKind.TypeAsValueExpression
						Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = TryCast(boundNode, Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
						If (boundExpression IsNot Nothing) Then
							constantValueOpt = boundExpression.ConstantValueOpt
						Else
							constantValueOpt = Nothing
						End If
						Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = constantValueOpt
						Dim wasCompilerGenerated As Boolean = boundNode.WasCompilerGenerated
						rValueWrapper = New NoneOperation(Me.GetIOperationChildren(boundNode), Me._semanticModel, boundNode.Syntax, Nothing, constantValue, wasCompilerGenerated)
						Exit Select
					Case BoundKind.UnaryOperator
						rValueWrapper = Me.CreateBoundUnaryOperatorOperation(DirectCast(boundNode, BoundUnaryOperator))
						Exit Select
					Case BoundKind.UserDefinedUnaryOperator
						rValueWrapper = Me.CreateBoundUserDefinedUnaryOperatorOperation(DirectCast(boundNode, BoundUserDefinedUnaryOperator))
						Exit Select
					Case BoundKind.NullableIsTrueOperator
						rValueWrapper = Me.CreateBoundNullableIsTrueOperator(DirectCast(boundNode, BoundNullableIsTrueOperator))
						Exit Select
					Case BoundKind.BinaryOperator
						rValueWrapper = Me.CreateBoundBinaryOperatorOperation(DirectCast(boundNode, BoundBinaryOperator))
						Exit Select
					Case BoundKind.UserDefinedBinaryOperator
						rValueWrapper = Me.CreateBoundUserDefinedBinaryOperatorOperation(DirectCast(boundNode, BoundUserDefinedBinaryOperator))
						Exit Select
					Case BoundKind.UserDefinedShortCircuitingOperator
						rValueWrapper = Me.CreateBoundUserDefinedShortCircuitingOperatorOperation(DirectCast(boundNode, BoundUserDefinedShortCircuitingOperator))
						Exit Select
					Case BoundKind.AssignmentOperator
						rValueWrapper = Me.CreateBoundAssignmentOperatorOperation(DirectCast(boundNode, BoundAssignmentOperator))
						Exit Select
					Case BoundKind.TernaryConditionalExpression
						rValueWrapper = Me.CreateBoundTernaryConditionalExpressionOperation(DirectCast(boundNode, BoundTernaryConditionalExpression))
						Exit Select
					Case BoundKind.BinaryConditionalExpression
						rValueWrapper = Me.CreateBoundBinaryConditionalExpressionOperation(DirectCast(boundNode, BoundBinaryConditionalExpression))
						Exit Select
					Case BoundKind.Conversion
						rValueWrapper = Me.CreateBoundConversionOperation(DirectCast(boundNode, BoundConversion))
						Exit Select
					Case BoundKind.[DirectCast]
						rValueWrapper = Me.CreateBoundDirectCastOperation(DirectCast(boundNode, BoundDirectCast))
						Exit Select
					Case BoundKind.[TryCast]
						rValueWrapper = Me.CreateBoundTryCastOperation(DirectCast(boundNode, BoundTryCast))
						Exit Select
					Case BoundKind.[TypeOf]
						rValueWrapper = Me.CreateBoundTypeOfOperation(DirectCast(boundNode, BoundTypeOf))
						Exit Select
					Case BoundKind.NoOpStatement
						rValueWrapper = Me.CreateBoundNoOpStatementOperation(DirectCast(boundNode, BoundNoOpStatement))
						Exit Select
					Case BoundKind.ReturnStatement
						rValueWrapper = Me.CreateBoundReturnStatementOperation(DirectCast(boundNode, BoundReturnStatement))
						Exit Select
					Case BoundKind.YieldStatement
						rValueWrapper = Me.CreateBoundYieldStatementOperation(DirectCast(boundNode, BoundYieldStatement))
						Exit Select
					Case BoundKind.ThrowStatement
						rValueWrapper = Me.CreateBoundThrowStatementOperation(DirectCast(boundNode, BoundThrowStatement))
						Exit Select
					Case BoundKind.RedimStatement
						rValueWrapper = Me.CreateBoundReDimOperation(DirectCast(boundNode, BoundRedimStatement))
						Exit Select
					Case BoundKind.RedimClause
						rValueWrapper = Me.CreateBoundReDimClauseOperation(DirectCast(boundNode, BoundRedimClause))
						Exit Select
					Case BoundKind.[Call]
						rValueWrapper = Me.CreateBoundCallOperation(DirectCast(boundNode, BoundCall))
						Exit Select
					Case BoundKind.LateMemberAccess
						rValueWrapper = Me.CreateBoundLateMemberAccessOperation(DirectCast(boundNode, BoundLateMemberAccess))
						Exit Select
					Case BoundKind.LateInvocation
						rValueWrapper = Me.CreateBoundLateInvocationOperation(DirectCast(boundNode, BoundLateInvocation))
						Exit Select
					Case BoundKind.TupleLiteral
						rValueWrapper = Me.CreateBoundTupleLiteralOperation(DirectCast(boundNode, BoundTupleLiteral))
						Exit Select
					Case BoundKind.ConvertedTupleLiteral
						rValueWrapper = Me.CreateBoundConvertedTupleLiteralOperation(DirectCast(boundNode, BoundConvertedTupleLiteral))
						Exit Select
					Case BoundKind.ObjectCreationExpression
						rValueWrapper = Me.CreateBoundObjectCreationExpressionOperation(DirectCast(boundNode, BoundObjectCreationExpression))
						Exit Select
					Case BoundKind.NoPiaObjectCreationExpression
						rValueWrapper = Me.CreateNoPiaObjectCreationExpressionOperation(DirectCast(boundNode, BoundNoPiaObjectCreationExpression))
						Exit Select
					Case BoundKind.AnonymousTypeCreationExpression
						rValueWrapper = Me.CreateBoundAnonymousTypeCreationExpressionOperation(DirectCast(boundNode, BoundAnonymousTypeCreationExpression))
						Exit Select
					Case BoundKind.AnonymousTypePropertyAccess
						rValueWrapper = Me.CreateBoundAnonymousTypePropertyAccessOperation(DirectCast(boundNode, BoundAnonymousTypePropertyAccess))
						Exit Select
					Case BoundKind.AnonymousTypeFieldInitializer
						rValueWrapper = Me.Create(DirectCast(boundNode, BoundAnonymousTypeFieldInitializer).Value)
						Exit Select
					Case BoundKind.ObjectInitializerExpression
						rValueWrapper = Me.CreateBoundObjectInitializerExpressionOperation(DirectCast(boundNode, BoundObjectInitializerExpression))
						Exit Select
					Case BoundKind.CollectionInitializerExpression
						rValueWrapper = Me.CreateBoundCollectionInitializerExpressionOperation(DirectCast(boundNode, BoundCollectionInitializerExpression))
						Exit Select
					Case BoundKind.NewT
						rValueWrapper = Me.CreateBoundNewTOperation(DirectCast(boundNode, BoundNewT))
						Exit Select
					Case BoundKind.DelegateCreationExpression
						rValueWrapper = Me.CreateBoundDelegateCreationExpressionOperation(DirectCast(boundNode, BoundDelegateCreationExpression))
						Exit Select
					Case BoundKind.ArrayCreation
						rValueWrapper = Me.CreateBoundArrayCreationOperation(DirectCast(boundNode, BoundArrayCreation))
						Exit Select
					Case BoundKind.ArrayInitialization
						rValueWrapper = Me.CreateBoundArrayInitializationOperation(DirectCast(boundNode, BoundArrayInitialization))
						Exit Select
					Case BoundKind.FieldAccess
						rValueWrapper = Me.CreateBoundFieldAccessOperation(DirectCast(boundNode, BoundFieldAccess))
						Exit Select
					Case BoundKind.PropertyAccess
						rValueWrapper = Me.CreateBoundPropertyAccessOperation(DirectCast(boundNode, BoundPropertyAccess))
						Exit Select
					Case BoundKind.EventAccess
						rValueWrapper = Me.CreateBoundEventAccessOperation(DirectCast(boundNode, BoundEventAccess))
						Exit Select
					Case BoundKind.Block
						rValueWrapper = Me.CreateBoundBlockOperation(DirectCast(boundNode, BoundBlock))
						Exit Select
					Case BoundKind.LocalDeclaration
						rValueWrapper = Me.CreateBoundLocalDeclarationOperation(DirectCast(boundNode, BoundLocalDeclaration))
						Exit Select
					Case BoundKind.DimStatement
						rValueWrapper = Me.CreateBoundDimStatementOperation(DirectCast(boundNode, BoundDimStatement))
						Exit Select
					Case BoundKind.FieldInitializer
						rValueWrapper = Me.CreateBoundFieldInitializerOperation(DirectCast(boundNode, BoundFieldInitializer))
						Exit Select
					Case BoundKind.PropertyInitializer
						rValueWrapper = Me.CreateBoundPropertyInitializerOperation(DirectCast(boundNode, BoundPropertyInitializer))
						Exit Select
					Case BoundKind.ParameterEqualsValue
						rValueWrapper = Me.CreateBoundParameterEqualsValueOperation(DirectCast(boundNode, BoundParameterEqualsValue))
						Exit Select
					Case BoundKind.ExpressionStatement
						rValueWrapper = Me.CreateBoundExpressionStatementOperation(DirectCast(boundNode, BoundExpressionStatement))
						Exit Select
					Case BoundKind.IfStatement
						rValueWrapper = Me.CreateBoundIfStatementOperation(DirectCast(boundNode, BoundIfStatement))
						Exit Select
					Case BoundKind.SelectStatement
						rValueWrapper = Me.CreateBoundSelectStatementOperation(DirectCast(boundNode, BoundSelectStatement))
						Exit Select
					Case BoundKind.CaseBlock
						rValueWrapper = Me.CreateBoundCaseBlockOperation(DirectCast(boundNode, BoundCaseBlock))
						Exit Select
					Case BoundKind.SimpleCaseClause
						rValueWrapper = Me.CreateBoundSimpleCaseClauseOperation(DirectCast(boundNode, BoundSimpleCaseClause))
						Exit Select
					Case BoundKind.RangeCaseClause
						rValueWrapper = Me.CreateBoundRangeCaseClauseOperation(DirectCast(boundNode, BoundRangeCaseClause))
						Exit Select
					Case BoundKind.RelationalCaseClause
						rValueWrapper = Me.CreateBoundRelationalCaseClauseOperation(DirectCast(boundNode, BoundRelationalCaseClause))
						Exit Select
					Case BoundKind.DoLoopStatement
						rValueWrapper = Me.CreateBoundDoLoopStatementOperation(DirectCast(boundNode, BoundDoLoopStatement))
						Exit Select
					Case BoundKind.WhileStatement
						rValueWrapper = Me.CreateBoundWhileStatementOperation(DirectCast(boundNode, BoundWhileStatement))
						Exit Select
					Case BoundKind.ForToStatement
						rValueWrapper = Me.CreateBoundForToStatementOperation(DirectCast(boundNode, BoundForToStatement))
						Exit Select
					Case BoundKind.ForEachStatement
						rValueWrapper = Me.CreateBoundForEachStatementOperation(DirectCast(boundNode, BoundForEachStatement))
						Exit Select
					Case BoundKind.ExitStatement
						rValueWrapper = Me.CreateBoundExitStatementOperation(DirectCast(boundNode, BoundExitStatement))
						Exit Select
					Case BoundKind.ContinueStatement
						rValueWrapper = Me.CreateBoundContinueStatementOperation(DirectCast(boundNode, BoundContinueStatement))
						Exit Select
					Case BoundKind.TryStatement
						rValueWrapper = Me.CreateBoundTryStatementOperation(DirectCast(boundNode, BoundTryStatement))
						Exit Select
					Case BoundKind.CatchBlock
						rValueWrapper = Me.CreateBoundCatchBlockOperation(DirectCast(boundNode, BoundCatchBlock))
						Exit Select
					Case BoundKind.Literal
						rValueWrapper = Me.CreateBoundLiteralOperation(DirectCast(boundNode, BoundLiteral), False)
						Exit Select
					Case BoundKind.MeReference
						rValueWrapper = Me.CreateBoundMeReferenceOperation(DirectCast(boundNode, BoundMeReference))
						Exit Select
					Case BoundKind.MyBaseReference
						rValueWrapper = Me.CreateBoundMyBaseReferenceOperation(DirectCast(boundNode, BoundMyBaseReference))
						Exit Select
					Case BoundKind.MyClassReference
						rValueWrapper = Me.CreateBoundMyClassReferenceOperation(DirectCast(boundNode, BoundMyClassReference))
						Exit Select
					Case BoundKind.Local
						rValueWrapper = Me.CreateBoundLocalOperation(DirectCast(boundNode, BoundLocal))
						Exit Select
					Case BoundKind.Parameter
						rValueWrapper = Me.CreateBoundParameterOperation(DirectCast(boundNode, BoundParameter))
						Exit Select
					Case BoundKind.LabelStatement
						rValueWrapper = Me.CreateBoundLabelStatementOperation(DirectCast(boundNode, BoundLabelStatement))
						Exit Select
					Case BoundKind.GotoStatement
						rValueWrapper = Me.CreateBoundGotoStatementOperation(DirectCast(boundNode, BoundGotoStatement))
						Exit Select
					Case BoundKind.WithStatement
						rValueWrapper = Me.CreateBoundWithStatementOperation(DirectCast(boundNode, BoundWithStatement))
						Exit Select
					Case BoundKind.Lambda
						rValueWrapper = Me.CreateBoundLambdaOperation(DirectCast(boundNode, BoundLambda))
						Exit Select
					Case BoundKind.QueryExpression
						rValueWrapper = Me.CreateBoundQueryExpressionOperation(DirectCast(boundNode, BoundQueryExpression))
						Exit Select
					Case BoundKind.QuerySource
						rValueWrapper = Me.Create(DirectCast(boundNode, BoundQuerySource).Expression)
						Exit Select
					Case BoundKind.ToQueryableCollectionConversion
						rValueWrapper = Me.Create(DirectCast(boundNode, BoundToQueryableCollectionConversion).ConversionCall)
						Exit Select
					Case BoundKind.QueryableSource
						rValueWrapper = Me.Create(DirectCast(boundNode, BoundQueryableSource).Source)
						Exit Select
					Case BoundKind.QueryClause
						rValueWrapper = Me.Create(DirectCast(boundNode, BoundQueryClause).UnderlyingExpression)
						Exit Select
					Case BoundKind.Ordering
						rValueWrapper = Me.Create(DirectCast(boundNode, BoundOrdering).UnderlyingExpression)
						Exit Select
					Case BoundKind.QueryLambda
						rValueWrapper = Me.Create(VisualBasicOperationFactory.RewriteQueryLambda(DirectCast(boundNode, BoundQueryLambda)))
						Exit Select
					Case BoundKind.RangeVariableAssignment
						rValueWrapper = Me.Create(DirectCast(boundNode, BoundRangeVariableAssignment).Value)
						Exit Select
					Case BoundKind.AggregateClause
						rValueWrapper = Me.CreateBoundAggregateClauseOperation(DirectCast(boundNode, BoundAggregateClause))
						Exit Select
					Case BoundKind.GroupAggregation
						rValueWrapper = Me.Create(DirectCast(boundNode, BoundGroupAggregation).Group)
						Exit Select
					Case BoundKind.AddHandlerStatement
						rValueWrapper = Me.CreateBoundAddHandlerStatementOperation(DirectCast(boundNode, BoundAddHandlerStatement))
						Exit Select
					Case BoundKind.RemoveHandlerStatement
						rValueWrapper = Me.CreateBoundRemoveHandlerStatementOperation(DirectCast(boundNode, BoundRemoveHandlerStatement))
						Exit Select
					Case BoundKind.RaiseEventStatement
						rValueWrapper = Me.CreateBoundRaiseEventStatementOperation(DirectCast(boundNode, BoundRaiseEventStatement))
						Exit Select
					Case BoundKind.UsingStatement
						rValueWrapper = Me.CreateBoundUsingStatementOperation(DirectCast(boundNode, BoundUsingStatement))
						Exit Select
					Case BoundKind.SyncLockStatement
						rValueWrapper = Me.CreateBoundSyncLockStatementOperation(DirectCast(boundNode, BoundSyncLockStatement))
						Exit Select
					Case BoundKind.AwaitOperator
						rValueWrapper = Me.CreateBoundAwaitOperatorOperation(DirectCast(boundNode, BoundAwaitOperator))
						Exit Select
					Case BoundKind.StopStatement
						rValueWrapper = Me.CreateBoundStopStatementOperation(DirectCast(boundNode, BoundStopStatement))
						Exit Select
					Case BoundKind.EndStatement
						rValueWrapper = Me.CreateBoundEndStatementOperation(DirectCast(boundNode, BoundEndStatement))
						Exit Select
					Case BoundKind.ConditionalAccess
						rValueWrapper = Me.CreateBoundConditionalAccessOperation(DirectCast(boundNode, BoundConditionalAccess))
						Exit Select
					Case BoundKind.ConditionalAccessReceiverPlaceholder
						rValueWrapper = Me.CreateBoundConditionalAccessReceiverPlaceholderOperation(DirectCast(boundNode, BoundConditionalAccessReceiverPlaceholder))
						Exit Select
					Case BoundKind.NameOfOperator
						rValueWrapper = Me.CreateBoundNameOfOperatorOperation(DirectCast(boundNode, BoundNameOfOperator))
						Exit Select
					Case BoundKind.InterpolatedStringExpression
						rValueWrapper = Me.CreateBoundInterpolatedStringExpressionOperation(DirectCast(boundNode, BoundInterpolatedStringExpression))
						Exit Select
					Case BoundKind.Interpolation
						rValueWrapper = Me.CreateBoundInterpolationOperation(DirectCast(boundNode, BoundInterpolation))
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(boundNode.Kind)
				End Select
			Else
				rValueWrapper = Nothing
			End If
			Return rValueWrapper
		End Function

		Private Function CreateAnonymousTypePropertyAccessImplicitReceiverOperation(ByVal propertySym As IPropertySymbol, ByVal syntax As SyntaxNode) As InstanceReferenceOperation
			Return New InstanceReferenceOperation(InstanceReferenceKind.ImplicitReceiver, Me._semanticModel, syntax, propertySym.ContainingType, True)
		End Function

		Private Function CreateArgumentOperation(ByVal kind As ArgumentKind, ByVal parameter As IParameterSymbol, ByVal valueNode As BoundNode, ByVal inConversion As Conversion, ByVal outConversion As Conversion, ByVal isImplicit As Boolean) As IArgumentOperation
			Dim syntax As Microsoft.CodeAnalysis.SyntaxNode
			Dim parent As Object
			If (valueNode.Syntax.Kind() = SyntaxKind.OmittedArgument) Then
				syntax = valueNode.Syntax
			Else
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = valueNode.Syntax
				If (syntaxNode IsNot Nothing) Then
					parent = syntaxNode.Parent
				Else
					parent = Nothing
				End If
				syntax = TryCast(parent, ArgumentSyntax)
			End If
			Dim syntax1 As Microsoft.CodeAnalysis.SyntaxNode = syntax
			Dim operation As IOperation = Me.Create(valueNode)
			If (syntax1 Is Nothing) Then
				syntax1 = operation.Syntax
				isImplicit = True
			End If
			Return New ArgumentOperation(kind, parameter, operation, DirectCast(inConversion, IConvertibleConversion), DirectCast(outConversion, IConvertibleConversion), Me._semanticModel, syntax1, isImplicit)
		End Function

		Private Function CreateBoundAddHandlerStatementOperation(ByVal boundAddHandlerStatement As Microsoft.CodeAnalysis.VisualBasic.BoundAddHandlerStatement) As IExpressionStatementOperation
			Dim addRemoveHandlerStatementExpression As IOperation = Me.GetAddRemoveHandlerStatementExpression(boundAddHandlerStatement)
			Dim syntax As SyntaxNode = boundAddHandlerStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundAddHandlerStatement.WasCompilerGenerated
			Return New ExpressionStatementOperation(addRemoveHandlerStatementExpression, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundAggregateClauseOperation(ByVal boundAggregateClause As Microsoft.CodeAnalysis.VisualBasic.BoundAggregateClause) As IOperation
			Dim aggregateQueryOperation As IOperation
			If (boundAggregateClause.CapturedGroupOpt IsNot Nothing) Then
				Me.RecordParent(boundAggregateClause.GroupPlaceholderOpt, boundAggregateClause)
				Dim operation As IOperation = Me.Create(boundAggregateClause.CapturedGroupOpt)
				Dim operation1 As IOperation = Me.Create(boundAggregateClause.UnderlyingExpression)
				Dim syntax As SyntaxNode = boundAggregateClause.Syntax
				Dim type As ITypeSymbol = boundAggregateClause.Type
				Dim wasCompilerGenerated As Boolean = boundAggregateClause.WasCompilerGenerated
				aggregateQueryOperation = New Microsoft.CodeAnalysis.Operations.AggregateQueryOperation(operation, operation1, Me._semanticModel, syntax, type, wasCompilerGenerated)
			Else
				aggregateQueryOperation = Me.Create(boundAggregateClause.UnderlyingExpression)
			End If
			Return aggregateQueryOperation
		End Function

		Private Function CreateBoundAnonymousTypeCreationExpressionOperation(ByVal boundAnonymousTypeCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypeCreationExpression) As IAnonymousObjectCreationOperation
			Dim anonymousTypeCreationInitializers As ImmutableArray(Of IOperation) = Me.GetAnonymousTypeCreationInitializers(boundAnonymousTypeCreationExpression)
			Dim syntax As SyntaxNode = boundAnonymousTypeCreationExpression.Syntax
			Dim type As ITypeSymbol = boundAnonymousTypeCreationExpression.Type
			Dim wasCompilerGenerated As Boolean = boundAnonymousTypeCreationExpression.WasCompilerGenerated
			Return New AnonymousObjectCreationOperation(anonymousTypeCreationInitializers, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundAnonymousTypePropertyAccessOperation(ByVal boundAnonymousTypePropertyAccess As Microsoft.CodeAnalysis.VisualBasic.BoundAnonymousTypePropertyAccess) As IPropertyReferenceOperation
			Dim expressionSymbol As IPropertySymbol = DirectCast(boundAnonymousTypePropertyAccess.ExpressionSymbol, IPropertySymbol)
			Dim operation As IOperation = Me.CreateAnonymousTypePropertyAccessImplicitReceiverOperation(expressionSymbol, boundAnonymousTypePropertyAccess.Syntax.FirstAncestorOrSelf(Of AnonymousObjectCreationExpressionSyntax)(Nothing, True))
			Dim empty As ImmutableArray(Of IArgumentOperation) = ImmutableArray(Of IArgumentOperation).Empty
			Dim syntax As SyntaxNode = boundAnonymousTypePropertyAccess.Syntax
			Dim type As ITypeSymbol = boundAnonymousTypePropertyAccess.Type
			Dim wasCompilerGenerated As Boolean = boundAnonymousTypePropertyAccess.WasCompilerGenerated
			Return New PropertyReferenceOperation(expressionSymbol, empty, operation, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundArrayAccessOperation(ByVal boundArrayAccess As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess) As IArrayElementReferenceOperation
			Dim operation As IOperation = Me.Create(boundArrayAccess.Expression)
			Dim operations As ImmutableArray(Of IOperation) = Me.CreateFromArray(Of BoundExpression, IOperation)(boundArrayAccess.Indices)
			Dim syntax As SyntaxNode = boundArrayAccess.Syntax
			Dim type As ITypeSymbol = boundArrayAccess.Type
			Dim wasCompilerGenerated As Boolean = boundArrayAccess.WasCompilerGenerated
			Return New ArrayElementReferenceOperation(operation, operations, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundArrayCreationOperation(ByVal boundArrayCreation As Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation) As IArrayCreationOperation
			Dim operations As ImmutableArray(Of IOperation) = Me.CreateFromArray(Of BoundExpression, IOperation)(boundArrayCreation.Bounds)
			Dim arrayInitializerOperation As IArrayInitializerOperation = DirectCast(Me.Create(boundArrayCreation.InitializerOpt), IArrayInitializerOperation)
			Dim syntax As SyntaxNode = boundArrayCreation.Syntax
			Dim type As ITypeSymbol = boundArrayCreation.Type
			Dim wasCompilerGenerated As Boolean = boundArrayCreation.WasCompilerGenerated
			Return New ArrayCreationOperation(operations, arrayInitializerOperation, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundArrayInitializationOperation(ByVal boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization) As IArrayInitializerOperation
			Dim operations As ImmutableArray(Of IOperation) = Me.CreateFromArray(Of BoundExpression, IOperation)(boundArrayInitialization.Initializers)
			Dim syntax As SyntaxNode = boundArrayInitialization.Syntax
			Dim wasCompilerGenerated As Boolean = boundArrayInitialization.WasCompilerGenerated
			Return New ArrayInitializerOperation(operations, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundAssignmentOperatorOperation(ByVal boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator) As IOperation
			Dim noneOperation As IOperation
			If (VisualBasicOperationFactory.IsMidStatement(boundAssignmentOperator.Right)) Then
				Dim constantValueOpt As Microsoft.CodeAnalysis.ConstantValue = boundAssignmentOperator.ConstantValueOpt
				Dim wasCompilerGenerated As Boolean = boundAssignmentOperator.WasCompilerGenerated
				noneOperation = New Microsoft.CodeAnalysis.Operations.NoneOperation(Me.GetIOperationChildren(boundAssignmentOperator), Me._semanticModel, boundAssignmentOperator.Syntax, Nothing, constantValueOpt, wasCompilerGenerated)
			ElseIf (boundAssignmentOperator.LeftOnTheRightOpt Is Nothing) Then
				Dim operation As IOperation = Me.Create(boundAssignmentOperator.Left)
				Dim operation1 As IOperation = Me.Create(boundAssignmentOperator.Right)
				Dim flag As Boolean = boundAssignmentOperator.WasCompilerGenerated
				Dim syntax As SyntaxNode = boundAssignmentOperator.Syntax
				Dim type As ITypeSymbol = boundAssignmentOperator.Type
				Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = boundAssignmentOperator.ConstantValueOpt
				noneOperation = New SimpleAssignmentOperation(False, operation, operation1, Me._semanticModel, syntax, type, constantValue, flag)
			Else
				noneOperation = Me.CreateCompoundAssignment(boundAssignmentOperator)
			End If
			Return noneOperation
		End Function

		Private Function CreateBoundAwaitOperatorOperation(ByVal boundAwaitOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAwaitOperator) As IAwaitOperation
			Dim operation As IOperation = Me.Create(boundAwaitOperator.Operand)
			Dim syntax As SyntaxNode = boundAwaitOperator.Syntax
			Dim type As ITypeSymbol = boundAwaitOperator.Type
			Dim wasCompilerGenerated As Boolean = boundAwaitOperator.WasCompilerGenerated
			Return New AwaitOperation(operation, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundBadExpressionOperation(ByVal boundBadExpression As Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression) As IInvalidOperation
			Dim type As ITypeSymbol
			Dim syntax As SyntaxNode = boundBadExpression.Syntax
			If (syntax.IsMissing) Then
				type = Nothing
			Else
				type = boundBadExpression.Type
			End If
			Dim typeSymbol As ITypeSymbol = type
			Dim constantValueOpt As ConstantValue = boundBadExpression.ConstantValueOpt
			Dim flag As Boolean = If(boundBadExpression.WasCompilerGenerated, True, boundBadExpression.ChildBoundNodes.Any(Function(e As BoundExpression) If(e IsNot Nothing, e.Syntax, Nothing) = boundBadExpression.Syntax))
			Return New InvalidOperation(Me.CreateFromArray(Of BoundExpression, IOperation)(boundBadExpression.ChildBoundNodes), Me._semanticModel, syntax, typeSymbol, constantValueOpt, flag)
		End Function

		Private Function CreateBoundBadStatementOperation(ByVal boundBadStatement As Microsoft.CodeAnalysis.VisualBasic.BoundBadStatement) As IInvalidOperation
			Dim syntax As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = boundBadStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundBadStatement.WasCompilerGenerated
			If (Not wasCompilerGenerated) Then
				Dim enumerator As ImmutableArray(Of BoundNode).Enumerator = boundBadStatement.ChildBoundNodes.GetEnumerator()
				Do
					If (Not enumerator.MoveNext()) Then
						Return New InvalidOperation(Me.CreateFromArray(Of BoundNode, IOperation)(boundBadStatement.ChildBoundNodes), Me._semanticModel, syntaxNode, Nothing, Nothing, wasCompilerGenerated)
					End If
					Dim current As BoundNode = enumerator.Current
					If (current IsNot Nothing) Then
						syntax = current.Syntax
					Else
						syntax = Nothing
					End If
				Loop While syntax <> boundBadStatement.Syntax
				wasCompilerGenerated = True
			End If
			Return New InvalidOperation(Me.CreateFromArray(Of BoundNode, IOperation)(boundBadStatement.ChildBoundNodes), Me._semanticModel, syntaxNode, Nothing, Nothing, wasCompilerGenerated)
		End Function

		Private Function CreateBoundBinaryConditionalExpressionOperation(ByVal boundBinaryConditionalExpression As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryConditionalExpression) As ICoalesceOperation
			Dim operation As IOperation = Me.Create(boundBinaryConditionalExpression.TestExpression)
			Dim operation1 As IOperation = Me.Create(boundBinaryConditionalExpression.ElseExpression)
			Dim syntax As SyntaxNode = boundBinaryConditionalExpression.Syntax
			Dim type As ITypeSymbol = boundBinaryConditionalExpression.Type
			Dim constantValueOpt As ConstantValue = boundBinaryConditionalExpression.ConstantValueOpt
			Dim wasCompilerGenerated As Boolean = boundBinaryConditionalExpression.WasCompilerGenerated
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(Conversions.Identity)
			If (Not TypeSymbol.Equals(boundBinaryConditionalExpression.Type, boundBinaryConditionalExpression.TestExpression.Type, TypeCompareKind.ConsiderEverything)) Then
				Dim convertedTestExpression As BoundExpression = boundBinaryConditionalExpression.ConvertedTestExpression
				If (convertedTestExpression IsNot Nothing) Then
					conversion = If(convertedTestExpression.Kind <> BoundKind.Conversion, New Microsoft.CodeAnalysis.VisualBasic.Conversion(), VisualBasicOperationFactory.CreateConversion(convertedTestExpression))
				End If
			End If
			Return New CoalesceOperation(operation, operation1, DirectCast(conversion, IConvertibleConversion), Me._semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated)
		End Function

		Friend Function CreateBoundBinaryOperatorChild(ByVal binaryOperator As BoundExpression, ByVal isLeft As Boolean) As IOperation
			Dim userDefinedBinaryOperatorChild As IOperation
			Dim kind As BoundKind = binaryOperator.Kind
			If (kind = BoundKind.UserDefinedBinaryOperator) Then
				Dim boundUserDefinedBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator = DirectCast(binaryOperator, Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator)
				Dim userDefinedBinaryOperatorInfo As VisualBasicOperationFactory.BinaryOperatorInfo = VisualBasicOperationFactory.GetUserDefinedBinaryOperatorInfo(boundUserDefinedBinaryOperator)
				userDefinedBinaryOperatorChild = Me.GetUserDefinedBinaryOperatorChild(boundUserDefinedBinaryOperator, If(isLeft, userDefinedBinaryOperatorInfo.LeftOperand, userDefinedBinaryOperatorInfo.RightOperand))
			Else
				If (kind <> BoundKind.UserDefinedShortCircuitingOperator) Then
					Throw ExceptionUtilities.UnexpectedValue(binaryOperator.Kind)
				End If
				Dim boundUserDefinedShortCircuitingOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedShortCircuitingOperator = DirectCast(binaryOperator, Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedShortCircuitingOperator)
				Dim binaryOperatorInfo As VisualBasicOperationFactory.BinaryOperatorInfo = VisualBasicOperationFactory.GetUserDefinedBinaryOperatorInfo(boundUserDefinedShortCircuitingOperator.BitwiseOperator)
				If (Not isLeft) Then
					userDefinedBinaryOperatorChild = Me.GetUserDefinedBinaryOperatorChild(boundUserDefinedShortCircuitingOperator.BitwiseOperator, binaryOperatorInfo.RightOperand)
				Else
					userDefinedBinaryOperatorChild = If(boundUserDefinedShortCircuitingOperator.LeftOperand IsNot Nothing, Me.Create(boundUserDefinedShortCircuitingOperator.LeftOperand), Me.GetUserDefinedBinaryOperatorChild(boundUserDefinedShortCircuitingOperator.BitwiseOperator, binaryOperatorInfo.LeftOperand))
				End If
			End If
			Return userDefinedBinaryOperatorChild
		End Function

		Private Function CreateBoundBinaryOperatorOperation(ByVal boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) As IOperation
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator).GetInstance()
			Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = boundBinaryOperator
			Do
				instance.Push(left)
				left = TryCast(left.Left, Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
			Loop While left IsNot Nothing
			Dim binaryOperation As IOperation = Nothing
			While instance.TryPop(' 
			' Current member / type: Microsoft.CodeAnalysis.IOperation Microsoft.CodeAnalysis.Operations.VisualBasicOperationFactory::CreateBoundBinaryOperatorOperation(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.IOperation CreateBoundBinaryOperatorOperation(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
			' 
			' The unary opperator AddressReference is not supported in VisualBasic
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.ToString(UnaryOperator ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 876
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1268
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 138
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(IList`1 , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2787
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(IList`1 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2713
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 2157
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 126
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1270
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 138
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 288
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3528
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1111
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 90
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1916
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1841
			'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 447
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com


		Private Function CreateBoundBlockOperation(ByVal boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock) As IBlockOperation
			Dim operations As ImmutableArray(Of IOperation) = Me.CreateFromArray(Of BoundStatement, IOperation)(boundBlock.Statements)
			Dim localSymbols As ImmutableArray(Of ILocalSymbol) = boundBlock.Locals.[As](Of ILocalSymbol)()
			Dim syntax As SyntaxNode = boundBlock.Syntax
			Dim wasCompilerGenerated As Boolean = boundBlock.WasCompilerGenerated
			Return New BlockOperation(operations, localSymbols, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundCallOperation(ByVal boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall) As IInvocationOperation
			Dim flag As Boolean
			Dim operation As IOperation
			Dim argumentOperations As ImmutableArray(Of IArgumentOperation)
			Dim syntax As SyntaxNode
			Dim type As ITypeSymbol
			Dim wasCompilerGenerated As Boolean
			Dim kind As Boolean
			Dim receiverOpt As Object
			Dim methodGroupOpt As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup
			Dim kind1 As Boolean
			Dim obj As Object
			Dim boundMethodGroup As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup
			Dim method As IMethodSymbol = boundCall.Method
			If (method IsNot Nothing AndAlso (method.IsVirtual OrElse method.IsAbstract OrElse method.IsOverride)) Then
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundCall.ReceiverOpt
				If (boundExpression IsNot Nothing) Then
					kind1 = boundExpression.Kind <> BoundKind.MyBaseReference
				Else
					kind1 = False
				End If
				If (Not kind1) Then
					GoTo Label1
				End If
				Dim receiverOpt1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundCall.ReceiverOpt
				If (receiverOpt1 IsNot Nothing) Then
					kind = receiverOpt1.Kind <> BoundKind.MyClassReference
					flag = kind
					receiverOpt = boundCall.ReceiverOpt
					If (receiverOpt Is Nothing) Then
						obj = receiverOpt
						methodGroupOpt = boundCall.MethodGroupOpt
						If (methodGroupOpt IsNot Nothing) Then
							receiverOpt = methodGroupOpt.ReceiverOpt
						Else
							boundMethodGroup = methodGroupOpt
							receiverOpt = Nothing
						End If
					End If
					operation = Me.CreateReceiverOperation(receiverOpt, method)
					argumentOperations = Me.DeriveArguments(boundCall)
					syntax = boundCall.Syntax
					type = boundCall.Type
					wasCompilerGenerated = boundCall.WasCompilerGenerated
					Return New InvocationOperation(method, operation, flag, argumentOperations, Me._semanticModel, syntax, type, wasCompilerGenerated)
				Else
					kind = False
					flag = kind
					receiverOpt = boundCall.ReceiverOpt
					If (receiverOpt Is Nothing) Then
						obj = receiverOpt
						methodGroupOpt = boundCall.MethodGroupOpt
						If (methodGroupOpt IsNot Nothing) Then
							receiverOpt = methodGroupOpt.ReceiverOpt
						Else
							boundMethodGroup = methodGroupOpt
							receiverOpt = Nothing
						End If
					End If
					operation = Me.CreateReceiverOperation(receiverOpt, method)
					argumentOperations = Me.DeriveArguments(boundCall)
					syntax = boundCall.Syntax
					type = boundCall.Type
					wasCompilerGenerated = boundCall.WasCompilerGenerated
					Return New InvocationOperation(method, operation, flag, argumentOperations, Me._semanticModel, syntax, type, wasCompilerGenerated)
				End If
			End If
		Label1:
			kind = False
			flag = kind
			receiverOpt = boundCall.ReceiverOpt
			If (receiverOpt Is Nothing) Then
				obj = receiverOpt
				methodGroupOpt = boundCall.MethodGroupOpt
				If (methodGroupOpt IsNot Nothing) Then
					receiverOpt = methodGroupOpt.ReceiverOpt
				Else
					boundMethodGroup = methodGroupOpt
					receiverOpt = Nothing
				End If
			End If
			operation = Me.CreateReceiverOperation(receiverOpt, method)
			argumentOperations = Me.DeriveArguments(boundCall)
			syntax = boundCall.Syntax
			type = boundCall.Type
			wasCompilerGenerated = boundCall.WasCompilerGenerated
			Return New InvocationOperation(method, operation, flag, argumentOperations, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Friend Function CreateBoundCaseBlockBody(ByVal boundCaseBlock As Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock) As ImmutableArray(Of IOperation)
			Return ImmutableArray.Create(Of IOperation)(Me.Create(boundCaseBlock.Body))
		End Function

		Friend Function CreateBoundCaseBlockClauses(ByVal boundCaseBlock As Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock) As ImmutableArray(Of ICaseClauseOperation)
			Dim caseClauseOperations As ImmutableArray(Of ICaseClauseOperation)
			Dim caseStatement As BoundCaseStatement = boundCaseBlock.CaseStatement
			caseClauseOperations = If(Not caseStatement.CaseClauses.IsEmpty OrElse caseStatement.Syntax.Kind() <> SyntaxKind.CaseElseStatement, Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of BoundCaseClause, ICaseClauseOperation)(caseStatement.CaseClauses, Function(n As BoundCaseClause) DirectCast(Me.Create(n), ICaseClauseOperation)), ImmutableArray.Create(Of ICaseClauseOperation)(New DefaultCaseClauseOperation(Nothing, Me._semanticModel, caseStatement.Syntax, boundCaseBlock.WasCompilerGenerated)))
			Return caseClauseOperations
		End Function

		Friend Function CreateBoundCaseBlockCondition(ByVal boundCaseBlock As Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock) As IOperation
			Return Me.Create(boundCaseBlock.CaseStatement.ConditionOpt)
		End Function

		Private Function CreateBoundCaseBlockOperation(ByVal boundCaseBlock As Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock) As ISwitchCaseOperation
			Dim caseClauseOperations As ImmutableArray(Of ICaseClauseOperation) = Me.CreateBoundCaseBlockClauses(boundCaseBlock)
			Dim operations As ImmutableArray(Of IOperation) = ImmutableArray.Create(Of IOperation)(Me.Create(boundCaseBlock.Body))
			Dim operation As IOperation = Me.CreateBoundCaseBlockCondition(boundCaseBlock)
			Dim syntax As SyntaxNode = boundCaseBlock.Syntax
			Dim wasCompilerGenerated As Boolean = boundCaseBlock.WasCompilerGenerated
			Return New SwitchCaseOperation(caseClauseOperations, operations, ImmutableArray(Of ILocalSymbol).Empty, operation, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Friend Function CreateBoundCatchBlockExceptionDeclarationOrExpression(ByVal boundCatchBlock As Microsoft.CodeAnalysis.VisualBasic.BoundCatchBlock) As IOperation
			Dim variableDeclaratorOperation As IOperation
			Dim kind As Boolean
			If (boundCatchBlock.LocalOpt IsNot Nothing) Then
				Dim exceptionSourceOpt As BoundExpression = boundCatchBlock.ExceptionSourceOpt
				If (exceptionSourceOpt IsNot Nothing) Then
					kind = exceptionSourceOpt.Kind = BoundKind.Local
				Else
					kind = False
				End If
				If (Not kind OrElse CObj(boundCatchBlock.LocalOpt) <> CObj(DirectCast(boundCatchBlock.ExceptionSourceOpt, BoundLocal).LocalSymbol)) Then
					variableDeclaratorOperation = Me.Create(boundCatchBlock.ExceptionSourceOpt)
					Return variableDeclaratorOperation
				End If
				variableDeclaratorOperation = New Microsoft.CodeAnalysis.Operations.VariableDeclaratorOperation(boundCatchBlock.LocalOpt, Nothing, ImmutableArray(Of IOperation).Empty, Me._semanticModel, boundCatchBlock.ExceptionSourceOpt.Syntax, False)
				Return variableDeclaratorOperation
			End If
			variableDeclaratorOperation = Me.Create(boundCatchBlock.ExceptionSourceOpt)
			Return variableDeclaratorOperation
		End Function

		Private Function CreateBoundCatchBlockOperation(ByVal boundCatchBlock As Microsoft.CodeAnalysis.VisualBasic.BoundCatchBlock) As ICatchClauseOperation
			Dim type As Object
			Dim empty As ImmutableArray(Of ILocalSymbol)
			Dim operation As IOperation = Me.CreateBoundCatchBlockExceptionDeclarationOrExpression(boundCatchBlock)
			Dim operation1 As IOperation = Me.Create(boundCatchBlock.ExceptionFilterOpt)
			Dim blockOperation As IBlockOperation = DirectCast(Me.Create(boundCatchBlock.Body), IBlockOperation)
			Dim exceptionSourceOpt As BoundExpression = boundCatchBlock.ExceptionSourceOpt
			If (exceptionSourceOpt IsNot Nothing) Then
				type = exceptionSourceOpt.Type
			Else
				type = Nothing
			End If
			If (type Is Nothing) Then
				type = DirectCast(Me._semanticModel.Compilation, VisualBasicCompilation).GetWellKnownType(WellKnownType.System_Exception)
			End If
			Dim typeSymbol As ITypeSymbol = type
			If (boundCatchBlock.LocalOpt IsNot Nothing) Then
				empty = ImmutableArray.Create(Of ILocalSymbol)(boundCatchBlock.LocalOpt)
			Else
				empty = ImmutableArray(Of ILocalSymbol).Empty
			End If
			Dim localSymbols As ImmutableArray(Of ILocalSymbol) = empty
			Dim syntax As SyntaxNode = boundCatchBlock.Syntax
			Dim wasCompilerGenerated As Boolean = boundCatchBlock.WasCompilerGenerated
			Return New CatchClauseOperation(operation, typeSymbol, localSymbols, operation1, blockOperation, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundCollectionInitializerExpressionOperation(ByVal boundCollectionInitializerExpression As Microsoft.CodeAnalysis.VisualBasic.BoundCollectionInitializerExpression) As IObjectOrCollectionInitializerOperation
			Dim operations As ImmutableArray(Of IOperation) = Me.CreateFromArray(Of BoundExpression, IOperation)(boundCollectionInitializerExpression.Initializers)
			Dim syntax As SyntaxNode = boundCollectionInitializerExpression.Syntax
			Dim type As ITypeSymbol = boundCollectionInitializerExpression.Type
			Dim wasCompilerGenerated As Boolean = boundCollectionInitializerExpression.WasCompilerGenerated
			Return New ObjectOrCollectionInitializerOperation(operations, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundConditionalAccessOperation(ByVal boundConditionalAccess As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalAccess) As IConditionalAccessOperation
			Me.RecordParent(boundConditionalAccess.Placeholder, boundConditionalAccess)
			Dim operation As IOperation = Me.Create(boundConditionalAccess.Receiver)
			Dim operation1 As IOperation = Me.Create(boundConditionalAccess.AccessExpression)
			Dim syntax As SyntaxNode = boundConditionalAccess.Syntax
			Dim type As ITypeSymbol = boundConditionalAccess.Type
			Dim wasCompilerGenerated As Boolean = boundConditionalAccess.WasCompilerGenerated
			Return New ConditionalAccessOperation(operation, operation1, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundConditionalAccessReceiverPlaceholderOperation(ByVal boundConditionalAccessReceiverPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalAccessReceiverPlaceholder) As IConditionalAccessInstanceOperation
			Dim syntax As SyntaxNode = boundConditionalAccessReceiverPlaceholder.Syntax
			Dim type As ITypeSymbol = boundConditionalAccessReceiverPlaceholder.Type
			Dim wasCompilerGenerated As Boolean = boundConditionalAccessReceiverPlaceholder.WasCompilerGenerated
			Return New ConditionalAccessInstanceOperation(Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundContinueStatementOperation(ByVal boundContinueStatement As Microsoft.CodeAnalysis.VisualBasic.BoundContinueStatement) As IBranchOperation
			Dim label As LabelSymbol = boundContinueStatement.Label
			Dim branchKind As Microsoft.CodeAnalysis.Operations.BranchKind = Microsoft.CodeAnalysis.Operations.BranchKind.[Continue]
			Dim syntax As SyntaxNode = boundContinueStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundContinueStatement.WasCompilerGenerated
			Return New BranchOperation(label, branchKind, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Friend Function CreateBoundControlVariableOperation(ByVal boundForStatement As Microsoft.CodeAnalysis.VisualBasic.BoundForStatement) As IOperation
			Dim declaredOrInferredLocalOpt As LocalSymbol = boundForStatement.DeclaredOrInferredLocalOpt
			Dim controlVariable As BoundExpression = boundForStatement.ControlVariable
			If (declaredOrInferredLocalOpt Is Nothing) Then
				Return Me.Create(controlVariable)
			End If
			Return New VariableDeclaratorOperation(declaredOrInferredLocalOpt, Nothing, ImmutableArray(Of IOperation).Empty, Me._semanticModel, controlVariable.Syntax, boundForStatement.WasCompilerGenerated)
		End Function

		Private Function CreateBoundConversionOperation(ByVal boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion) As IOperation
			Dim operation As IOperation
			operation = If(Not boundConversion.Syntax.IsMissing OrElse boundConversion.Operand.Kind <> BoundKind.BadExpression, Me.CreateBoundConversionOrCastOperation(boundConversion, False), Me.Create(boundConversion.Operand))
			Return operation
		End Function

		Private Function CreateBoundConversionOrCastOperation(ByVal boundConversionOrCast As Microsoft.CodeAnalysis.VisualBasic.BoundConversionOrCast, ByVal isTryCast As Boolean) As IOperation
			Dim conversionOperation As IOperation
			Dim flag As Boolean = False
			Dim type As ITypeSymbol = boundConversionOrCast.Type
			Dim constantValueOpt As ConstantValue = boundConversionOrCast.ConstantValueOpt
			Dim syntax As SyntaxNode = boundConversionOrCast.Syntax
			Dim flag1 As Boolean = If(boundConversionOrCast.WasCompilerGenerated, True, Not boundConversionOrCast.ExplicitCastInCode)
			Dim conversionOperand As BoundExpression = VisualBasicOperationFactory.GetConversionOperand(boundConversionOrCast)
			If (conversionOperand.Syntax = boundConversionOrCast.Syntax) Then
				If (conversionOperand.Kind = BoundKind.ConvertedTupleLiteral AndAlso TypeSymbol.Equals(conversionOperand.Type, boundConversionOrCast.Type, TypeCompareKind.ConsiderEverything)) Then
					conversionOperation = Me.Create(conversionOperand)
					Return conversionOperation
				End If
				flag1 = True
			End If
			Dim conversionInfo As ValueTuple(Of IOperation, Conversion, Boolean) = Me.GetConversionInfo(boundConversionOrCast)
			Dim item2 As Conversion = conversionInfo.Item2
			If (conversionInfo.Item3 Is Nothing) Then
				conversionOperation = New Microsoft.CodeAnalysis.Operations.ConversionOperation(conversionInfo.Item1, DirectCast(item2, IConvertibleConversion), isTryCast, flag, Me._semanticModel, syntax, type, constantValueOpt, flag1)
			Else
				conversionOperation = New DelegateCreationOperation(conversionInfo.Item1, Me._semanticModel, syntax, type, flag1)
			End If
			Return conversionOperation
		End Function

		Private Function CreateBoundConvertedTupleLiteralOperation(ByVal boundConvertedTupleLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundConvertedTupleLiteral) As ITupleOperation
			Return Me.CreateTupleOperation(boundConvertedTupleLiteral, boundConvertedTupleLiteral.NaturalTypeOpt)
		End Function

		Friend Function CreateBoundDelegateCreationExpressionChildOperation(ByVal boundDelegateCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression) As IMethodReferenceOperation
			Dim method As IMethodSymbol = boundDelegateCreationExpression.Method
			Dim flag As Boolean = If(method Is Nothing OrElse Not method.IsAbstract AndAlso Not method.IsOverride AndAlso Not method.IsVirtual, False, Not boundDelegateCreationExpression.SuppressVirtualCalls)
			Dim receiverOpt As Object = boundDelegateCreationExpression.ReceiverOpt
			If (receiverOpt Is Nothing) Then
				Dim methodGroupOpt As BoundMethodGroup = boundDelegateCreationExpression.MethodGroupOpt
				If (methodGroupOpt IsNot Nothing) Then
					receiverOpt = methodGroupOpt.ReceiverOpt
				Else
					receiverOpt = Nothing
				End If
			End If
			Dim operation As IOperation = Me.CreateReceiverOperation(receiverOpt, method)
			Dim syntax As SyntaxNode = boundDelegateCreationExpression.Syntax
			Dim typeSymbol As ITypeSymbol = Nothing
			Dim wasCompilerGenerated As Boolean = boundDelegateCreationExpression.WasCompilerGenerated
			Return New MethodReferenceOperation(method, flag, operation, Me._semanticModel, syntax, typeSymbol, wasCompilerGenerated)
		End Function

		Private Function CreateBoundDelegateCreationExpressionOperation(ByVal boundDelegateCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression) As IDelegateCreationOperation
			Dim methodReferenceOperation As IMethodReferenceOperation = Me.CreateBoundDelegateCreationExpressionChildOperation(boundDelegateCreationExpression)
			Dim syntax As SyntaxNode = boundDelegateCreationExpression.Syntax
			Dim type As ITypeSymbol = boundDelegateCreationExpression.Type
			Return New DelegateCreationOperation(methodReferenceOperation, Me._semanticModel, syntax, type, True)
		End Function

		Private Function CreateBoundDimStatementOperation(ByVal boundDimStatement As Microsoft.CodeAnalysis.VisualBasic.BoundDimStatement) As IVariableDeclarationGroupOperation
			Dim variableDeclarationStatementVariables As ImmutableArray(Of IVariableDeclarationOperation) = Me.GetVariableDeclarationStatementVariables(boundDimStatement.LocalDeclarations)
			Dim syntax As SyntaxNode = boundDimStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundDimStatement.WasCompilerGenerated
			Return New VariableDeclarationGroupOperation(variableDeclarationStatementVariables, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundDirectCastOperation(ByVal boundDirectCast As Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast) As IOperation
			Return Me.CreateBoundConversionOrCastOperation(boundDirectCast, False)
		End Function

		Private Function CreateBoundDoLoopStatementOperation(ByVal boundDoLoopStatement As Microsoft.CodeAnalysis.VisualBasic.BoundDoLoopStatement) As IWhileLoopOperation
			Dim operation As IOperation
			Dim operation1 As IOperation = Me.Create(boundDoLoopStatement.ConditionOpt)
			Dim operation2 As IOperation = Me.Create(boundDoLoopStatement.Body)
			If (boundDoLoopStatement.TopConditionOpt Is Nothing OrElse boundDoLoopStatement.BottomConditionOpt Is Nothing) Then
				operation = Nothing
			Else
				operation = Me.Create(boundDoLoopStatement.BottomConditionOpt)
			End If
			Dim operation3 As IOperation = operation
			Dim empty As ImmutableArray(Of ILocalSymbol) = ImmutableArray(Of ILocalSymbol).Empty
			Dim continueLabel As ILabelSymbol = boundDoLoopStatement.ContinueLabel
			Dim exitLabel As ILabelSymbol = boundDoLoopStatement.ExitLabel
			Dim conditionIsTop As Boolean = boundDoLoopStatement.ConditionIsTop
			Dim conditionIsUntil As Boolean = boundDoLoopStatement.ConditionIsUntil
			Dim syntax As SyntaxNode = boundDoLoopStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundDoLoopStatement.WasCompilerGenerated
			Return New WhileLoopOperation(operation1, conditionIsTop, conditionIsUntil, operation3, operation2, empty, continueLabel, exitLabel, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundEndStatementOperation(ByVal boundEndStatement As Microsoft.CodeAnalysis.VisualBasic.BoundEndStatement) As IEndOperation
			Dim syntax As SyntaxNode = boundEndStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundEndStatement.WasCompilerGenerated
			Return New EndOperation(Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundEventAccessOperation(ByVal boundEventAccess As Microsoft.CodeAnalysis.VisualBasic.BoundEventAccess) As IEventReferenceOperation
			Dim eventSymbol As IEventSymbol = boundEventAccess.EventSymbol
			Dim operation As IOperation = Me.CreateReceiverOperation(boundEventAccess.ReceiverOpt, eventSymbol)
			Dim syntax As SyntaxNode = boundEventAccess.Syntax
			Dim type As ITypeSymbol = boundEventAccess.Type
			Dim wasCompilerGenerated As Boolean = boundEventAccess.WasCompilerGenerated
			Return New EventReferenceOperation(eventSymbol, operation, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundExitStatementOperation(ByVal boundExitStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExitStatement) As IBranchOperation
			Dim label As LabelSymbol = boundExitStatement.Label
			Dim branchKind As Microsoft.CodeAnalysis.Operations.BranchKind = Microsoft.CodeAnalysis.Operations.BranchKind.Break
			Dim syntax As SyntaxNode = boundExitStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundExitStatement.WasCompilerGenerated
			Return New BranchOperation(label, branchKind, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundExpressionStatementOperation(ByVal boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement) As IExpressionStatementOperation
			Dim operation As IOperation = Me.Create(boundExpressionStatement.Expression)
			Dim syntax As SyntaxNode = boundExpressionStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundExpressionStatement.WasCompilerGenerated
			Return New ExpressionStatementOperation(operation, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundFieldAccessOperation(ByVal boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess) As IFieldReferenceOperation
			Dim fieldSymbol As IFieldSymbol = boundFieldAccess.FieldSymbol
			Dim flag As Boolean = False
			Dim operation As IOperation = Me.CreateReceiverOperation(boundFieldAccess.ReceiverOpt, fieldSymbol)
			Dim syntax As SyntaxNode = boundFieldAccess.Syntax
			Dim type As ITypeSymbol = boundFieldAccess.Type
			Dim constantValueOpt As ConstantValue = boundFieldAccess.ConstantValueOpt
			Dim wasCompilerGenerated As Boolean = boundFieldAccess.WasCompilerGenerated
			Return New FieldReferenceOperation(fieldSymbol, flag, operation, Me._semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated)
		End Function

		Private Function CreateBoundFieldInitializerOperation(ByVal boundFieldInitializer As Microsoft.CodeAnalysis.VisualBasic.BoundFieldInitializer) As IFieldInitializerOperation
			Dim fieldSymbols As ImmutableArray(Of IFieldSymbol) = boundFieldInitializer.InitializedFields.[As](Of IFieldSymbol)()
			Dim operation As IOperation = Me.Create(boundFieldInitializer.InitialValue)
			Dim syntax As SyntaxNode = boundFieldInitializer.Syntax
			Dim wasCompilerGenerated As Boolean = boundFieldInitializer.WasCompilerGenerated
			Return New FieldInitializerOperation(fieldSymbols, ImmutableArray(Of ILocalSymbol).Empty, operation, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundForEachStatementOperation(ByVal boundForEachStatement As Microsoft.CodeAnalysis.VisualBasic.BoundForEachStatement) As IForEachLoopOperation
			Dim empty As ImmutableArray(Of IOperation)
			Dim localSymbols As ImmutableArray(Of ILocalSymbol)
			Dim forEachLoopOperationInfo As Microsoft.CodeAnalysis.Operations.ForEachLoopOperationInfo = Me.GetForEachLoopOperationInfo(boundForEachStatement)
			Dim operation As IOperation = Me.CreateBoundControlVariableOperation(boundForEachStatement)
			Dim operation1 As IOperation = Me.Create(boundForEachStatement.Collection)
			If (boundForEachStatement.NextVariablesOpt.IsDefault) Then
				empty = ImmutableArray(Of IOperation).Empty
			Else
				empty = Me.CreateFromArray(Of BoundExpression, IOperation)(boundForEachStatement.NextVariablesOpt)
			End If
			Dim operations As ImmutableArray(Of IOperation) = empty
			Dim operation2 As IOperation = Me.Create(boundForEachStatement.Body)
			If (boundForEachStatement.DeclaredOrInferredLocalOpt IsNot Nothing) Then
				localSymbols = ImmutableArray.Create(Of ILocalSymbol)(boundForEachStatement.DeclaredOrInferredLocalOpt)
			Else
				localSymbols = ImmutableArray(Of ILocalSymbol).Empty
			End If
			Dim localSymbols1 As ImmutableArray(Of ILocalSymbol) = localSymbols
			Dim continueLabel As ILabelSymbol = boundForEachStatement.ContinueLabel
			Dim exitLabel As ILabelSymbol = boundForEachStatement.ExitLabel
			Dim syntax As SyntaxNode = boundForEachStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundForEachStatement.WasCompilerGenerated
			Return New ForEachLoopOperation(operation, operation1, operations, forEachLoopOperationInfo, False, operation2, localSymbols1, continueLabel, exitLabel, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundForToStatementOperation(ByVal boundForToStatement As Microsoft.CodeAnalysis.VisualBasic.BoundForToStatement) As IForToLoopOperation
			Dim empty As ImmutableArray(Of IOperation)
			Dim localSymbols As ImmutableArray(Of ILocalSymbol)
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal
			Dim operation As IOperation = Me.CreateBoundControlVariableOperation(boundForToStatement)
			Dim operation1 As IOperation = Me.Create(boundForToStatement.InitialValue)
			Dim operation2 As IOperation = Me.Create(boundForToStatement.LimitValue)
			Dim operation3 As IOperation = Me.Create(boundForToStatement.StepValue)
			Dim operation4 As IOperation = Me.Create(boundForToStatement.Body)
			If (boundForToStatement.NextVariablesOpt.IsDefault) Then
				empty = ImmutableArray(Of IOperation).Empty
			Else
				empty = Me.CreateFromArray(Of BoundExpression, IOperation)(boundForToStatement.NextVariablesOpt)
			End If
			Dim operations As ImmutableArray(Of IOperation) = empty
			If (boundForToStatement.DeclaredOrInferredLocalOpt IsNot Nothing) Then
				localSymbols = ImmutableArray.Create(Of ILocalSymbol)(boundForToStatement.DeclaredOrInferredLocalOpt)
			Else
				localSymbols = ImmutableArray(Of ILocalSymbol).Empty
			End If
			Dim localSymbols1 As ImmutableArray(Of ILocalSymbol) = localSymbols
			Dim continueLabel As ILabelSymbol = boundForToStatement.ContinueLabel
			Dim exitLabel As ILabelSymbol = boundForToStatement.ExitLabel
			Dim syntax As SyntaxNode = boundForToStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundForToStatement.WasCompilerGenerated
			If (boundForToStatement.ControlVariable.Type.IsObjectType()) Then
				Dim semanticModel As Microsoft.CodeAnalysis.SemanticModel = Me._semanticModel
				Dim spanStart As Integer = boundForToStatement.Syntax.SpanStart
				Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
				synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(DirectCast(semanticModel.GetEnclosingSymbol(spanStart, cancellationToken), Symbol), boundForToStatement.ControlVariable.Type, SynthesizedLocalKind.ForInitialValue, boundForToStatement.Syntax, False)
			Else
				synthesizedLocal = Nothing
			End If
			Dim synthesizedLocal1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = synthesizedLocal
			Dim forToLoopOperationUserDefinedInfo As Microsoft.CodeAnalysis.Operations.ForToLoopOperationUserDefinedInfo = Nothing
			Dim operatorsOpt As BoundForToUserDefinedOperators = boundForToStatement.OperatorsOpt
			If (operatorsOpt IsNot Nothing) Then
				Me.RecordParent(operatorsOpt.LeftOperandPlaceholder, boundForToStatement)
				Me.RecordParent(operatorsOpt.RightOperandPlaceholder, boundForToStatement)
				forToLoopOperationUserDefinedInfo = New Microsoft.CodeAnalysis.Operations.ForToLoopOperationUserDefinedInfo(DirectCast(Microsoft.CodeAnalysis.Operation.SetParentOperation(Of IOperation)(Me.Create(operatorsOpt.Addition), Nothing), IBinaryOperation), DirectCast(Microsoft.CodeAnalysis.Operation.SetParentOperation(Of IOperation)(Me.Create(operatorsOpt.Subtraction), Nothing), IBinaryOperation), Microsoft.CodeAnalysis.Operation.SetParentOperation(Of IOperation)(Me.Create(operatorsOpt.LessThanOrEqual), Nothing), Microsoft.CodeAnalysis.Operation.SetParentOperation(Of IOperation)(Me.Create(operatorsOpt.GreaterThanOrEqual), Nothing))
			End If
			Return New ForToLoopOperation(operation, operation1, operation2, operation3, boundForToStatement.Checked, operations, New ValueTuple(Of ILocalSymbol, Microsoft.CodeAnalysis.Operations.ForToLoopOperationUserDefinedInfo)(synthesizedLocal1, forToLoopOperationUserDefinedInfo), operation4, localSymbols1, continueLabel, exitLabel, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundGetTypeOperation(ByVal boundGetType As Microsoft.CodeAnalysis.VisualBasic.BoundGetType) As ITypeOfOperation
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = boundGetType.SourceType.Type
			Dim syntax As SyntaxNode = boundGetType.Syntax
			Dim typeSymbol As ITypeSymbol = boundGetType.Type
			Dim wasCompilerGenerated As Boolean = boundGetType.WasCompilerGenerated
			Return New TypeOfOperation(type, Me._semanticModel, syntax, typeSymbol, wasCompilerGenerated)
		End Function

		Private Function CreateBoundGotoStatementOperation(ByVal boundGotoStatement As Microsoft.CodeAnalysis.VisualBasic.BoundGotoStatement) As IBranchOperation
			Dim label As LabelSymbol = boundGotoStatement.Label
			Dim branchKind As Microsoft.CodeAnalysis.Operations.BranchKind = Microsoft.CodeAnalysis.Operations.BranchKind.[GoTo]
			Dim syntax As SyntaxNode = boundGotoStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundGotoStatement.WasCompilerGenerated
			Return New BranchOperation(label, branchKind, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundIfStatementOperation(ByVal boundIfStatement As Microsoft.CodeAnalysis.VisualBasic.BoundIfStatement) As IConditionalOperation
			Dim operation As IOperation = Me.Create(boundIfStatement.Condition)
			Dim operation1 As IOperation = Me.Create(boundIfStatement.Consequence)
			Dim operation2 As IOperation = Me.Create(boundIfStatement.AlternativeOpt)
			Dim syntax As SyntaxNode = boundIfStatement.Syntax
			Dim typeSymbol As ITypeSymbol = Nothing
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = Nothing
			Dim wasCompilerGenerated As Boolean = boundIfStatement.WasCompilerGenerated
			Dim flag As Boolean = False
			Return New ConditionalOperation(operation, operation1, operation2, flag, Me._semanticModel, syntax, typeSymbol, constantValue, wasCompilerGenerated)
		End Function

		Friend Function CreateBoundInterpolatedStringContentOperation(ByVal parts As ImmutableArray(Of BoundNode)) As ImmutableArray(Of IInterpolatedStringContentOperation)
			Dim instance As ArrayBuilder(Of IInterpolatedStringContentOperation) = ArrayBuilder(Of IInterpolatedStringContentOperation).GetInstance(parts.Length)
			Dim enumerator As ImmutableArray(Of BoundNode).Enumerator = parts.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As BoundNode = enumerator.Current
				If (current.Kind <> BoundKind.Interpolation) Then
					instance.Add(Me.CreateBoundInterpolatedStringTextOperation(DirectCast(current, BoundLiteral)))
				Else
					instance.Add(DirectCast(Me.Create(current), IInterpolatedStringContentOperation))
				End If
			End While
			Return instance.ToImmutableAndFree()
		End Function

		Private Function CreateBoundInterpolatedStringExpressionOperation(ByVal boundInterpolatedString As BoundInterpolatedStringExpression) As IInterpolatedStringOperation
			Dim interpolatedStringContentOperations As ImmutableArray(Of IInterpolatedStringContentOperation) = Me.CreateBoundInterpolatedStringContentOperation(boundInterpolatedString.Contents)
			Dim syntax As SyntaxNode = boundInterpolatedString.Syntax
			Dim type As ITypeSymbol = boundInterpolatedString.Type
			Dim constantValueOpt As ConstantValue = boundInterpolatedString.ConstantValueOpt
			Dim wasCompilerGenerated As Boolean = boundInterpolatedString.WasCompilerGenerated
			Return New InterpolatedStringOperation(interpolatedStringContentOperations, Me._semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated)
		End Function

		Private Function CreateBoundInterpolatedStringTextOperation(ByVal boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral) As IInterpolatedStringTextOperation
			Dim literalOperation As ILiteralOperation = Me.CreateBoundLiteralOperation(boundLiteral, True)
			Dim syntax As SyntaxNode = boundLiteral.Syntax
			Dim wasCompilerGenerated As Boolean = boundLiteral.WasCompilerGenerated
			Return New InterpolatedStringTextOperation(literalOperation, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundInterpolationOperation(ByVal boundInterpolation As Microsoft.CodeAnalysis.VisualBasic.BoundInterpolation) As IInterpolationOperation
			Dim operation As IOperation = Me.Create(boundInterpolation.Expression)
			Dim operation1 As IOperation = Me.Create(boundInterpolation.AlignmentOpt)
			Dim operation2 As IOperation = Me.Create(boundInterpolation.FormatStringOpt)
			Dim syntax As SyntaxNode = boundInterpolation.Syntax
			Dim wasCompilerGenerated As Boolean = boundInterpolation.WasCompilerGenerated
			Return New InterpolationOperation(operation, operation1, operation2, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundLabelStatementOperation(ByVal boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement) As ILabeledOperation
			Dim flag As Boolean
			Dim label As LabelSymbol = boundLabelStatement.Label
			Dim operation As IOperation = Nothing
			Dim syntax As SyntaxNode = boundLabelStatement.Syntax
			flag = If(boundLabelStatement.WasCompilerGenerated, True, VisualBasicOperationFactory.IsEndSubOrFunctionStatement(syntax))
			Return New LabeledOperation(label, operation, Me._semanticModel, syntax, flag)
		End Function

		Private Function CreateBoundLambdaOperation(ByVal boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda) As IAnonymousFunctionOperation
			Dim lambdaSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol = boundLambda.LambdaSymbol
			Dim blockOperation As IBlockOperation = DirectCast(Me.Create(boundLambda.Body), IBlockOperation)
			Dim syntax As SyntaxNode = boundLambda.Syntax
			Dim wasCompilerGenerated As Boolean = boundLambda.WasCompilerGenerated
			Return New AnonymousFunctionOperation(lambdaSymbol, blockOperation, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundLateInvocationOperation(ByVal boundLateInvocation As Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation) As IOperation
			Dim operation As IOperation = Me.Create(boundLateInvocation.Member)
			Dim operations As ImmutableArray(Of IOperation) = Me.CreateFromArray(Of BoundExpression, IOperation)(boundLateInvocation.ArgumentsOpt)
			Dim argumentNamesOpt As ImmutableArray(Of String) = boundLateInvocation.ArgumentNamesOpt
			Dim refKinds As ImmutableArray(Of RefKind) = New ImmutableArray(Of RefKind)()
			Dim syntax As SyntaxNode = boundLateInvocation.Syntax
			Dim type As ITypeSymbol = boundLateInvocation.Type
			Dim wasCompilerGenerated As Boolean = boundLateInvocation.WasCompilerGenerated
			Return New DynamicInvocationOperation(operation, operations, argumentNamesOpt, refKinds, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundLateMemberAccessOperation(ByVal boundLateMemberAccess As Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess) As IDynamicMemberReferenceOperation
			Dim operation As IOperation = Me.Create(boundLateMemberAccess.ReceiverOpt)
			Dim nameOpt As String = boundLateMemberAccess.NameOpt
			Dim empty As ImmutableArray(Of ITypeSymbol) = ImmutableArray(Of ITypeSymbol).Empty
			If (boundLateMemberAccess.TypeArgumentsOpt IsNot Nothing) Then
				empty = ImmutableArray(Of ITypeSymbol).CastUp(Of TypeSymbol)(boundLateMemberAccess.TypeArgumentsOpt.Arguments)
			End If
			Dim containerTypeOpt As ITypeSymbol = Nothing
			If (boundLateMemberAccess.ContainerTypeOpt IsNot Nothing AndAlso (boundLateMemberAccess.ReceiverOpt Is Nothing OrElse Not TypeSymbol.Equals(boundLateMemberAccess.ContainerTypeOpt, boundLateMemberAccess.ReceiverOpt.Type, TypeCompareKind.ConsiderEverything))) Then
				containerTypeOpt = boundLateMemberAccess.ContainerTypeOpt
			End If
			Dim syntax As SyntaxNode = boundLateMemberAccess.Syntax
			Dim type As ITypeSymbol = boundLateMemberAccess.Type
			Dim wasCompilerGenerated As Boolean = boundLateMemberAccess.WasCompilerGenerated
			Return New DynamicMemberReferenceOperation(operation, nameOpt, empty, containerTypeOpt, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Friend Function CreateBoundLiteralOperation(ByVal boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral, Optional ByVal implicit As Boolean = False) As ILiteralOperation
			Dim syntax As SyntaxNode = boundLiteral.Syntax
			Dim type As ITypeSymbol = boundLiteral.Type
			Dim constantValueOpt As ConstantValue = boundLiteral.ConstantValueOpt
			Return New LiteralOperation(Me._semanticModel, syntax, type, constantValueOpt, If(boundLiteral.WasCompilerGenerated, True, implicit))
		End Function

		Private Function CreateBoundLocalDeclarationOperation(ByVal boundLocalDeclaration As Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration) As IVariableDeclarationGroupOperation
			Dim variableDeclarationStatementVariables As ImmutableArray(Of IVariableDeclarationOperation) = Me.GetVariableDeclarationStatementVariables(ImmutableArray.Create(Of BoundLocalDeclarationBase)(boundLocalDeclaration))
			Dim syntax As SyntaxNode = boundLocalDeclaration.Syntax
			Return New VariableDeclarationGroupOperation(variableDeclarationStatementVariables, Me._semanticModel, syntax, True)
		End Function

		Private Function CreateBoundLocalOperation(ByVal boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal) As IOperation
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = boundLocal.LocalSymbol
			Dim flag As Boolean = False
			Dim syntax As SyntaxNode = boundLocal.Syntax
			Dim type As ITypeSymbol = boundLocal.Type
			Dim constantValueOpt As ConstantValue = boundLocal.ConstantValueOpt
			Dim wasCompilerGenerated As Boolean = boundLocal.WasCompilerGenerated
			Return New LocalReferenceOperation(localSymbol, flag, Me._semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated)
		End Function

		Private Function CreateBoundLValueToRValueWrapper(ByVal boundNode As BoundLValueToRValueWrapper) As IOperation
			Return Me.Create(boundNode.UnderlyingLValue)
		End Function

		Private Function CreateBoundMeReferenceOperation(ByVal boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference) As IInstanceReferenceOperation
			Dim syntax As SyntaxNode = boundMeReference.Syntax
			Dim type As ITypeSymbol = boundMeReference.Type
			Dim wasCompilerGenerated As Boolean = boundMeReference.WasCompilerGenerated
			Return New InstanceReferenceOperation(InstanceReferenceKind.ContainingTypeInstance, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundMyBaseReferenceOperation(ByVal boundMyBaseReference As Microsoft.CodeAnalysis.VisualBasic.BoundMyBaseReference) As IInstanceReferenceOperation
			Dim syntax As SyntaxNode = boundMyBaseReference.Syntax
			Dim type As ITypeSymbol = boundMyBaseReference.Type
			Dim wasCompilerGenerated As Boolean = boundMyBaseReference.WasCompilerGenerated
			Return New InstanceReferenceOperation(InstanceReferenceKind.ContainingTypeInstance, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundMyClassReferenceOperation(ByVal boundMyClassReference As Microsoft.CodeAnalysis.VisualBasic.BoundMyClassReference) As IInstanceReferenceOperation
			Dim syntax As SyntaxNode = boundMyClassReference.Syntax
			Dim type As ITypeSymbol = boundMyClassReference.Type
			Dim wasCompilerGenerated As Boolean = boundMyClassReference.WasCompilerGenerated
			Return New InstanceReferenceOperation(InstanceReferenceKind.ContainingTypeInstance, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundNameOfOperatorOperation(ByVal boundNameOfOperator As Microsoft.CodeAnalysis.VisualBasic.BoundNameOfOperator) As INameOfOperation
			Dim operation As IOperation = Me.Create(boundNameOfOperator.Argument)
			Dim syntax As SyntaxNode = boundNameOfOperator.Syntax
			Dim type As ITypeSymbol = boundNameOfOperator.Type
			Dim constantValueOpt As ConstantValue = boundNameOfOperator.ConstantValueOpt
			Dim wasCompilerGenerated As Boolean = boundNameOfOperator.WasCompilerGenerated
			Return New NameOfOperation(operation, Me._semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated)
		End Function

		Private Function CreateBoundNewTOperation(ByVal boundNewT As Microsoft.CodeAnalysis.VisualBasic.BoundNewT) As ITypeParameterObjectCreationOperation
			Dim objectOrCollectionInitializerOperation As IObjectOrCollectionInitializerOperation = DirectCast(Me.Create(boundNewT.InitializerOpt), IObjectOrCollectionInitializerOperation)
			Dim syntax As SyntaxNode = boundNewT.Syntax
			Dim type As ITypeSymbol = boundNewT.Type
			Dim wasCompilerGenerated As Boolean = boundNewT.WasCompilerGenerated
			Return New TypeParameterObjectCreationOperation(objectOrCollectionInitializerOperation, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundNoOpStatementOperation(ByVal boundNoOpStatement As Microsoft.CodeAnalysis.VisualBasic.BoundNoOpStatement) As IEmptyOperation
			Dim syntax As SyntaxNode = boundNoOpStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundNoOpStatement.WasCompilerGenerated
			Return New EmptyOperation(Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundNullableIsTrueOperator(ByVal boundNullableIsTrueOperator As Microsoft.CodeAnalysis.VisualBasic.BoundNullableIsTrueOperator) As IOperation
			Dim invalidOperation As IOperation
			Dim syntax As SyntaxNode = boundNullableIsTrueOperator.Syntax
			Dim type As ITypeSymbol = boundNullableIsTrueOperator.Type
			Dim constantValueOpt As ConstantValue = boundNullableIsTrueOperator.ConstantValueOpt
			Dim wasCompilerGenerated As Boolean = boundNullableIsTrueOperator.WasCompilerGenerated
			Dim specialTypeMember As MethodSymbol = DirectCast(DirectCast(Me._semanticModel.Compilation, VisualBasicCompilation).GetSpecialTypeMember(SpecialMember.System_Nullable_T_GetValueOrDefault), MethodSymbol)
			If (specialTypeMember Is Nothing) Then
				invalidOperation = New Microsoft.CodeAnalysis.Operations.InvalidOperation(Me.CreateFromArray(Of BoundNode, IOperation)(DirectCast(boundNullableIsTrueOperator, IBoundInvalidNode).InvalidNodeChildren), Me._semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated)
			Else
				Dim operation As IOperation = Me.CreateReceiverOperation(boundNullableIsTrueOperator.Operand, specialTypeMember)
				invalidOperation = New InvocationOperation(specialTypeMember.AsMember(DirectCast(boundNullableIsTrueOperator.Operand.Type, NamedTypeSymbol)), operation, False, ImmutableArray(Of IArgumentOperation).Empty, Me._semanticModel, syntax, boundNullableIsTrueOperator.Type, wasCompilerGenerated)
			End If
			Return invalidOperation
		End Function

		Private Function CreateBoundObjectCreationExpressionOperation(ByVal boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression) As IObjectCreationOperation
			Dim constructorOpt As MethodSymbol = boundObjectCreationExpression.ConstructorOpt
			Dim objectOrCollectionInitializerOperation As IObjectOrCollectionInitializerOperation = DirectCast(Me.Create(boundObjectCreationExpression.InitializerOpt), IObjectOrCollectionInitializerOperation)
			Dim argumentOperations As ImmutableArray(Of IArgumentOperation) = Me.DeriveArguments(boundObjectCreationExpression)
			Dim syntax As SyntaxNode = boundObjectCreationExpression.Syntax
			Dim type As ITypeSymbol = boundObjectCreationExpression.Type
			Dim constantValueOpt As ConstantValue = boundObjectCreationExpression.ConstantValueOpt
			Dim wasCompilerGenerated As Boolean = boundObjectCreationExpression.WasCompilerGenerated
			Return New ObjectCreationOperation(constructorOpt, objectOrCollectionInitializerOperation, argumentOperations, Me._semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated)
		End Function

		Private Function CreateBoundObjectInitializerExpressionOperation(ByVal boundObjectInitializerExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpression) As IObjectOrCollectionInitializerOperation
			Dim operations As ImmutableArray(Of IOperation) = Me.CreateFromArray(Of BoundExpression, IOperation)(boundObjectInitializerExpression.Initializers)
			Dim syntax As SyntaxNode = boundObjectInitializerExpression.Syntax
			Dim type As ITypeSymbol = boundObjectInitializerExpression.Type
			Dim wasCompilerGenerated As Boolean = boundObjectInitializerExpression.WasCompilerGenerated
			Return New ObjectOrCollectionInitializerOperation(operations, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundOmittedArgumentOperation(ByVal boundOmittedArgument As Microsoft.CodeAnalysis.VisualBasic.BoundOmittedArgument) As IOmittedArgumentOperation
			Dim syntax As SyntaxNode = boundOmittedArgument.Syntax
			Dim type As ITypeSymbol = boundOmittedArgument.Type
			Dim wasCompilerGenerated As Boolean = boundOmittedArgument.WasCompilerGenerated
			Return New OmittedArgumentOperation(Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundParameterEqualsValueOperation(ByVal boundParameterEqualsValue As Microsoft.CodeAnalysis.VisualBasic.BoundParameterEqualsValue) As IParameterInitializerOperation
			Dim parameter As ParameterSymbol = boundParameterEqualsValue.Parameter
			Dim operation As IOperation = Me.Create(boundParameterEqualsValue.Value)
			Dim syntax As SyntaxNode = boundParameterEqualsValue.Syntax
			Dim wasCompilerGenerated As Boolean = boundParameterEqualsValue.WasCompilerGenerated
			Return New ParameterInitializerOperation(parameter, ImmutableArray(Of ILocalSymbol).Empty, operation, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundParameterOperation(ByVal boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter) As IParameterReferenceOperation
			Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = boundParameter.ParameterSymbol
			Dim syntax As SyntaxNode = boundParameter.Syntax
			Dim type As ITypeSymbol = boundParameter.Type
			Dim wasCompilerGenerated As Boolean = boundParameter.WasCompilerGenerated
			Return New ParameterReferenceOperation(parameterSymbol, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundParenthesizedOperation(ByVal boundParenthesized As Microsoft.CodeAnalysis.VisualBasic.BoundParenthesized) As IParenthesizedOperation
			Dim operation As IOperation = Me.Create(boundParenthesized.Expression)
			Dim syntax As SyntaxNode = boundParenthesized.Syntax
			Dim type As ITypeSymbol = boundParenthesized.Type
			Dim constantValueOpt As ConstantValue = boundParenthesized.ConstantValueOpt
			Dim wasCompilerGenerated As Boolean = boundParenthesized.WasCompilerGenerated
			Return New ParenthesizedOperation(operation, Me._semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated)
		End Function

		Private Function CreateBoundPropertyAccessOperation(ByVal boundPropertyAccess As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess) As IPropertyReferenceOperation
			Dim propertySymbol As IPropertySymbol = boundPropertyAccess.PropertySymbol
			Dim receiverOpt As Object = boundPropertyAccess.ReceiverOpt
			If (receiverOpt Is Nothing) Then
				Dim propertyGroupOpt As BoundPropertyGroup = boundPropertyAccess.PropertyGroupOpt
				If (propertyGroupOpt IsNot Nothing) Then
					receiverOpt = propertyGroupOpt.ReceiverOpt
				Else
					receiverOpt = Nothing
				End If
			End If
			Dim operation As IOperation = Me.CreateReceiverOperation(receiverOpt, propertySymbol)
			Dim argumentOperations As ImmutableArray(Of IArgumentOperation) = Me.DeriveArguments(boundPropertyAccess)
			Dim syntax As SyntaxNode = boundPropertyAccess.Syntax
			Dim type As ITypeSymbol = boundPropertyAccess.Type
			Dim wasCompilerGenerated As Boolean = boundPropertyAccess.WasCompilerGenerated
			Return New PropertyReferenceOperation(propertySymbol, argumentOperations, operation, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundPropertyInitializerOperation(ByVal boundPropertyInitializer As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyInitializer) As IPropertyInitializerOperation
			Dim propertySymbols As ImmutableArray(Of IPropertySymbol) = boundPropertyInitializer.InitializedProperties.[As](Of IPropertySymbol)()
			Dim operation As IOperation = Me.Create(boundPropertyInitializer.InitialValue)
			Dim syntax As SyntaxNode = boundPropertyInitializer.Syntax
			Dim wasCompilerGenerated As Boolean = boundPropertyInitializer.WasCompilerGenerated
			Return New PropertyInitializerOperation(propertySymbols, ImmutableArray(Of ILocalSymbol).Empty, operation, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundQueryExpressionOperation(ByVal boundQueryExpression As Microsoft.CodeAnalysis.VisualBasic.BoundQueryExpression) As IOperation
			Dim operation As IOperation = Me.Create(boundQueryExpression.LastOperator)
			Dim syntax As SyntaxNode = boundQueryExpression.Syntax
			Dim type As ITypeSymbol = boundQueryExpression.Type
			Dim wasCompilerGenerated As Boolean = boundQueryExpression.WasCompilerGenerated
			Return New TranslatedQueryOperation(operation, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Friend Function CreateBoundRaiseEventStatementEventReference(ByVal boundRaiseEventStatement As Microsoft.CodeAnalysis.VisualBasic.BoundRaiseEventStatement) As IEventReferenceOperation
			Dim syntax As Object
			Dim receiverOpt As BoundExpression = DirectCast(boundRaiseEventStatement.EventInvocation, BoundCall).ReceiverOpt
			If (receiverOpt IsNot Nothing) Then
				syntax = receiverOpt.Syntax
			Else
				syntax = Nothing
			End If
			If (syntax Is Nothing) Then
				Dim raiseEventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax = TryCast(boundRaiseEventStatement.Syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax)
				If (raiseEventStatementSyntax IsNot Nothing) Then
					syntax = raiseEventStatementSyntax.Name
				Else
					syntax = Nothing
				End If
				If (syntax Is Nothing) Then
					syntax = boundRaiseEventStatement.Syntax
				End If
			End If
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = syntax
			Dim type As ITypeSymbol = boundRaiseEventStatement.EventSymbol.Type
			Dim flag As Boolean = False
			If (If(receiverOpt IsNot Nothing, receiverOpt.Kind = BoundKind.FieldAccess, False)) Then
				receiverOpt = DirectCast(receiverOpt, BoundFieldAccess).ReceiverOpt
			End If
			Dim operation As IOperation = Me.CreateReceiverOperation(receiverOpt, boundRaiseEventStatement.EventSymbol)
			Return New EventReferenceOperation(boundRaiseEventStatement.EventSymbol, operation, Me._semanticModel, syntaxNode, type, flag)
		End Function

		Private Function CreateBoundRaiseEventStatementOperation(ByVal boundRaiseEventStatement As Microsoft.CodeAnalysis.VisualBasic.BoundRaiseEventStatement) As IOperation
			Dim invalidOperation As IOperation
			Dim syntax As SyntaxNode = boundRaiseEventStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundRaiseEventStatement.WasCompilerGenerated
			Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = boundRaiseEventStatement.EventSymbol
			Dim eventInvocation As BoundCall = TryCast(boundRaiseEventStatement.EventInvocation, BoundCall)
			If (eventInvocation Is Nothing OrElse eventInvocation.ReceiverOpt Is Nothing AndAlso Not eventSymbol.IsShared) Then
				invalidOperation = New Microsoft.CodeAnalysis.Operations.InvalidOperation(Me.CreateFromArray(Of BoundNode, IOperation)(DirectCast(boundRaiseEventStatement, IBoundInvalidNode).InvalidNodeChildren), Me._semanticModel, syntax, Nothing, Nothing, wasCompilerGenerated)
			Else
				Dim eventReferenceOperation As IEventReferenceOperation = Me.CreateBoundRaiseEventStatementEventReference(boundRaiseEventStatement)
				Dim argumentOperations As ImmutableArray(Of IArgumentOperation) = Me.DeriveArguments(boundRaiseEventStatement)
				invalidOperation = New RaiseEventOperation(eventReferenceOperation, argumentOperations, Me._semanticModel, syntax, wasCompilerGenerated)
			End If
			Return invalidOperation
		End Function

		Private Function CreateBoundRangeCaseClauseOperation(ByVal boundRangeCaseClause As Microsoft.CodeAnalysis.VisualBasic.BoundRangeCaseClause) As IRangeCaseClauseOperation
			Dim operation As IOperation = Me.Create(VisualBasicOperationFactory.GetCaseClauseValue(boundRangeCaseClause.LowerBoundOpt, boundRangeCaseClause.LowerBoundConditionOpt))
			Dim operation1 As IOperation = Me.Create(VisualBasicOperationFactory.GetCaseClauseValue(boundRangeCaseClause.UpperBoundOpt, boundRangeCaseClause.UpperBoundConditionOpt))
			Dim syntax As SyntaxNode = boundRangeCaseClause.Syntax
			Dim wasCompilerGenerated As Boolean = boundRangeCaseClause.WasCompilerGenerated
			Return New RangeCaseClauseOperation(operation, operation1, Nothing, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundReDimClauseOperation(ByVal boundRedimClause As Microsoft.CodeAnalysis.VisualBasic.BoundRedimClause) As IReDimClauseOperation
			Dim operation As IOperation = Me.Create(boundRedimClause.Operand)
			Dim operations As ImmutableArray(Of IOperation) = Me.CreateFromArray(Of BoundExpression, IOperation)(boundRedimClause.Indices)
			Dim syntax As SyntaxNode = boundRedimClause.Syntax
			Dim wasCompilerGenerated As Boolean = boundRedimClause.WasCompilerGenerated
			Return New ReDimClauseOperation(operation, operations, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundReDimOperation(ByVal boundRedimStatement As Microsoft.CodeAnalysis.VisualBasic.BoundRedimStatement) As IReDimOperation
			Dim reDimClauseOperations As ImmutableArray(Of IReDimClauseOperation) = Me.CreateFromArray(Of BoundRedimClause, IReDimClauseOperation)(boundRedimStatement.Clauses)
			Dim flag As Boolean = boundRedimStatement.Syntax.Kind() = SyntaxKind.ReDimPreserveStatement
			Dim syntax As SyntaxNode = boundRedimStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundRedimStatement.WasCompilerGenerated
			Return New ReDimOperation(reDimClauseOperations, flag, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundRelationalCaseClauseOperation(ByVal boundRelationalCaseClause As Microsoft.CodeAnalysis.VisualBasic.BoundRelationalCaseClause) As IRelationalCaseClauseOperation
			Dim operation As IOperation = Me.Create(VisualBasicOperationFactory.GetSingleValueCaseClauseValue(boundRelationalCaseClause))
			Dim binaryOperatorKind As Microsoft.CodeAnalysis.Operations.BinaryOperatorKind = If(operation IsNot Nothing, VisualBasicOperationFactory.Helper.DeriveBinaryOperatorKind(boundRelationalCaseClause.OperatorKind, Nothing), Microsoft.CodeAnalysis.Operations.BinaryOperatorKind.None)
			Dim syntax As SyntaxNode = boundRelationalCaseClause.Syntax
			Dim wasCompilerGenerated As Boolean = boundRelationalCaseClause.WasCompilerGenerated
			Return New RelationalCaseClauseOperation(operation, binaryOperatorKind, Nothing, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundRemoveHandlerStatementOperation(ByVal boundRemoveHandlerStatement As Microsoft.CodeAnalysis.VisualBasic.BoundRemoveHandlerStatement) As IExpressionStatementOperation
			Dim addRemoveHandlerStatementExpression As IOperation = Me.GetAddRemoveHandlerStatementExpression(boundRemoveHandlerStatement)
			Dim syntax As SyntaxNode = boundRemoveHandlerStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundRemoveHandlerStatement.WasCompilerGenerated
			Return New ExpressionStatementOperation(addRemoveHandlerStatementExpression, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundReturnStatementOperation(ByVal boundReturnStatement As Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement) As IReturnOperation
			Dim flag As Boolean
			Dim operation As IOperation = Me.Create(boundReturnStatement.ExpressionOpt)
			Dim syntax As SyntaxNode = boundReturnStatement.Syntax
			flag = If(boundReturnStatement.WasCompilerGenerated, True, VisualBasicOperationFactory.IsEndSubOrFunctionStatement(syntax))
			Return New ReturnOperation(operation, OperationKind.[Return], Me._semanticModel, syntax, flag)
		End Function

		Private Function CreateBoundRValuePlaceholderOperation(ByVal boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder) As IOperation
			Dim placeholderOperation As IOperation
			Dim expression As Object
			Dim syntax As SyntaxNode = boundRValuePlaceholder.Syntax
			Dim type As ITypeSymbol = boundRValuePlaceholder.Type
			Dim wasCompilerGenerated As Boolean = boundRValuePlaceholder.WasCompilerGenerated
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Me.TryGetParent(boundRValuePlaceholder)
			Dim placeholderKind As Microsoft.CodeAnalysis.Operations.PlaceholderKind = Microsoft.CodeAnalysis.Operations.PlaceholderKind.Unspecified
			If (boundNode IsNot Nothing) Then
				Dim kind As BoundKind = boundNode.Kind
				If (kind > BoundKind.ForToStatement) Then
					If (kind = BoundKind.AggregateClause) Then
						placeholderKind = Microsoft.CodeAnalysis.Operations.PlaceholderKind.AggregationGroup
					Else
						If (kind <> BoundKind.ConditionalAccess) Then
							placeholderOperation = New Microsoft.CodeAnalysis.Operations.PlaceholderOperation(placeholderKind, Me._semanticModel, syntax, type, wasCompilerGenerated)
							Return placeholderOperation
						End If
						Dim conditionalAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax = TryCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax)
						If (conditionalAccessExpressionSyntax IsNot Nothing) Then
							expression = conditionalAccessExpressionSyntax.Expression
						Else
							expression = Nothing
						End If
						If (expression Is Nothing) Then
							expression = syntax
						End If
						syntax = expression
						placeholderOperation = New ConditionalAccessInstanceOperation(Me._semanticModel, syntax, type, wasCompilerGenerated)
						Return placeholderOperation
					End If
				ElseIf (kind = BoundKind.SelectStatement) Then
					placeholderKind = Microsoft.CodeAnalysis.Operations.PlaceholderKind.SwitchOperationExpression
				ElseIf (kind = BoundKind.ForToStatement) Then
					placeholderKind = If(boundRValuePlaceholder <> DirectCast(boundNode, BoundForToStatement).OperatorsOpt.LeftOperandPlaceholder, Microsoft.CodeAnalysis.Operations.PlaceholderKind.ForToLoopBinaryOperatorRightOperand, Microsoft.CodeAnalysis.Operations.PlaceholderKind.ForToLoopBinaryOperatorLeftOperand)
				End If
			End If
			placeholderOperation = New Microsoft.CodeAnalysis.Operations.PlaceholderOperation(placeholderKind, Me._semanticModel, syntax, type, wasCompilerGenerated)
			Return placeholderOperation
		End Function

		Private Function CreateBoundSelectStatementOperation(ByVal boundSelectStatement As Microsoft.CodeAnalysis.VisualBasic.BoundSelectStatement) As ISwitchOperation
			Me.RecordParent(boundSelectStatement.ExprPlaceholderOpt, boundSelectStatement)
			Dim operation As IOperation = Me.Create(boundSelectStatement.ExpressionStatement.Expression)
			Dim switchCaseOperations As ImmutableArray(Of ISwitchCaseOperation) = Me.CreateFromArray(Of BoundCaseBlock, ISwitchCaseOperation)(boundSelectStatement.CaseBlocks)
			Dim exitLabel As ILabelSymbol = boundSelectStatement.ExitLabel
			Dim syntax As SyntaxNode = boundSelectStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundSelectStatement.WasCompilerGenerated
			Return New SwitchOperation(ImmutableArray(Of ILocalSymbol).Empty, operation, switchCaseOperations, exitLabel, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundSimpleCaseClauseOperation(ByVal boundSimpleCaseClause As Microsoft.CodeAnalysis.VisualBasic.BoundSimpleCaseClause) As ISingleValueCaseClauseOperation
			Dim operation As IOperation = Me.Create(VisualBasicOperationFactory.GetSingleValueCaseClauseValue(boundSimpleCaseClause))
			Dim syntax As SyntaxNode = boundSimpleCaseClause.Syntax
			Dim wasCompilerGenerated As Boolean = boundSimpleCaseClause.WasCompilerGenerated
			Return New SingleValueCaseClauseOperation(operation, Nothing, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundStopStatementOperation(ByVal boundStopStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStopStatement) As IStopOperation
			Dim syntax As SyntaxNode = boundStopStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundStopStatement.WasCompilerGenerated
			Return New StopOperation(Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundSyncLockStatementOperation(ByVal boundSyncLockStatement As Microsoft.CodeAnalysis.VisualBasic.BoundSyncLockStatement) As ILockOperation
			Dim synthesizedLocal As ILocalSymbol
			If (Me._semanticModel.Compilation.CommonGetWellKnownTypeMember(WellKnownMember.System_Threading_Monitor__Enter2) Is Nothing) Then
				synthesizedLocal = Nothing
			Else
				Dim semanticModel As Microsoft.CodeAnalysis.SemanticModel = Me._semanticModel
				Dim spanStart As Integer = boundSyncLockStatement.Syntax.SpanStart
				Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
				synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(DirectCast(semanticModel.GetEnclosingSymbol(spanStart, cancellationToken), Symbol), DirectCast(Me._semanticModel.Compilation.GetSpecialType(SpecialType.System_Boolean), TypeSymbol), SynthesizedLocalKind.LockTaken, boundSyncLockStatement.LockExpression.Syntax, False)
			End If
			Dim localSymbol As ILocalSymbol = synthesizedLocal
			Dim operation As IOperation = Me.Create(boundSyncLockStatement.LockExpression)
			Dim operation1 As IOperation = Me.Create(boundSyncLockStatement.Body)
			Dim syntax As SyntaxNode = boundSyncLockStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundSyncLockStatement.WasCompilerGenerated
			Return New LockOperation(operation, operation1, localSymbol, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundTernaryConditionalExpressionOperation(ByVal boundTernaryConditionalExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTernaryConditionalExpression) As IConditionalOperation
			Dim operation As IOperation = Me.Create(boundTernaryConditionalExpression.Condition)
			Dim operation1 As IOperation = Me.Create(boundTernaryConditionalExpression.WhenTrue)
			Dim operation2 As IOperation = Me.Create(boundTernaryConditionalExpression.WhenFalse)
			Dim syntax As SyntaxNode = boundTernaryConditionalExpression.Syntax
			Dim type As ITypeSymbol = boundTernaryConditionalExpression.Type
			Dim constantValueOpt As ConstantValue = boundTernaryConditionalExpression.ConstantValueOpt
			Dim wasCompilerGenerated As Boolean = boundTernaryConditionalExpression.WasCompilerGenerated
			Dim flag As Boolean = False
			Return New ConditionalOperation(operation, operation1, operation2, flag, Me._semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated)
		End Function

		Private Function CreateBoundThrowStatementOperation(ByVal boundThrowStatement As Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement) As IThrowOperation
			Dim operation As IOperation = Me.Create(boundThrowStatement.ExpressionOpt)
			Dim syntax As SyntaxNode = boundThrowStatement.Syntax
			Dim typeSymbol As ITypeSymbol = Nothing
			Dim wasCompilerGenerated As Boolean = boundThrowStatement.WasCompilerGenerated
			Return New ThrowOperation(operation, Me._semanticModel, syntax, typeSymbol, wasCompilerGenerated)
		End Function

		Private Function CreateBoundTryCastOperation(ByVal boundTryCast As Microsoft.CodeAnalysis.VisualBasic.BoundTryCast) As IOperation
			Return Me.CreateBoundConversionOrCastOperation(boundTryCast, True)
		End Function

		Private Function CreateBoundTryStatementOperation(ByVal boundTryStatement As Microsoft.CodeAnalysis.VisualBasic.BoundTryStatement) As ITryOperation
			Dim blockOperation As IBlockOperation = DirectCast(Me.Create(boundTryStatement.TryBlock), IBlockOperation)
			Dim catchClauseOperations As ImmutableArray(Of ICatchClauseOperation) = Me.CreateFromArray(Of BoundCatchBlock, ICatchClauseOperation)(boundTryStatement.CatchBlocks)
			Dim blockOperation1 As IBlockOperation = DirectCast(Me.Create(boundTryStatement.FinallyBlockOpt), IBlockOperation)
			Dim exitLabelOpt As ILabelSymbol = boundTryStatement.ExitLabelOpt
			Dim syntax As SyntaxNode = boundTryStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundTryStatement.WasCompilerGenerated
			Return New TryOperation(blockOperation, catchClauseOperations, blockOperation1, exitLabelOpt, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundTupleLiteralOperation(ByVal boundTupleLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundTupleLiteral) As ITupleOperation
			Return Me.CreateTupleOperation(boundTupleLiteral, boundTupleLiteral.Type)
		End Function

		Private Function CreateBoundTypeArgumentsOperation(ByVal boundTypeArguments As Microsoft.CodeAnalysis.VisualBasic.BoundTypeArguments) As IInvalidOperation
			Dim syntax As SyntaxNode = boundTypeArguments.Syntax
			Dim typeSymbol As ITypeSymbol = Nothing
			Dim constantValueOpt As ConstantValue = boundTypeArguments.ConstantValueOpt
			Dim wasCompilerGenerated As Boolean = boundTypeArguments.WasCompilerGenerated
			Return New InvalidOperation(ImmutableArray(Of IOperation).Empty, Me._semanticModel, syntax, typeSymbol, constantValueOpt, wasCompilerGenerated)
		End Function

		Private Function CreateBoundTypeOfOperation(ByVal boundTypeOf As Microsoft.CodeAnalysis.VisualBasic.BoundTypeOf) As IIsTypeOperation
			Dim operation As IOperation = Me.Create(boundTypeOf.Operand)
			Dim targetType As ITypeSymbol = boundTypeOf.TargetType
			Dim isTypeOfIsNotExpression As Boolean = boundTypeOf.IsTypeOfIsNotExpression
			Dim syntax As SyntaxNode = boundTypeOf.Syntax
			Dim type As ITypeSymbol = boundTypeOf.Type
			Dim wasCompilerGenerated As Boolean = boundTypeOf.WasCompilerGenerated
			Return New IsTypeOperation(operation, targetType, isTypeOfIsNotExpression, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Friend Function CreateBoundUnaryOperatorChild(ByVal boundOperator As BoundExpression) As IOperation
			Dim operation As IOperation
			Dim kind As BoundKind = boundOperator.Kind
			If (kind = BoundKind.UnaryOperator) Then
				operation = Me.Create(DirectCast(boundOperator, BoundUnaryOperator).Operand)
			Else
				If (kind <> BoundKind.UserDefinedUnaryOperator) Then
					Throw ExceptionUtilities.UnexpectedValue(boundOperator.Kind)
				End If
				Dim boundUserDefinedUnaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator = DirectCast(boundOperator, Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator)
				operation = If(boundUserDefinedUnaryOperator.UnderlyingExpression.Kind <> BoundKind.[Call], Me.GetChildOfBadExpression(boundUserDefinedUnaryOperator.UnderlyingExpression, 0), Me.Create(boundUserDefinedUnaryOperator.Operand))
			End If
			Return operation
		End Function

		Private Function CreateBoundUnaryOperatorOperation(ByVal boundUnaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator) As IUnaryOperation
			Dim operation As IOperation = Me.CreateBoundUnaryOperatorChild(boundUnaryOperator)
			Dim unaryOperatorKind As Microsoft.CodeAnalysis.Operations.UnaryOperatorKind = VisualBasicOperationFactory.Helper.DeriveUnaryOperatorKind(boundUnaryOperator.OperatorKind)
			Dim methodSymbol As IMethodSymbol = Nothing
			Dim syntax As SyntaxNode = boundUnaryOperator.Syntax
			Dim type As ITypeSymbol = boundUnaryOperator.Type
			Dim constantValueOpt As ConstantValue = boundUnaryOperator.ConstantValueOpt
			Dim operatorKind As Boolean = CInt((boundUnaryOperator.OperatorKind And Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Lifted)) <> 0
			Dim checked As Boolean = boundUnaryOperator.Checked
			Dim wasCompilerGenerated As Boolean = boundUnaryOperator.WasCompilerGenerated
			Return New UnaryOperation(unaryOperatorKind, operation, operatorKind, checked, methodSymbol, Me._semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated)
		End Function

		Private Function CreateBoundUserDefinedBinaryOperatorOperation(ByVal boundUserDefinedBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator) As IBinaryOperation
			Dim operation As IOperation = Me.CreateBoundBinaryOperatorChild(boundUserDefinedBinaryOperator, True)
			Dim operation1 As IOperation = Me.CreateBoundBinaryOperatorChild(boundUserDefinedBinaryOperator, False)
			Dim userDefinedBinaryOperatorInfo As VisualBasicOperationFactory.BinaryOperatorInfo = VisualBasicOperationFactory.GetUserDefinedBinaryOperatorInfo(boundUserDefinedBinaryOperator)
			Dim syntax As SyntaxNode = boundUserDefinedBinaryOperator.Syntax
			Dim type As ITypeSymbol = boundUserDefinedBinaryOperator.Type
			Dim constantValueOpt As ConstantValue = boundUserDefinedBinaryOperator.ConstantValueOpt
			Dim wasCompilerGenerated As Boolean = boundUserDefinedBinaryOperator.WasCompilerGenerated
			Return New BinaryOperation(userDefinedBinaryOperatorInfo.OperatorKind, operation, operation1, userDefinedBinaryOperatorInfo.IsLifted, userDefinedBinaryOperatorInfo.IsChecked, userDefinedBinaryOperatorInfo.IsCompareText, userDefinedBinaryOperatorInfo.OperatorMethod, Nothing, Me._semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated)
		End Function

		Private Function CreateBoundUserDefinedShortCircuitingOperatorOperation(ByVal boundUserDefinedShortCircuitingOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedShortCircuitingOperator) As IBinaryOperation
			Dim binaryOperatorKind As Microsoft.CodeAnalysis.Operations.BinaryOperatorKind
			Dim operation As IOperation = Me.CreateBoundBinaryOperatorChild(boundUserDefinedShortCircuitingOperator, True)
			Dim operation1 As IOperation = Me.CreateBoundBinaryOperatorChild(boundUserDefinedShortCircuitingOperator, False)
			Dim userDefinedBinaryOperatorInfo As VisualBasicOperationFactory.BinaryOperatorInfo = VisualBasicOperationFactory.GetUserDefinedBinaryOperatorInfo(boundUserDefinedShortCircuitingOperator.BitwiseOperator)
			binaryOperatorKind = If(userDefinedBinaryOperatorInfo.OperatorKind = Microsoft.CodeAnalysis.Operations.BinaryOperatorKind.[And], Microsoft.CodeAnalysis.Operations.BinaryOperatorKind.ConditionalAnd, Microsoft.CodeAnalysis.Operations.BinaryOperatorKind.ConditionalOr)
			Dim syntax As SyntaxNode = boundUserDefinedShortCircuitingOperator.Syntax
			Dim type As ITypeSymbol = boundUserDefinedShortCircuitingOperator.Type
			Dim constantValueOpt As ConstantValue = boundUserDefinedShortCircuitingOperator.ConstantValueOpt
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim wasCompilerGenerated As Boolean = boundUserDefinedShortCircuitingOperator.WasCompilerGenerated
			Dim methodSymbol As IMethodSymbol = Nothing
			Dim leftTest As BoundExpression = boundUserDefinedShortCircuitingOperator.LeftTest
			If (leftTest IsNot Nothing) Then
				methodSymbol = VisualBasicOperationFactory.TryGetOperatorMethod(DirectCast(If(leftTest.Kind = BoundKind.UserDefinedUnaryOperator, leftTest, DirectCast(leftTest, BoundNullableIsTrueOperator).Operand), BoundUserDefinedUnaryOperator))
			End If
			Return New BinaryOperation(binaryOperatorKind, operation, operation1, userDefinedBinaryOperatorInfo.IsLifted, flag, flag1, userDefinedBinaryOperatorInfo.OperatorMethod, methodSymbol, Me._semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated)
		End Function

		Private Function CreateBoundUserDefinedUnaryOperatorOperation(ByVal boundUserDefinedUnaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator) As IUnaryOperation
			Dim operation As IOperation = Me.CreateBoundUnaryOperatorChild(boundUserDefinedUnaryOperator)
			Dim unaryOperatorKind As Microsoft.CodeAnalysis.Operations.UnaryOperatorKind = VisualBasicOperationFactory.Helper.DeriveUnaryOperatorKind(boundUserDefinedUnaryOperator.OperatorKind)
			Dim methodSymbol As IMethodSymbol = VisualBasicOperationFactory.TryGetOperatorMethod(boundUserDefinedUnaryOperator)
			Dim syntax As SyntaxNode = boundUserDefinedUnaryOperator.Syntax
			Dim type As ITypeSymbol = boundUserDefinedUnaryOperator.Type
			Dim constantValueOpt As ConstantValue = boundUserDefinedUnaryOperator.ConstantValueOpt
			Dim operatorKind As Boolean = CInt((boundUserDefinedUnaryOperator.OperatorKind And Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Lifted)) <> 0
			Dim flag As Boolean = False
			Dim wasCompilerGenerated As Boolean = boundUserDefinedUnaryOperator.WasCompilerGenerated
			Return New UnaryOperation(unaryOperatorKind, operation, operatorKind, flag, methodSymbol, Me._semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated)
		End Function

		Private Function CreateBoundUsingStatementOperation(ByVal boundUsingStatement As Microsoft.CodeAnalysis.VisualBasic.BoundUsingStatement) As IUsingOperation
			Dim operation As IOperation = Me.CreateBoundUsingStatementResources(boundUsingStatement)
			Dim operation1 As IOperation = Me.Create(boundUsingStatement.Body)
			Dim localSymbols As ImmutableArray(Of ILocalSymbol) = ImmutableArray(Of ILocalSymbol).CastUp(Of LocalSymbol)(boundUsingStatement.Locals)
			Dim syntax As SyntaxNode = boundUsingStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundUsingStatement.WasCompilerGenerated
			Dim disposeOperationInfo As Microsoft.CodeAnalysis.Operations.DisposeOperationInfo = New Microsoft.CodeAnalysis.Operations.DisposeOperationInfo()
			Return New UsingOperation(operation, operation1, localSymbols, False, disposeOperationInfo, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Friend Function CreateBoundUsingStatementResources(ByVal boundUsingStatement As Microsoft.CodeAnalysis.VisualBasic.BoundUsingStatement) As IOperation
			Dim usingStatementDeclaration As IOperation
			If (boundUsingStatement.ResourceList.IsDefault) Then
				usingStatementDeclaration = Me.Create(boundUsingStatement.ResourceExpressionOpt)
			Else
				usingStatementDeclaration = Me.GetUsingStatementDeclaration(boundUsingStatement.ResourceList, DirectCast(boundUsingStatement.Syntax, UsingBlockSyntax).UsingStatement)
			End If
			Return usingStatementDeclaration
		End Function

		Private Function CreateBoundWhileStatementOperation(ByVal boundWhileStatement As Microsoft.CodeAnalysis.VisualBasic.BoundWhileStatement) As IWhileLoopOperation
			Dim operation As IOperation = Me.Create(boundWhileStatement.Condition)
			Dim operation1 As IOperation = Me.Create(boundWhileStatement.Body)
			Dim operation2 As IOperation = Nothing
			Dim empty As ImmutableArray(Of ILocalSymbol) = ImmutableArray(Of ILocalSymbol).Empty
			Dim continueLabel As ILabelSymbol = boundWhileStatement.ContinueLabel
			Dim exitLabel As ILabelSymbol = boundWhileStatement.ExitLabel
			Dim flag As Boolean = True
			Dim flag1 As Boolean = False
			Dim syntax As SyntaxNode = boundWhileStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundWhileStatement.WasCompilerGenerated
			Return New WhileLoopOperation(operation, flag, flag1, operation2, operation1, empty, continueLabel, exitLabel, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundWithLValueExpressionPlaceholder(ByVal boundWithLValueExpressionPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundWithLValueExpressionPlaceholder) As IInstanceReferenceOperation
			Dim syntax As SyntaxNode = boundWithLValueExpressionPlaceholder.Syntax
			Dim type As ITypeSymbol = boundWithLValueExpressionPlaceholder.Type
			Dim wasCompilerGenerated As Boolean = boundWithLValueExpressionPlaceholder.WasCompilerGenerated
			Return New InstanceReferenceOperation(InstanceReferenceKind.ImplicitReceiver, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundWithRValueExpressionPlaceholder(ByVal boundWithRValueExpressionPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundWithRValueExpressionPlaceholder) As IInstanceReferenceOperation
			Dim syntax As SyntaxNode = boundWithRValueExpressionPlaceholder.Syntax
			Dim type As ITypeSymbol = boundWithRValueExpressionPlaceholder.Type
			Dim wasCompilerGenerated As Boolean = boundWithRValueExpressionPlaceholder.WasCompilerGenerated
			Return New InstanceReferenceOperation(InstanceReferenceKind.ImplicitReceiver, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function CreateBoundWithStatementOperation(ByVal boundWithStatement As Microsoft.CodeAnalysis.VisualBasic.BoundWithStatement) As IWithStatementOperation
			Dim operation As IOperation = Me.Create(boundWithStatement.OriginalExpression)
			Dim operation1 As IOperation = Me.Create(boundWithStatement.Body)
			Dim syntax As SyntaxNode = boundWithStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundWithStatement.WasCompilerGenerated
			Return New WithStatementOperation(operation1, operation, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateBoundYieldStatementOperation(ByVal boundYieldStatement As Microsoft.CodeAnalysis.VisualBasic.BoundYieldStatement) As IReturnOperation
			Dim operation As IOperation = Me.Create(boundYieldStatement.Expression)
			Dim syntax As SyntaxNode = boundYieldStatement.Syntax
			Dim wasCompilerGenerated As Boolean = boundYieldStatement.WasCompilerGenerated
			Return New ReturnOperation(operation, OperationKind.YieldReturn, Me._semanticModel, syntax, wasCompilerGenerated)
		End Function

		Private Function CreateCompoundAssignment(ByVal boundAssignment As BoundAssignmentOperator) As ICompoundAssignmentOperation
			Dim binaryOperatorInfo As VisualBasicOperationFactory.BinaryOperatorInfo
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(Conversions.Identity)
			Dim conversion1 As Microsoft.CodeAnalysis.VisualBasic.Conversion = conversion
			Dim right As BoundExpression = Nothing
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.BoundKind = boundAssignment.Right.Kind
			If (CByte(kind) - CByte(Microsoft.CodeAnalysis.VisualBasic.BoundKind.BinaryOperator) <= CByte(Microsoft.CodeAnalysis.VisualBasic.BoundKind.OmittedArgument)) Then
				right = boundAssignment.Right
			Else
				If (kind <> Microsoft.CodeAnalysis.VisualBasic.BoundKind.Conversion) Then
					Throw ExceptionUtilities.UnexpectedValue(boundAssignment.Kind)
				End If
				Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(boundAssignment.Right, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
				conversion1 = VisualBasicOperationFactory.CreateConversion(boundConversion)
				right = VisualBasicOperationFactory.GetConversionOperand(boundConversion)
			End If
			Dim boundKind As Microsoft.CodeAnalysis.VisualBasic.BoundKind = right.Kind
			If (boundKind = Microsoft.CodeAnalysis.VisualBasic.BoundKind.BinaryOperator) Then
				binaryOperatorInfo = VisualBasicOperationFactory.GetBinaryOperatorInfo(DirectCast(right, BoundBinaryOperator))
			Else
				If (boundKind <> Microsoft.CodeAnalysis.VisualBasic.BoundKind.UserDefinedBinaryOperator) Then
					Throw ExceptionUtilities.UnexpectedValue(boundAssignment.Kind)
				End If
				binaryOperatorInfo = VisualBasicOperationFactory.GetUserDefinedBinaryOperatorInfo(DirectCast(right, BoundUserDefinedBinaryOperator))
			End If
			Dim leftOperand As BoundExpression = binaryOperatorInfo.LeftOperand
			If (leftOperand.Kind = Microsoft.CodeAnalysis.VisualBasic.BoundKind.Conversion) Then
				Dim boundConversion1 As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(leftOperand, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
				conversion = VisualBasicOperationFactory.CreateConversion(boundConversion1)
				leftOperand = VisualBasicOperationFactory.GetConversionOperand(boundConversion1)
			End If
			Dim operation As IOperation = Me.Create(boundAssignment.Left)
			Dim operation1 As IOperation = Me.CreateCompoundAssignmentRightOperand(boundAssignment)
			Dim syntax As SyntaxNode = boundAssignment.Syntax
			Dim type As ITypeSymbol = boundAssignment.Type
			Dim wasCompilerGenerated As Boolean = boundAssignment.WasCompilerGenerated
			Return New CompoundAssignmentOperation(DirectCast(conversion, IConvertibleConversion), DirectCast(conversion1, IConvertibleConversion), binaryOperatorInfo.OperatorKind, binaryOperatorInfo.IsLifted, binaryOperatorInfo.IsChecked, binaryOperatorInfo.OperatorMethod, operation, operation1, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Friend Function CreateCompoundAssignmentRightOperand(ByVal boundAssignment As BoundAssignmentOperator) As IOperation
			Dim userDefinedBinaryOperatorChild As IOperation
			Dim binaryOperatorInfo As VisualBasicOperationFactory.BinaryOperatorInfo
			Dim right As BoundExpression = Nothing
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.BoundKind = boundAssignment.Right.Kind
			If (CByte(kind) - CByte(Microsoft.CodeAnalysis.VisualBasic.BoundKind.BinaryOperator) <= CByte(Microsoft.CodeAnalysis.VisualBasic.BoundKind.OmittedArgument)) Then
				right = boundAssignment.Right
			Else
				If (kind <> Microsoft.CodeAnalysis.VisualBasic.BoundKind.Conversion) Then
					Throw ExceptionUtilities.UnexpectedValue(boundAssignment.Kind)
				End If
				right = VisualBasicOperationFactory.GetConversionOperand(DirectCast(boundAssignment.Right, BoundConversion))
			End If
			Dim boundKind As Microsoft.CodeAnalysis.VisualBasic.BoundKind = right.Kind
			If (boundKind = Microsoft.CodeAnalysis.VisualBasic.BoundKind.BinaryOperator) Then
				binaryOperatorInfo = VisualBasicOperationFactory.GetBinaryOperatorInfo(DirectCast(right, BoundBinaryOperator))
				userDefinedBinaryOperatorChild = Me.Create(binaryOperatorInfo.RightOperand)
			Else
				If (boundKind <> Microsoft.CodeAnalysis.VisualBasic.BoundKind.UserDefinedBinaryOperator) Then
					Throw ExceptionUtilities.UnexpectedValue(boundAssignment.Kind)
				End If
				Dim boundUserDefinedBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator = DirectCast(right, Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator)
				binaryOperatorInfo = VisualBasicOperationFactory.GetUserDefinedBinaryOperatorInfo(boundUserDefinedBinaryOperator)
				userDefinedBinaryOperatorChild = Me.GetUserDefinedBinaryOperatorChild(boundUserDefinedBinaryOperator, binaryOperatorInfo.RightOperand)
			End If
			Return userDefinedBinaryOperatorChild
		End Function

		Private Shared Function CreateConversion(ByVal expression As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion
			If (expression.Kind <> BoundKind.Conversion) Then
				conversion = If(expression.Kind = BoundKind.[TryCast] OrElse expression.Kind = BoundKind.[DirectCast], New Microsoft.CodeAnalysis.VisualBasic.Conversion(KeyValuePairUtil.Create(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind, MethodSymbol)(DirectCast(expression, BoundConversionOrCast).ConversionKind, Nothing)), New Microsoft.CodeAnalysis.VisualBasic.Conversion(Conversions.Identity))
			Else
				Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(expression, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
				Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = boundConversion.ConversionKind
				Dim method As MethodSymbol = Nothing
				If (conversionKind.HasFlag(Microsoft.CodeAnalysis.VisualBasic.ConversionKind.UserDefined) AndAlso boundConversion.Operand.Kind = BoundKind.UserDefinedConversion) Then
					method = DirectCast(boundConversion.Operand, BoundUserDefinedConversion).[Call].Method
				End If
				conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(KeyValuePairUtil.Create(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind, MethodSymbol)(conversionKind, method))
			End If
			Return conversion
		End Function

		Private Function CreateDelegateCreationConversionOperand(ByVal operand As BoundExpression) As IOperation
			Dim operation As IOperation
			If (operand.Kind <> BoundKind.DelegateCreationExpression) Then
				operation = Me.Create(operand)
			Else
				operation = Me.CreateBoundDelegateCreationExpressionChildOperation(DirectCast(operand, BoundDelegateCreationExpression))
			End If
			Return operation
		End Function

		Public Function CreateFromArray(Of TBoundNode As BoundNode, TOperation As {Class, IOperation})(ByVal nodeArray As ImmutableArray(Of TBoundNode)) As ImmutableArray(Of TOperation)
			Dim instance As ArrayBuilder(Of TOperation) = ArrayBuilder(Of TOperation).GetInstance(nodeArray.Length)
			Dim enumerator As ImmutableArray(Of TBoundNode).Enumerator = nodeArray.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As TBoundNode = enumerator.Current
				instance.AddIfNotNull(DirectCast(Me.Create(DirectCast(current, BoundNode)), TOperation))
			End While
			Return instance.ToImmutableAndFree()
		End Function

		Private Function CreateNoPiaObjectCreationExpressionOperation(ByVal creation As BoundNoPiaObjectCreationExpression) As INoPiaObjectCreationOperation
			Dim objectOrCollectionInitializerOperation As IObjectOrCollectionInitializerOperation = DirectCast(Me.Create(creation.InitializerOpt), IObjectOrCollectionInitializerOperation)
			Dim syntax As SyntaxNode = creation.Syntax
			Dim type As ITypeSymbol = creation.Type
			Dim wasCompilerGenerated As Boolean = creation.WasCompilerGenerated
			Return New NoPiaObjectCreationOperation(objectOrCollectionInitializerOperation, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Friend Function CreateReceiverOperation(ByVal node As BoundNode, ByVal symbol As ISymbol) As IOperation
			Dim operation As IOperation
			If (node Is Nothing OrElse node.Kind = BoundKind.TypeExpression) Then
				operation = Nothing
			ElseIf (symbol Is Nothing OrElse Not node.WasCompilerGenerated OrElse Not symbol.IsStatic OrElse node.Kind <> BoundKind.MeReference AndAlso node.Kind <> BoundKind.WithLValueExpressionPlaceholder AndAlso node.Kind <> BoundKind.WithRValueExpressionPlaceholder) Then
				operation = Me.Create(node)
			Else
				operation = Nothing
			End If
			Return operation
		End Function

		Private Function CreateTupleOperation(ByVal boundTupleExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTupleExpression, ByVal naturalType As ITypeSymbol) As ITupleOperation
			Dim operations As ImmutableArray(Of IOperation) = Me.CreateFromArray(Of BoundExpression, IOperation)(boundTupleExpression.Arguments)
			Dim syntax As SyntaxNode = boundTupleExpression.Syntax
			Dim type As ITypeSymbol = boundTupleExpression.Type
			Dim wasCompilerGenerated As Boolean = boundTupleExpression.WasCompilerGenerated
			Return New TupleOperation(operations, naturalType, Me._semanticModel, syntax, type, wasCompilerGenerated)
		End Function

		Private Function DeriveArgument(ByVal index As Integer, ByVal argument As BoundExpression, ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal isDefault As Boolean) As IArgumentOperation
			Dim argumentOperation As IArgumentOperation
			Dim flag As Boolean = If(Not argument.WasCompilerGenerated, False, argument.Syntax.Kind() <> SyntaxKind.OmittedArgument)
			If (argument.Kind <> BoundKind.ByRefArgumentWithCopyBack) Then
				Dim length As Integer = parameters.Length
				Dim argumentKind As Microsoft.CodeAnalysis.Operations.ArgumentKind = Microsoft.CodeAnalysis.Operations.ArgumentKind.Explicit
				If (argument.WasCompilerGenerated) Then
					If (isDefault) Then
						argumentKind = Microsoft.CodeAnalysis.Operations.ArgumentKind.DefaultValue
					ElseIf (argument.Kind = BoundKind.ArrayCreation AndAlso DirectCast(argument, BoundArrayCreation).IsParamArrayArgument) Then
						argumentKind = Microsoft.CodeAnalysis.Operations.ArgumentKind.[ParamArray]
					End If
				End If
				argumentOperation = Me.CreateArgumentOperation(argumentKind, parameters(index), argument, New Conversion(Conversions.Identity), New Conversion(Conversions.Identity), flag)
			Else
				Dim boundByRefArgumentWithCopyBack As Microsoft.CodeAnalysis.VisualBasic.BoundByRefArgumentWithCopyBack = DirectCast(argument, Microsoft.CodeAnalysis.VisualBasic.BoundByRefArgumentWithCopyBack)
				Dim item As ParameterSymbol = parameters(index)
				argumentOperation = Me.CreateArgumentOperation(Microsoft.CodeAnalysis.Operations.ArgumentKind.Explicit, item, boundByRefArgumentWithCopyBack.OriginalArgument, VisualBasicOperationFactory.CreateConversion(boundByRefArgumentWithCopyBack.InConversion), VisualBasicOperationFactory.CreateConversion(boundByRefArgumentWithCopyBack.OutConversion), flag)
			End If
			Return argumentOperation
		End Function

		Friend Function DeriveArguments(ByVal boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As ImmutableArray(Of IArgumentOperation)
			Dim empty As ImmutableArray(Of IArgumentOperation)
			Dim defaultArguments As BitVector
			Dim argumentOperations As ImmutableArray(Of IArgumentOperation)
			Dim empty1 As ImmutableArray(Of IArgumentOperation)
			Dim kind As BoundKind = boundNode.Kind
			If (kind <= BoundKind.ObjectCreationExpression) Then
				If (kind = BoundKind.[Call]) Then
					Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = DirectCast(boundNode, Microsoft.CodeAnalysis.VisualBasic.BoundCall)
					Dim arguments As ImmutableArray(Of BoundExpression) = boundCall.Arguments
					Dim parameters As ImmutableArray(Of ParameterSymbol) = boundCall.Method.Parameters
					defaultArguments = boundCall.DefaultArguments
					empty = Me.DeriveArguments(arguments, parameters, defaultArguments)
				Else
					If (kind <> BoundKind.ObjectCreationExpression) Then
						Throw ExceptionUtilities.UnexpectedValue(boundNode.Kind)
					End If
					Dim boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression = DirectCast(boundNode, Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression)
					If (Not boundObjectCreationExpression.Arguments.IsDefault) Then
						If (boundObjectCreationExpression.ConstructorOpt Is Nothing) Then
							empty1 = ImmutableArray(Of IArgumentOperation).Empty
						Else
							Dim boundExpressions As ImmutableArray(Of BoundExpression) = boundObjectCreationExpression.Arguments
							Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = boundObjectCreationExpression.ConstructorOpt.Parameters
							defaultArguments = boundObjectCreationExpression.DefaultArguments
							empty1 = Me.DeriveArguments(boundExpressions, parameterSymbols, defaultArguments)
						End If
						empty = empty1
					Else
						empty = ImmutableArray(Of IArgumentOperation).Empty
					End If
				End If
			ElseIf (kind = BoundKind.PropertyAccess) Then
				Dim boundPropertyAccess As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess = DirectCast(boundNode, Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess)
				If (boundPropertyAccess.Arguments.Length = 0) Then
					argumentOperations = ImmutableArray(Of IArgumentOperation).Empty
				Else
					Dim arguments1 As ImmutableArray(Of BoundExpression) = boundPropertyAccess.Arguments
					Dim parameters1 As ImmutableArray(Of ParameterSymbol) = boundPropertyAccess.PropertySymbol.Parameters
					defaultArguments = boundPropertyAccess.DefaultArguments
					argumentOperations = Me.DeriveArguments(arguments1, parameters1, defaultArguments)
				End If
				empty = argumentOperations
			Else
				If (kind <> BoundKind.RaiseEventStatement) Then
					Throw ExceptionUtilities.UnexpectedValue(boundNode.Kind)
				End If
				empty = Me.DeriveArguments(DirectCast(DirectCast(boundNode, BoundRaiseEventStatement).EventInvocation, Microsoft.CodeAnalysis.VisualBasic.BoundCall))
			End If
			Return empty
		End Function

		Friend Function DeriveArguments(ByVal boundArguments As ImmutableArray(Of BoundExpression), ByVal parameters As ImmutableArray(Of ParameterSymbol), ByRef defaultArguments As BitVector) As ImmutableArray(Of IArgumentOperation)
			Dim length As Integer = boundArguments.Length
			Dim instance As ArrayBuilder(Of IArgumentOperation) = ArrayBuilder(Of IArgumentOperation).GetInstance(length)
			Dim num As Integer = length - 1
			Dim num1 As Integer = 0
			Do
				instance.Add(Me.DeriveArgument(num1, boundArguments(num1), parameters, defaultArguments(num1)))
				num1 = num1 + 1
			Loop While num1 <= num
			Return instance.ToImmutableAndFree()
		End Function

		Friend Function GetAddRemoveHandlerStatementExpression(ByVal statement As BoundAddRemoveHandlerStatement) As IOperation
			Dim operation As IOperation = Me.Create(statement.EventAccess)
			Dim operation1 As IOperation = Me.Create(statement.Handler)
			Dim kind As Boolean = statement.Kind = BoundKind.AddHandlerStatement
			Return New EventAssignmentOperation(operation, operation1, kind, Me._semanticModel, statement.Syntax, Nothing, True)
		End Function

		Friend Function GetAnonymousTypeCreationInitializers(ByVal expression As BoundAnonymousTypeCreationExpression) As ImmutableArray(Of IOperation)
			Dim propertyReferenceOperation As IOperation
			Dim wasCompilerGenerated As Boolean
			Dim parent As Object
			Dim properties As ImmutableArray(Of AnonymousTypeManager.AnonymousTypePropertyPublicSymbol) = DirectCast(expression.Type, AnonymousTypeManager.AnonymousTypePublicSymbol).Properties
			Dim arguments As ImmutableArray(Of BoundExpression) = expression.Arguments
			Dim instance As ArrayBuilder(Of IOperation) = ArrayBuilder(Of IOperation).GetInstance(arguments.Length)
			Dim num As Integer = 0
			arguments = expression.Arguments
			Dim length As Integer = arguments.Length - 1
			Dim num1 As Integer = 0
			Do
				arguments = expression.Arguments
				Dim operation As IOperation = Me.Create(arguments(num1))
				If (num >= expression.Declarations.Length OrElse num1 <> expression.Declarations(num).PropertyIndex) Then
					Dim item As IPropertySymbol = properties(num1)
					Dim instanceReferenceOperation As IInstanceReferenceOperation = Me.CreateAnonymousTypePropertyAccessImplicitReceiverOperation(item, expression.Syntax)
					propertyReferenceOperation = New Microsoft.CodeAnalysis.Operations.PropertyReferenceOperation(item, ImmutableArray(Of IArgumentOperation).Empty, instanceReferenceOperation, Me._semanticModel, operation.Syntax, item.Type, True)
					wasCompilerGenerated = True
				Else
					propertyReferenceOperation = Me.CreateBoundAnonymousTypePropertyAccessOperation(expression.Declarations(num))
					num = num + 1
					wasCompilerGenerated = expression.WasCompilerGenerated
				End If
				Dim flag As Boolean = False
				Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = operation.Syntax
				If (syntax IsNot Nothing) Then
					parent = syntax.Parent
				Else
					parent = Nothing
				End If
				If (parent Is Nothing) Then
					parent = expression.Syntax
				End If
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = parent
				Dim type As ITypeSymbol = propertyReferenceOperation.Type
				Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = operation.GetConstantValue()
				Dim simpleAssignmentOperation As Microsoft.CodeAnalysis.Operations.SimpleAssignmentOperation = New Microsoft.CodeAnalysis.Operations.SimpleAssignmentOperation(flag, propertyReferenceOperation, operation, Me._semanticModel, syntaxNode, type, constantValue, wasCompilerGenerated)
				instance.Add(simpleAssignmentOperation)
				num1 = num1 + 1
			Loop While num1 <= length
			Return instance.ToImmutableAndFree()
		End Function

		Private Shared Function GetBinaryOperatorInfo(ByVal boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) As VisualBasicOperationFactory.BinaryOperatorInfo
			Return New VisualBasicOperationFactory.BinaryOperatorInfo(boundBinaryOperator.Left, boundBinaryOperator.Right, VisualBasicOperationFactory.Helper.DeriveBinaryOperatorKind(boundBinaryOperator.OperatorKind, boundBinaryOperator.Left), Nothing, CInt((boundBinaryOperator.OperatorKind And Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Lifted)) <> 0, boundBinaryOperator.Checked, CInt((boundBinaryOperator.OperatorKind And Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.CompareText)) <> 0)
		End Function

		Friend Shared Function GetCaseClauseValue(ByVal valueOpt As BoundExpression, ByVal conditionOpt As BoundExpression) As BoundExpression
			Dim right As BoundExpression
			If (valueOpt Is Nothing) Then
				Dim kind As BoundKind = conditionOpt.Kind
				If (kind = BoundKind.BinaryOperator) Then
					right = DirectCast(conditionOpt, BoundBinaryOperator).Right
				Else
					If (kind <> BoundKind.UserDefinedBinaryOperator) Then
						Throw ExceptionUtilities.UnexpectedValue(conditionOpt.Kind)
					End If
					right = VisualBasicOperationFactory.GetUserDefinedBinaryOperatorChildBoundNode(DirectCast(conditionOpt, BoundUserDefinedBinaryOperator), 1)
				End If
			Else
				right = valueOpt
			End If
			Return right
		End Function

		Private Function GetChildOfBadExpression(ByVal parent As BoundNode, ByVal index As Integer) As IOperation
			Dim operation As IOperation
			Dim operation1 As IOperation = Me.Create(VisualBasicOperationFactory.GetChildOfBadExpressionBoundNode(parent, index))
			If (operation1 Is Nothing) Then
				Dim wasCompilerGenerated As Boolean = parent.WasCompilerGenerated
				operation = OperationFactory.CreateInvalidOperation(Me._semanticModel, parent.Syntax, ImmutableArray(Of IOperation).Empty, wasCompilerGenerated)
			Else
				operation = operation1
			End If
			Return operation
		End Function

		Private Shared Function GetChildOfBadExpressionBoundNode(ByVal parent As BoundNode, ByVal index As Integer) As BoundExpression
			Dim item As BoundExpression
			Dim boundBadExpression As Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression = TryCast(parent, Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression)
			If (If(boundBadExpression IsNot Nothing, boundBadExpression.ChildBoundNodes.Length <= index, True)) Then
				item = Nothing
			Else
				item = boundBadExpression.ChildBoundNodes(index)
			End If
			Return item
		End Function

		Private Function GetConversionInfo(ByVal boundConversion As BoundConversionOrCast) As <TupleElementNames(New String() { "Operation", "Conversion", "IsDelegateCreation" })> ValueTuple(Of IOperation, Microsoft.CodeAnalysis.VisualBasic.Conversion, Boolean)
			Dim valueTuple As ValueTuple(Of IOperation, Microsoft.CodeAnalysis.VisualBasic.Conversion, Boolean)
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion = VisualBasicOperationFactory.CreateConversion(boundConversion)
			Dim conversionOperand As BoundExpression = VisualBasicOperationFactory.GetConversionOperand(boundConversion)
			If (conversion.IsIdentity AndAlso boundConversion.ExplicitCastInCode) Then
				Dim valueTuple1 As ValueTuple(Of IOperation, Microsoft.CodeAnalysis.VisualBasic.Conversion, Boolean) = Me.TryGetAdjustedConversionInfo(boundConversion, conversionOperand)
				If (valueTuple1.Item1 Is Nothing) Then
					valueTuple = If(Not VisualBasicOperationFactory.IsDelegateCreation(boundConversion.Syntax, conversionOperand, boundConversion.Type), New ValueTuple(Of IOperation, Microsoft.CodeAnalysis.VisualBasic.Conversion, Boolean)(Me.Create(conversionOperand), conversion, False), New ValueTuple(Of IOperation, Microsoft.CodeAnalysis.VisualBasic.Conversion, Boolean)(Me.CreateDelegateCreationConversionOperand(conversionOperand), conversion, True))
					Return valueTuple
				End If
				valueTuple = valueTuple1
				Return valueTuple
			End If
			valueTuple = If(Not VisualBasicOperationFactory.IsDelegateCreation(boundConversion.Syntax, conversionOperand, boundConversion.Type), New ValueTuple(Of IOperation, Microsoft.CodeAnalysis.VisualBasic.Conversion, Boolean)(Me.Create(conversionOperand), conversion, False), New ValueTuple(Of IOperation, Microsoft.CodeAnalysis.VisualBasic.Conversion, Boolean)(Me.CreateDelegateCreationConversionOperand(conversionOperand), conversion, True))
			Return valueTuple
		End Function

		Private Shared Function GetConversionOperand(ByVal boundConversion As BoundConversionOrCast) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			boundExpression = If((boundConversion.ConversionKind And ConversionKind.UserDefined) <> ConversionKind.UserDefined, boundConversion.Operand, DirectCast(boundConversion.Operand, BoundUserDefinedConversion).Operand)
			Return boundExpression
		End Function

		Friend Function GetForEachLoopOperationInfo(ByVal boundForEachStatement As Microsoft.CodeAnalysis.VisualBasic.BoundForEachStatement) As ForEachLoopOperationInfo
			Dim argumentOperations As ImmutableArray(Of IArgumentOperation)
			Dim flag As Boolean
			Dim argumentOperations1 As ImmutableArray(Of IArgumentOperation)
			Dim argumentOperations2 As ImmutableArray(Of IArgumentOperation)
			Dim argumentOperations3 As ImmutableArray(Of IArgumentOperation)
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = New ImmutableArray(Of BoundExpression)()
			Dim null As Microsoft.CodeAnalysis.BitVector = Microsoft.CodeAnalysis.BitVector.Null
			Dim boundExpressions1 As ImmutableArray(Of BoundExpression) = New ImmutableArray(Of BoundExpression)()
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = Microsoft.CodeAnalysis.BitVector.Null
			Dim boundExpressions2 As ImmutableArray(Of BoundExpression) = New ImmutableArray(Of BoundExpression)()
			Dim null1 As Microsoft.CodeAnalysis.BitVector = Microsoft.CodeAnalysis.BitVector.Null
			Dim forEachStatementInfo As Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo = MemberSemanticModel.GetForEachStatementInfo(boundForEachStatement, DirectCast(Me._semanticModel.Compilation, VisualBasicCompilation), boundExpressions, null, boundExpressions1, bitVector, boundExpressions2, null1)
			Dim elementType As ITypeSymbol = forEachStatementInfo.ElementType
			Dim getEnumeratorMethod As IMethodSymbol = forEachStatementInfo.GetEnumeratorMethod
			Dim currentProperty As IPropertySymbol = forEachStatementInfo.CurrentProperty
			Dim moveNextMethod As IMethodSymbol = forEachStatementInfo.MoveNextMethod
			Dim needToDispose As Boolean = boundForEachStatement.EnumeratorInfo.NeedToDispose
			flag = If(Not boundForEachStatement.EnumeratorInfo.NeedToDispose, False, boundForEachStatement.EnumeratorInfo.IsOrInheritsFromOrImplementsIDisposable)
			Dim currentConversion As IConvertibleConversion = DirectCast(forEachStatementInfo.CurrentConversion, IConvertibleConversion)
			Dim elementConversion As IConvertibleConversion = DirectCast(forEachStatementInfo.ElementConversion, IConvertibleConversion)
			If (boundExpressions.IsDefaultOrEmpty) Then
				argumentOperations = New ImmutableArray(Of IArgumentOperation)()
				argumentOperations1 = argumentOperations
			Else
				argumentOperations1 = Operation.SetParentOperation(Of IArgumentOperation)(Me.DeriveArguments(boundExpressions, DirectCast(forEachStatementInfo.GetEnumeratorMethod, MethodSymbol).Parameters, null), Nothing)
			End If
			If (boundExpressions1.IsDefaultOrEmpty) Then
				argumentOperations = New ImmutableArray(Of IArgumentOperation)()
				argumentOperations2 = argumentOperations
			Else
				argumentOperations2 = Operation.SetParentOperation(Of IArgumentOperation)(Me.DeriveArguments(boundExpressions1, DirectCast(forEachStatementInfo.MoveNextMethod, MethodSymbol).Parameters, bitVector), Nothing)
			End If
			If (boundExpressions2.IsDefaultOrEmpty) Then
				argumentOperations = New ImmutableArray(Of IArgumentOperation)()
				argumentOperations3 = argumentOperations
			Else
				argumentOperations3 = Operation.SetParentOperation(Of IArgumentOperation)(Me.DeriveArguments(boundExpressions2, DirectCast(forEachStatementInfo.CurrentProperty, PropertySymbol).Parameters, null1), Nothing)
			End If
			argumentOperations = New ImmutableArray(Of IArgumentOperation)()
			Return New ForEachLoopOperationInfo(elementType, getEnumeratorMethod, currentProperty, moveNextMethod, False, needToDispose, flag, Nothing, currentConversion, elementConversion, argumentOperations1, argumentOperations2, argumentOperations3, argumentOperations)
		End Function

		Friend Function GetIOperationChildren(ByVal boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As ImmutableArray(Of IOperation)
			Dim immutableAndFree As ImmutableArray(Of IOperation)
			Dim boundNodeWithIOperationChild As IBoundNodeWithIOperationChildren = boundNode
			If (Not boundNodeWithIOperationChild.Children.IsDefaultOrEmpty) Then
				Dim instance As ArrayBuilder(Of IOperation) = ArrayBuilder(Of IOperation).GetInstance(boundNodeWithIOperationChild.Children.Length)
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundNode).Enumerator = boundNodeWithIOperationChild.Children.GetEnumerator()
				While enumerator.MoveNext()
					Dim operation As IOperation = Me.Create(enumerator.Current)
					If (operation Is Nothing) Then
						Continue While
					End If
					instance.Add(operation)
				End While
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray(Of IOperation).Empty
			End If
			Return immutableAndFree
		End Function

		Private Function GetObjectCreationInitializers(ByVal expression As BoundObjectCreationExpression) As ImmutableArray(Of IOperation)
			If (expression.InitializerOpt Is Nothing) Then
				Return ImmutableArray(Of IOperation).Empty
			End If
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of BoundExpression, IOperation)(expression.InitializerOpt.Initializers, Function(n As BoundExpression) Me.Create(n))
		End Function

		Private Shared Function GetSingleValueCaseClauseValue(ByVal clause As BoundSingleValueCaseClause) As BoundExpression
			Return VisualBasicOperationFactory.GetCaseClauseValue(clause.ValueOpt, clause.ConditionOpt)
		End Function

		Private Function GetUserDefinedBinaryOperatorChild(ByVal [operator] As BoundUserDefinedBinaryOperator, ByVal child As BoundExpression) As IOperation
			Dim operation As IOperation
			If (child Is Nothing) Then
				Dim wasCompilerGenerated As Boolean = [operator].UnderlyingExpression.WasCompilerGenerated
				operation = OperationFactory.CreateInvalidOperation(Me._semanticModel, [operator].UnderlyingExpression.Syntax, ImmutableArray(Of IOperation).Empty, wasCompilerGenerated)
			Else
				operation = Me.Create(child)
			End If
			Return operation
		End Function

		Private Shared Function GetUserDefinedBinaryOperatorChildBoundNode(ByVal [operator] As BoundUserDefinedBinaryOperator, ByVal index As Integer) As BoundExpression
			Dim childOfBadExpressionBoundNode As BoundExpression
			If ([operator].UnderlyingExpression.Kind <> BoundKind.[Call]) Then
				childOfBadExpressionBoundNode = VisualBasicOperationFactory.GetChildOfBadExpressionBoundNode([operator].UnderlyingExpression, index)
			ElseIf (index <> 0) Then
				If (index <> 1) Then
					Throw ExceptionUtilities.UnexpectedValue(index)
				End If
				childOfBadExpressionBoundNode = [operator].Right
			Else
				childOfBadExpressionBoundNode = [operator].Left
			End If
			Return childOfBadExpressionBoundNode
		End Function

		Private Shared Function GetUserDefinedBinaryOperatorInfo(ByVal boundUserDefinedBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator) As VisualBasicOperationFactory.BinaryOperatorInfo
			Dim method As MethodSymbol
			Dim userDefinedBinaryOperatorChildBoundNode As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = VisualBasicOperationFactory.GetUserDefinedBinaryOperatorChildBoundNode(boundUserDefinedBinaryOperator, 0)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = VisualBasicOperationFactory.GetUserDefinedBinaryOperatorChildBoundNode(boundUserDefinedBinaryOperator, 1)
			Dim binaryOperatorKind As Microsoft.CodeAnalysis.Operations.BinaryOperatorKind = VisualBasicOperationFactory.Helper.DeriveBinaryOperatorKind(boundUserDefinedBinaryOperator.OperatorKind, Nothing)
			If (boundUserDefinedBinaryOperator.UnderlyingExpression.Kind = BoundKind.[Call]) Then
				method = boundUserDefinedBinaryOperator.[Call].Method
			Else
				method = Nothing
			End If
			Return New VisualBasicOperationFactory.BinaryOperatorInfo(userDefinedBinaryOperatorChildBoundNode, boundExpression, binaryOperatorKind, method, CInt((boundUserDefinedBinaryOperator.OperatorKind And Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Lifted)) <> 0, boundUserDefinedBinaryOperator.Checked, False)
		End Function

		Private Function GetUsingStatementDeclaration(ByVal resourceList As ImmutableArray(Of BoundLocalDeclarationBase), ByVal syntax As SyntaxNode) As IVariableDeclarationGroupOperation
			Return New VariableDeclarationGroupOperation(Me.GetVariableDeclarationStatementVariables(resourceList), Me._semanticModel, syntax, False)
		End Function

		Friend Function GetVariableDeclarationStatementVariables(ByVal declarations As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase)) As ImmutableArray(Of IVariableDeclarationOperation)
			Dim enumerator As IEnumerator(Of IGrouping(Of SyntaxNode, Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase)) = Nothing
			Dim func As Func(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase, SyntaxNode)
			Dim boundLocalDeclarationBases As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase) = DirectCast(declarations, IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase))
			If (VisualBasicOperationFactory._Closure$__.$I140-0 Is Nothing) Then
				func = Function(declaration As Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase) If(declaration.Kind <> BoundKind.LocalDeclaration OrElse Not declaration.Syntax.IsKind(SyntaxKind.ModifiedIdentifier), declaration.Syntax, declaration.Syntax.Parent)
				VisualBasicOperationFactory._Closure$__.$I140-0 = func
			Else
				func = VisualBasicOperationFactory._Closure$__.$I140-0
			End If
			Dim groupings As IEnumerable(Of IGrouping(Of SyntaxNode, Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase)) = boundLocalDeclarationBases.GroupBy(Of SyntaxNode)(func)
			Using instance As ArrayBuilder(Of IVariableDeclarationOperation) = ArrayBuilder(Of IVariableDeclarationOperation).GetInstance()
				enumerator = groupings.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As IGrouping(Of SyntaxNode, Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase) = enumerator.Current
					Dim boundLocalDeclarationBase As Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase = current.First()
					Dim variableDeclaratorOperations As ImmutableArray(Of IVariableDeclaratorOperation) = New ImmutableArray(Of IVariableDeclaratorOperation)()
					Dim variableInitializerOperation As IVariableInitializerOperation = Nothing
					If (boundLocalDeclarationBase.Kind <> BoundKind.LocalDeclaration) Then
						Dim boundAsNewLocalDeclaration As BoundAsNewLocalDeclarations = DirectCast(boundLocalDeclarationBase, BoundAsNewLocalDeclarations)
						variableDeclaratorOperations = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration, IVariableDeclaratorOperation)(boundAsNewLocalDeclaration.LocalDeclarations, New Func(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration, IVariableDeclaratorOperation)(AddressOf Me.GetVariableDeclarator))
						Dim asClause As AsClauseSyntax = DirectCast(boundAsNewLocalDeclaration.Syntax, VariableDeclaratorSyntax).AsClause
						Me.Create(boundAsNewLocalDeclaration.Initializer)
						Dim operation As IOperation = Me.Create(boundAsNewLocalDeclaration.Initializer)
						variableInitializerOperation = New Microsoft.CodeAnalysis.Operations.VariableInitializerOperation(ImmutableArray(Of ILocalSymbol).Empty, operation, Me._semanticModel, asClause, False)
					Else
						variableDeclaratorOperations = Roslyn.Utilities.EnumerableExtensions.SelectAsArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration, IVariableDeclaratorOperation)(current.Cast(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration)(), New Func(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration, IVariableDeclaratorOperation)(AddressOf Me.GetVariableDeclarator))
						Dim boundLocalDeclaration As Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration = DirectCast(current.Last(), Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration)
						If (boundLocalDeclaration.DeclarationInitializerOpt IsNot Nothing) Then
							Dim parent As VariableDeclaratorSyntax = DirectCast(boundLocalDeclaration.Syntax.Parent, VariableDeclaratorSyntax)
							Dim initializer As SyntaxNode = parent.Initializer
							Dim flag As Boolean = False
							If (boundLocalDeclaration.InitializedByAsNew) Then
								initializer = parent.AsClause
							ElseIf (initializer Is Nothing) Then
								initializer = boundLocalDeclaration.InitializerOpt.Syntax
								flag = True
							End If
							Dim operation1 As IOperation = Me.Create(boundLocalDeclaration.InitializerOpt)
							variableInitializerOperation = New Microsoft.CodeAnalysis.Operations.VariableInitializerOperation(ImmutableArray(Of ILocalSymbol).Empty, operation1, Me._semanticModel, initializer, flag)
						End If
					End If
					instance.Add(New VariableDeclarationOperation(variableDeclaratorOperations, variableInitializerOperation, ImmutableArray(Of IOperation).Empty, Me._semanticModel, current.Key, False))
				End While
			End Using
			Return instance.ToImmutableAndFree()
		End Function

		Private Function GetVariableDeclarator(ByVal boundLocalDeclaration As Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration) As IVariableDeclaratorOperation
			Dim variableInitializerOperation As IVariableInitializerOperation = Nothing
			If (boundLocalDeclaration.IdentifierInitializerOpt IsNot Nothing) Then
				Dim syntax As SyntaxNode = boundLocalDeclaration.Syntax
				Dim operation As IOperation = Me.Create(boundLocalDeclaration.IdentifierInitializerOpt)
				variableInitializerOperation = New Microsoft.CodeAnalysis.Operations.VariableInitializerOperation(ImmutableArray(Of ILocalSymbol).Empty, operation, Me._semanticModel, syntax, True)
			End If
			Dim empty As ImmutableArray(Of IOperation) = ImmutableArray(Of IOperation).Empty
			Return New VariableDeclaratorOperation(boundLocalDeclaration.LocalSymbol, variableInitializerOperation, empty, Me._semanticModel, boundLocalDeclaration.Syntax, boundLocalDeclaration.WasCompilerGenerated)
		End Function

		Private Shared Function IsDelegateCreation(ByVal conversionSyntax As SyntaxNode, ByVal operand As BoundNode, ByVal targetType As TypeSymbol) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			If (targetType.IsDelegateType()) Then
				If (operand.Syntax.Kind() <> SyntaxKind.AddressOfExpression) Then
					flag1 = False
				ElseIf (conversionSyntax.Kind() = SyntaxKind.CTypeExpression OrElse conversionSyntax.Kind() = SyntaxKind.DirectCastExpression OrElse conversionSyntax.Kind() = SyntaxKind.TryCastExpression OrElse conversionSyntax.Kind() = SyntaxKind.ObjectCreationExpression) Then
					flag1 = True
				Else
					flag1 = If(conversionSyntax.Kind() <> SyntaxKind.AddressOfExpression, False, conversionSyntax = operand.Syntax)
				End If
				flag = If(flag1, True, If(operand.Kind = BoundKind.Lambda OrElse operand.Kind = BoundKind.QueryLambda, True, operand.Kind = BoundKind.UnboundLambda))
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function IsEndSubOrFunctionStatement(ByVal syntax As SyntaxNode) As Boolean
			Dim endBlockStatement As SyntaxNode
			Dim endSubOrFunctionStatement As SyntaxNode
			Dim parent As MethodBlockBaseSyntax = TryCast(syntax.Parent, MethodBlockBaseSyntax)
			If (parent IsNot Nothing) Then
				endBlockStatement = parent.EndBlockStatement
			Else
				endBlockStatement = Nothing
			End If
			If (endBlockStatement = syntax) Then
				Return True
			End If
			Dim multiLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax = TryCast(syntax.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax)
			If (multiLineLambdaExpressionSyntax IsNot Nothing) Then
				endSubOrFunctionStatement = multiLineLambdaExpressionSyntax.EndSubOrFunctionStatement
			Else
				endSubOrFunctionStatement = Nothing
			End If
			Return endSubOrFunctionStatement = syntax
		End Function

		Private Shared Function IsMidStatement(ByVal node As BoundNode) As Boolean
			If (node.Kind = BoundKind.Conversion) Then
				node = DirectCast(node, BoundConversion).Operand
				If (node.Kind = BoundKind.UserDefinedConversion) Then
					node = DirectCast(node, BoundUserDefinedConversion).Operand
				End If
			End If
			Return node.Kind = BoundKind.MidResult
		End Function

		Private Shared Function ParameterIsParamArray(ByVal parameter As ParameterSymbol) As Boolean
			If (Not parameter.IsParamArray OrElse parameter.Type.Kind <> SymbolKind.ArrayType) Then
				Return False
			End If
			Return DirectCast(parameter.Type, ArrayTypeSymbol).IsSZArray
		End Function

		Private Sub RecordParent(ByVal placeholderOpt As BoundValuePlaceholderBase, ByVal parent As BoundNode)
			If (placeholderOpt IsNot Nothing) Then
				If (Me._lazyPlaceholderToParentMap Is Nothing) Then
					Interlocked.CompareExchange(Of ConcurrentDictionary(Of BoundValuePlaceholderBase, BoundNode))(Me._lazyPlaceholderToParentMap, New ConcurrentDictionary(Of BoundValuePlaceholderBase, BoundNode)(2, 10, ReferenceEqualityComparer.Instance), Nothing)
				End If
				Me._lazyPlaceholderToParentMap.GetOrAdd(placeholderOpt, parent)
			End If
		End Sub

		Private Shared Function RewriteQueryLambda(ByVal node As BoundQueryLambda) As BoundNode
			Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = DirectCast((New VisualBasicOperationFactory.QueryLambdaRewriterPass1()).VisitQueryLambda(node), Microsoft.CodeAnalysis.VisualBasic.BoundLambda)
			Return (New VisualBasicOperationFactory.QueryLambdaRewriterPass2()).VisitLambda(boundLambda)
		End Function

		Private Function TryGetAdjustedConversionInfo(ByVal topLevelConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversionOrCast, ByVal boundOperand As BoundExpression) As <TupleElementNames(New String() { "Operation", "Conversion", "IsDelegateCreation" })> ValueTuple(Of IOperation, Conversion, Boolean)
			Dim valueTuple As ValueTuple(Of IOperation, Conversion, Boolean)
			If (boundOperand.Kind = BoundKind.Parenthesized) Then
				Dim valueTuple1 As ValueTuple(Of IOperation, Conversion, Boolean) = Me.TryGetAdjustedConversionInfo(topLevelConversion, DirectCast(boundOperand, BoundParenthesized).Expression)
				If (valueTuple1.Item1 Is Nothing) Then
					valueTuple = New ValueTuple(Of IOperation, Conversion, Boolean)()
					Return valueTuple
				End If
				valueTuple = New ValueTuple(Of IOperation, Conversion, Boolean)(New ParenthesizedOperation(valueTuple1.Item1, Me._semanticModel, boundOperand.Syntax, valueTuple1.Item1.Type, boundOperand.ConstantValueOpt, boundOperand.WasCompilerGenerated), valueTuple1.Item2, valueTuple1.Item3)
				Return valueTuple
			ElseIf (boundOperand.Kind <> topLevelConversion.Kind) Then
				If (Not boundOperand.Syntax.IsKind(SyntaxKind.AddressOfExpression) OrElse Not TypeSymbol.Equals(topLevelConversion.Type, boundOperand.Type, TypeCompareKind.ConsiderEverything) OrElse Not VisualBasicOperationFactory.IsDelegateCreation(topLevelConversion.Syntax, boundOperand, boundOperand.Type)) Then
					valueTuple = New ValueTuple(Of IOperation, Conversion, Boolean)()
					Return valueTuple
				End If
				valueTuple = New ValueTuple(Of IOperation, Conversion, Boolean)(Me.CreateDelegateCreationConversionOperand(boundOperand), New Conversion(), True)
				Return valueTuple
			Else
				Dim boundConversionOrCast As Microsoft.CodeAnalysis.VisualBasic.BoundConversionOrCast = DirectCast(boundOperand, Microsoft.CodeAnalysis.VisualBasic.BoundConversionOrCast)
				Dim conversionOperand As BoundExpression = VisualBasicOperationFactory.GetConversionOperand(boundConversionOrCast)
				If (boundConversionOrCast.Syntax <> conversionOperand.Syntax OrElse TypeSymbol.Equals(boundConversionOrCast.Type, conversionOperand.Type, TypeCompareKind.ConsiderEverything) OrElse Not boundConversionOrCast.ExplicitCastInCode OrElse Not TypeSymbol.Equals(topLevelConversion.Type, boundConversionOrCast.Type, TypeCompareKind.ConsiderEverything)) Then
					valueTuple = New ValueTuple(Of IOperation, Conversion, Boolean)()
					Return valueTuple
				End If
				valueTuple = Me.GetConversionInfo(boundConversionOrCast)
				Return valueTuple
			End If
			valueTuple = New ValueTuple(Of IOperation, Conversion, Boolean)()
			Return valueTuple
		End Function

		Private Shared Function TryGetOperatorMethod(ByVal boundUserDefinedUnaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator) As MethodSymbol
			If (boundUserDefinedUnaryOperator.UnderlyingExpression.Kind <> BoundKind.[Call]) Then
				Return Nothing
			End If
			Return boundUserDefinedUnaryOperator.[Call].Method
		End Function

		Private Function TryGetParent(ByVal placeholder As BoundValuePlaceholderBase) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
			If (Me._lazyPlaceholderToParentMap Is Nothing OrElse Not Me._lazyPlaceholderToParentMap.TryGetValue(placeholder, boundNode1)) Then
				boundNode = Nothing
			Else
				boundNode = boundNode1
			End If
			Return boundNode
		End Function

		Private Structure BinaryOperatorInfo
			Public ReadOnly LeftOperand As BoundExpression

			Public ReadOnly RightOperand As BoundExpression

			Public ReadOnly OperatorKind As Microsoft.CodeAnalysis.Operations.BinaryOperatorKind

			Public ReadOnly OperatorMethod As MethodSymbol

			Public ReadOnly IsLifted As Boolean

			Public ReadOnly IsChecked As Boolean

			Public ReadOnly IsCompareText As Boolean

			Public Sub New(ByVal leftOperand As BoundExpression, ByVal rightOperand As BoundExpression, ByVal binaryOperatorKind As Microsoft.CodeAnalysis.Operations.BinaryOperatorKind, ByVal operatorMethod As MethodSymbol, ByVal isLifted As Boolean, ByVal isChecked As Boolean, ByVal isCompareText As Boolean)
				Me = New VisualBasicOperationFactory.BinaryOperatorInfo() With
				{
					.LeftOperand = leftOperand,
					.RightOperand = rightOperand,
					.OperatorKind = binaryOperatorKind,
					.OperatorMethod = operatorMethod,
					.IsLifted = isLifted,
					.IsChecked = isChecked,
					.IsCompareText = isCompareText
				}
			End Sub
		End Structure

		Friend Class Helper
			Public Sub New()
				MyBase.New()
			End Sub

			Friend Shared Function DeriveBinaryOperatorKind(ByVal operatorKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind, ByVal leftOpt As BoundExpression) As Microsoft.CodeAnalysis.Operations.BinaryOperatorKind
				' 
				' Current member / type: Microsoft.CodeAnalysis.Operations.BinaryOperatorKind Microsoft.CodeAnalysis.Operations.VisualBasicOperationFactory/Helper::DeriveBinaryOperatorKind(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind,Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: Microsoft.CodeAnalysis.Operations.BinaryOperatorKind DeriveBinaryOperatorKind(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind,Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 84
				'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 73
				'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 27
				'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 198
				'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 79
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
				'    à ..Visit[,]( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 286
				'    à ..Visit( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 317
				'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 337
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 44
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
				'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
				'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Function

			Friend Shared Function DeriveUnaryOperatorKind(ByVal operatorKind As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind) As Microsoft.CodeAnalysis.Operations.UnaryOperatorKind
				Dim unaryOperatorKind As Microsoft.CodeAnalysis.Operations.UnaryOperatorKind
				Dim unaryOperatorKind1 As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind = operatorKind And Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.OpMask
				Select Case unaryOperatorKind1
					Case Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Plus
						unaryOperatorKind = Microsoft.CodeAnalysis.Operations.UnaryOperatorKind.Plus
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Minus
						unaryOperatorKind = Microsoft.CodeAnalysis.Operations.UnaryOperatorKind.Minus
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.[Not]
						unaryOperatorKind = Microsoft.CodeAnalysis.Operations.UnaryOperatorKind.[Not]
						Exit Select
					Case Else
						If (unaryOperatorKind1 = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.IsTrue) Then
							unaryOperatorKind = Microsoft.CodeAnalysis.Operations.UnaryOperatorKind.[True]
							Exit Select
						ElseIf (unaryOperatorKind1 = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.IsFalse) Then
							unaryOperatorKind = Microsoft.CodeAnalysis.Operations.UnaryOperatorKind.[False]
							Exit Select
						Else
							unaryOperatorKind = Microsoft.CodeAnalysis.Operations.UnaryOperatorKind.None
							Exit Select
						End If
				End Select
				Return unaryOperatorKind
			End Function
		End Class

		Private NotInheritable Class QueryLambdaRewriterPass1
			Inherits BoundTreeRewriterWithStackGuard
			Private _rangeVariableMap As Dictionary(Of RangeVariableSymbol, BoundExpression)

			Public Sub New()
				MyBase.New()
				Me._rangeVariableMap = Nothing
			End Sub

			Protected Overrides Function ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException() As Boolean
				Return False
			End Function

			Public Overrides Function VisitQueryLambda(ByVal node As BoundQueryLambda) As BoundNode
				LocalRewriter.PopulateRangeVariableMapForQueryLambdaRewrite(node, Me._rangeVariableMap, True)
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = LocalRewriter.CreateReturnStatementForQueryLambdaBody(MyBase.VisitExpressionWithStackGuard(node.Expression), node, CObj(node.LambdaSymbol.ReturnType) = CObj(LambdaSymbol.ReturnTypePendingDelegate))
				LocalRewriter.RemoveRangeVariables(node, Me._rangeVariableMap)
				Return LocalRewriter.RewriteQueryLambda(boundStatement, node)
			End Function

			Public Overrides Function VisitRangeVariable(ByVal node As BoundRangeVariable) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundPropertyAccess As BoundExpression = Nothing
				If (Me._rangeVariableMap.TryGetValue(node.RangeVariable, boundPropertyAccess)) Then
					Dim kind As BoundKind = boundPropertyAccess.Kind
					If (kind = BoundKind.PropertyAccess) Then
						Dim boundPropertyAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess = DirectCast(boundPropertyAccess, Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess)
						boundPropertyAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess(node.Syntax, boundPropertyAccess1.PropertySymbol, boundPropertyAccess1.PropertyGroupOpt, boundPropertyAccess1.AccessKind, boundPropertyAccess1.IsWriteable, boundPropertyAccess1.IsWriteable, boundPropertyAccess1.ReceiverOpt, boundPropertyAccess1.Arguments, boundPropertyAccess1.DefaultArguments, boundPropertyAccess1.Type, boundPropertyAccess1.HasErrors)
					ElseIf (kind = BoundKind.Parameter) Then
						Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = DirectCast(boundPropertyAccess, Microsoft.CodeAnalysis.VisualBasic.BoundParameter)
						boundPropertyAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(node.Syntax, boundParameter.ParameterSymbol, boundParameter.IsLValue, boundParameter.SuppressVirtualCalls, boundParameter.Type, boundParameter.HasErrors)
					End If
					If (node.WasCompilerGenerated) Then
						boundPropertyAccess.SetWasCompilerGenerated()
					End If
					boundNode = boundPropertyAccess
				Else
					boundNode = node
				End If
				Return boundNode
			End Function
		End Class

		Private NotInheritable Class QueryLambdaRewriterPass2
			Inherits BoundTreeRewriterWithStackGuard
			Private ReadOnly _uniqueNodes As HashSet(Of BoundParameter)

			Public Sub New()
				MyBase.New()
				Me._uniqueNodes = New HashSet(Of BoundParameter)()
			End Sub

			Protected Overrides Function ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException() As Boolean
				Return False
			End Function

			Public Overrides Function VisitParameter(ByVal node As BoundParameter) As BoundNode
				Dim isQueryLambdaMethod As Boolean
				Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = node.ParameterSymbol
				If (parameterSymbol IsNot Nothing) Then
					isQueryLambdaMethod = parameterSymbol.ContainingSymbol.IsQueryLambdaMethod
				Else
					isQueryLambdaMethod = False
				End If
				If (isQueryLambdaMethod AndAlso Not Me._uniqueNodes.Add(node)) Then
					Dim wasCompilerGenerated As Boolean = node.WasCompilerGenerated
					node = New BoundParameter(node.Syntax, node.ParameterSymbol, node.IsLValue, node.SuppressVirtualCalls, node.Type, node.HasErrors)
					If (wasCompilerGenerated) Then
						node.MakeCompilerGenerated()
					End If
				End If
				Return node
			End Function
		End Class
	End Class
End Namespace